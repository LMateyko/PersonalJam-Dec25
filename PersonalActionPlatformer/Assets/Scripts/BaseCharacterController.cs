using System.Collections.Generic;
using UnityEngine;

public class BaseCharacterController : MonoBehaviour
{
    [Header("Character Settings")]
    [SerializeField] protected string m_animationPrefix;
    [SerializeField] private int m_baseHealth = 3;

    [Header("Character Object References")]
    [SerializeField] protected Rigidbody2D m_rigidBody;
    [SerializeField] protected Collider2D m_bodyCollider; 
    [SerializeField] protected Collider2D m_attackCollider;
    [SerializeField] protected SpriteRenderer m_renderer;
    [SerializeField] private Animator m_animator;
    [SerializeField] private AudioSource m_sfxAudioSource;

    [Header("Character Audio Settings")]
    [SerializeField] private AudioClip m_hitSFX;
    [SerializeField] private AudioClip m_deathSFX;

    protected int m_currentHealth = 0;
    protected float m_targetVelocityX = 0;

    private readonly Vector3 FaceRightScale = new Vector3(1, 1, 1);
    private readonly Vector3 FaceLeftScale = new Vector3(-1, 1, 1);

    private const float HitStun = 0.25f;
    private const float CliffEdgeOffset = 0.1f;
    private ContactFilter2D m_terrainFilter;
    private List<ContactPoint2D> m_contactCache = new List<ContactPoint2D>();

    public bool IsAttacking { get; protected set; } = false;
    public bool IsGrounded { get; private set; } = false;
    public bool IsFacingWall { get; private set; } = false;
    public bool IsFacingCliff { get; private set; } = false;
    public bool IsFacingRight { get => transform.localScale.x > 0; }

    public int MaxHealth { get => m_baseHealth; }
    public int CurrentHealth { get => m_currentHealth; }
    public bool IsDead { get => m_currentHealth <= 0; }
    public bool IsDying { get => IsDead && IsAnimationPlaying("Death"); }
    public bool AnimationHasFinished { get => m_animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f; }
    public float TimeInAnimation { get => m_animator.GetCurrentAnimatorStateInfo(0).normalizedTime * m_animator.GetCurrentAnimatorStateInfo(0).length; }
    public bool IsHitStunned { get => IsAnimationPlaying("Hit") && (TimeInAnimation < HitStun || !IsGrounded); }

    #region Animation Helpers
    public void PlayCharacterAnimation(string animationName)
    {
        var fullAnimationName = $"{m_animationPrefix}_{animationName}";
        if(!IsAnimationPlaying(animationName))
        {
            //Debug.Log($"Swap {gameObject.name} to {fullAnimationName} Animation");
            m_animator.Play(fullAnimationName);
            m_animator.Update(0);
        }
            
    }

    public bool IsAnimationPlaying(string animationName)
    {
        var fullAnimationName = $"{m_animationPrefix}_{animationName}";
        return m_animator.GetCurrentAnimatorStateInfo(0).shortNameHash == Animator.StringToHash(fullAnimationName);
    }
    #endregion

    public void PlaySFX(AudioClip newSFX, float audioScale = 1f, float pitch = 1f)
    {
        if (newSFX == null)
            return;

        m_sfxAudioSource.pitch = pitch;
        m_sfxAudioSource.PlayOneShot(newSFX, audioScale);
    }

    public void FaceRight() { transform.localScale = FaceRightScale; }
    public void FaceLeft() { transform.localScale = FaceLeftScale; }

    public void SetTargetVelocityX(float targetVelocityX)
    {
        m_targetVelocityX = targetVelocityX;
    }

    public void LeapForward(Vector2 leapVelocity)
    {
        leapVelocity.x *= transform.localScale.x;
        m_targetVelocityX = leapVelocity.x;
        m_rigidBody.linearVelocityY = leapVelocity.y;
    }

    public virtual void EnterPit()
    {
        Destroy(gameObject);
    }

