using System.Collections.Generic;
using UnityEngine;

public class BaseCharacterController : MonoBehaviour
{
    [Header("Character Settings")]
    [SerializeField] protected string m_animationPrefix;
    [SerializeField] private int m_baseHealth = 3;

    [Header("Character Object References")]
    [SerializeField] protected Rigidbody2D m_rigidBody;
    [SerializeField] protected Collider2D m_attackCollider;
    [SerializeField] protected SpriteRenderer m_renderer;
    [SerializeField] private Animator m_animator;

    protected int m_currentHealth = 0;
    protected bool m_isAttacking = false;
    protected bool m_isGrounded = false;
    protected float m_targetVelocityX = 0;

    private const float m_hitStun = 0.25f;
    private ContactFilter2D m_terrainFilter;
    private List<ContactPoint2D> m_contactCache = new List<ContactPoint2D>();

    public bool IsDead { get => m_currentHealth <= 0; }
    public bool IsDying { get => IsDead && IsAnimationPlaying("Death"); }
    protected bool AnimationHasFinished { get => m_animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f; }
    protected float TimeInAnimation { get => m_animator.GetCurrentAnimatorStateInfo(0).normalizedTime * m_animator.GetCurrentAnimatorStateInfo(0).length; }
    protected bool IsHitStunned { get => IsAnimationPlaying("Hit") && (TimeInAnimation < m_hitStun || !m_isGrounded); }

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

    protected virtual void Awake()
    {
        m_currentHealth = m_baseHealth;

        m_terrainFilter = new ContactFilter2D { layerMask = LayerMask.GetMask("Terrain") };
        m_rigidBody.linearVelocity = Vector2.zero;

        Physics2D.IgnoreCollision(m_rigidBody.GetComponent<Collider2D>(), m_attackCollider, true);
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        m_isGrounded = IsGrounded();

        if (IsDead)
        {
            if (IsDying && AnimationHasFinished)
                OnDeath();

            return;
        }

        if (IsHitStunned)
        {
            return;
        }

        UpdateVelocity();
        SetAnimationState();
    }

    protected virtual void SetAnimationState() 
    {
        // Default to the Idle Animation;
        PlayCharacterAnimation("Idle");
    }

    protected virtual void OnDeath() { }

    private void UpdateVelocity()
    {
        

        //if (m_isGrounded)
        //    m_rigidBody.linearVelocityY = 0;

        if (m_isAttacking && m_isGrounded)
            m_rigidBody.linearVelocityX = 0;
        else
            m_rigidBody.linearVelocityX = m_targetVelocityX;
    }

    private bool IsGrounded()
    {
        var totalContacts = m_rigidBody.GetContacts(m_terrainFilter, m_contactCache);
        if (totalContacts == 0)
            return false;

        foreach (var contact in m_contactCache)
        {
            if (contact.normal == Vector2.up)
                return true;
        }

        return false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Attack")
        {
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
            PlayCharacterAnimation("Death");
        else
            PlayCharacterAnimation("Hit");
    }
}
