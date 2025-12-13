using UnityEngine;

public class EnemyRatController : EnemyBaseBehaviorController
{
    //[Header("Rat Settings")]
    //[Tooltip("How long the rat pauses when they are set to turn around.")]
    //[SerializeField] private bool m_pauseOnTurn = false;

    public override void UpdateBehavior() 
    {
        if (m_enemyController.IsFacingWall || m_enemyController.IsFacingCliff)
        {
            var localScale = transform.localScale;
            localScale.x *= -1;
            transform.localScale = localScale;
        }

        m_enemyController.SetTargetVelocityX(m_enemyController.MovementSpeed * transform.localScale.x);
    }
}