    protected virtual void Awake()
    {
        m_currentHealth = m_baseHealth;

        m_terrainFilter = new ContactFilter2D { layerMask = LayerMask.GetMask("Terrain") };
        m_rigidBody.linearVelocity = Vector2.zero;

        Physics2D.IgnoreCollision(m_rigidBody.GetComponent<Collider2D>(), m_attackCollider, true);
        m_attackCollider.enabled = false;
    }

    protected virtual void Start()
    {
        // Default to the Idle Animation;
        PlayCharacterAnimation("Idle");
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        CheckCollisionContacts();

        if (IsDead)
        {
            WhileDying();
            return;
        }

        if (IsHitStunned)
        {
            return;
        }

        UpdateVelocity();
    }

    protected virtual void WhileDying()
    {
        m_attackCollider.enabled = false;

        if (IsDying && AnimationHasFinished)
            OnDeath();
    }

    protected virtual void OnDeath() 
    {
        m_rigidBody.Sleep();
        m_bodyCollider.enabled = false;
    }

    private void UpdateVelocity()
    {
        if (IsAttacking && IsGrounded)
            m_rigidBody.linearVelocityX = 0;
        else
            m_rigidBody.linearVelocityX = m_targetVelocityX;
    }

    private void CheckCollisionContacts()
    {
        IsGrounded = false;
        IsFacingWall = false;
        IsFacingCliff = false;

        var totalContacts = m_rigidBody.GetContacts(m_terrainFilter, m_contactCache);
        if (totalContacts == 0)
            return;

        float trackedPosX = IsFacingRight? float.MinValue : float.MaxValue;
        float trackedPosY = 0;

        var adjustedNormal = Vector2.zero;
        foreach (var contact in m_contactCache)
        {
            // Adjust the normal to resolve values nearly 0; 
            adjustedNormal = contact.normal;
            if (adjustedNormal.x > -CliffEdgeOffset && adjustedNormal.x < CliffEdgeOffset)
                adjustedNormal.x = 0;
            if (adjustedNormal.y > -CliffEdgeOffset && adjustedNormal.y < CliffEdgeOffset)
                adjustedNormal.y = 0;

            if (adjustedNormal == Vector2.up)
            {
                IsGrounded = true;

                if ((!IsFacingRight && contact.point.x < trackedPosX)
                    || (IsFacingRight && contact.point.x > trackedPosX))
                {
                    trackedPosX = contact.point.x;
                    trackedPosY = contact.point.y;
                }

                Debug.DrawLine(contact.point, contact.point + (Vector2.up) * 2f, Color.green);
            }     

            if ((!IsFacingRight && adjustedNormal == Vector2.right)
                || (IsFacingRight && adjustedNormal == Vector2.left))
            {
                IsFacingWall = true;
            }
        }

        if ((!IsFacingRight && trackedPosX > m_bodyCollider.bounds.min.x + CliffEdgeOffset)
            || (IsFacingRight && trackedPosX < m_bodyCollider.bounds.max.x - CliffEdgeOffset))
        {
            var contactPos = new Vector2(trackedPosX, trackedPosY);
            Debug.DrawLine(contactPos, contactPos + (Vector2.up) * 2f, Color.red);
            IsFacingCliff = true;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Attack")
        {
            // Parent objects are either both players or both enemies
            if (collision.attachedRigidbody.CompareTag(gameObject.tag))
                return;

            if (IsDead || IsHitStunned)
                return;

            Debug.Log($"{collision.attachedRigidbody.gameObject.name} hit {gameObject.name}. ");
            TakeDamage();
        }

    }

    protected virtual void TakeDamage()
    {
        m_currentHealth--;
        if (IsDead)
        {
            Die();
        }
        else
        {
            GetHit();
        }
            
    }

    protected virtual void GetHit()
    {
        PlaySFX(m_hitSFX);
        PlayCharacterAnimation("Hit");
    }

    protected virtual void Die()
    {
        float deathPitch = 1f;

        if (m_deathSFX != null && m_hitSFX != null && m_deathSFX.name == m_hitSFX.name)
            deathPitch = 0.5f;

        PlaySFX(m_deathSFX, pitch: deathPitch);
        PlayCharacterAnimation("Death");
    }
}
