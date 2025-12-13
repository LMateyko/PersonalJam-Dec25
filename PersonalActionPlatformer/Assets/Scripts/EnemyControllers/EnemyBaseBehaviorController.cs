using UnityEngine;

public class EnemyBaseBehaviorController : MonoBehaviour
{
    [SerializeField] protected EnemyController m_enemyController;

    public virtual void UpdateBehavior() 
    {
        m_enemyController.SetTargetVelocityX(0);
    }
}
