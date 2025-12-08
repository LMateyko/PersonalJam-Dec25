using UnityEngine;

public class EnemyController : BaseCharacterController
{
    private enum MovementType
    {
        None,
        Patrol,
        Seek
    }

    [Header("Enemy Settings")]
    [SerializeField] private MovementType m_enemyMovementType;
    [SerializeField] private float m_movementSpeed = 3f;

    protected override void Update()
    {
        base.Update();
        if(IsHitStunned || IsDead)
        {
            m_rigidBody.linearVelocityX = 0f;
            return;
        }

        switch (m_enemyMovementType)
        {
            case MovementType.Patrol:
                UpdatePatrolBehavior();
                break;
            case MovementType.Seek:
                UpdateSeekBehavior();
                break;
            default:
                m_targetVelocityX = 0f;
                break;
        }
    }

    protected override void SetAnimationState()
    {
        if (m_rigidBody.linearVelocityX > 0.01f || m_rigidBody.linearVelocityX < -0.01f)
            PlayCharacterAnimation("Run");
        else
            PlayCharacterAnimation("Idle");
    }

    protected override void OnDeath()
    {
        base.OnDeath();

        Destroy(gameObject);
    }

    private void UpdatePatrolBehavior()
    {
        if(IsFacingWall || IsFacingCliff)
        {
            var localScale = transform.localScale;
            localScale.x *= -1;
            transform.localScale = localScale;
        }

        m_targetVelocityX = m_movementSpeed * transform.localScale.x;
    }

    private void UpdateSeekBehavior()
    {

    }
}
