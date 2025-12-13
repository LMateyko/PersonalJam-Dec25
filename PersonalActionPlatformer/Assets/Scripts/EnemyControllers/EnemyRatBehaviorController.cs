using UnityEngine;

public class EnemyRatController : EnemyBaseBehaviorController
{
    //[Header("Rat Settings")]
    //[Tooltip("How long the rat pauses when they are set to turn around.")]
    //[SerializeField] private bool m_pauseOnTurn = false;

    public override void UpdateBehavior() 
    {
        if (m_enemyOwner.IsFacingWall || m_enemyOwner.IsFacingCliff)
        {
            var localScale = transform.localScale;
            localScale.x *= -1;
            transform.localScale = localScale;
        }

        m_enemyOwner.SetTargetVelocityX(m_enemyOwner.MovementSpeed * transform.localScale.x);
    }
}
