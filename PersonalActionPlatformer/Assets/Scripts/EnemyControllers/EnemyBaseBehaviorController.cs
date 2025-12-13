using UnityEngine;

public class EnemyBaseBehaviorController : MonoBehaviour
{
    //[Header("Base Behavior Settings")]
    //[SerializeField] protected float m_movementSpeed = 3f;

    [Header("Base Behavior Object References")]
    [SerializeField] protected EnemyController m_enemyOwner;

    protected float DistanceToPlayer { get => Mathf.Abs(transform.position.x - m_foundPlayer.transform.position.x); }

    protected bool m_isMoving = false;
    protected PlayerController m_foundPlayer;

    protected virtual void Start()
    {
        m_foundPlayer = FindFirstObjectByType<PlayerController>(FindObjectsInactive.Exclude);
    }

    public virtual void UpdateBehavior() 
    {
        m_enemyOwner.SetTargetVelocityX(0);
    }

    // Returns true if the behavior is updating the current animation
    public virtual void UpdateAnimationState()
    {
        m_enemyOwner.PlayCharacterAnimation("Idle");
    }

    public virtual void OnTakeDamage() { }
}
