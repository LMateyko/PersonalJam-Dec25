using UnityEngine;

public class EnemyController : BaseCharacterController
{
    [Header("Enemy Settings")]    
    [SerializeField] protected bool m_persistCorpse = false;

    [Header("Enemy Object References")]
    [SerializeField] protected Collider2D m_hurtBox = default;
    [SerializeField] protected EnemyBaseBehaviorController m_behaviorController = default;


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

    protected override void TakeDamage()
    {
        base.TakeDamage();

        m_behaviorController.OnTakeDamage();
    }

    protected override void WhileDying()
    {
        base.WhileDying();

        m_hurtBox.enabled = false;
    }

    protected override void OnDeath()
    {
        base.OnDeath();

        if(!m_persistCorpse)
            Destroy(gameObject);
    }
}
