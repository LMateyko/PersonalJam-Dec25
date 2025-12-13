using UnityEngine;

public class EnemyController : BaseCharacterController
{
    [Header("Enemy Settings")]
    [SerializeField] protected float m_movementSpeed = 3f;

    [Header("Enemy Object References")]
    [SerializeField] protected Collider2D m_hurtBox = default;
    [SerializeField] protected EnemyBaseBehaviorController m_behaviorController = default;

    public float MovementSpeed { get => m_movementSpeed; }

    protected override void Update()
    {
        base.Update();
        if (IsHitStunned || IsDead)
        {
            m_rigidBody.linearVelocityX = 0f;
            return;
        }

        m_behaviorController.UpdateBehavior();
    }

    protected override void SetAnimationState()
    {
        if (m_rigidBody.linearVelocityX > 0.01f || m_rigidBody.linearVelocityX < -0.01f)
            PlayCharacterAnimation("Run");
        else
            PlayCharacterAnimation("Idle");
    }

    protected override void WhileDying()
    {
        base.WhileDying();

        m_hurtBox.enabled = false;
    }

    protected override void OnDeath()
    {
        base.OnDeath();

        Destroy(gameObject);
    }
}
