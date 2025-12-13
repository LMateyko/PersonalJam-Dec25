using UnityEngine;

public class EnemyBaseBehaviorController : MonoBehaviour
{
    [SerializeField] protected EnemyController m_enemyOwner;

    public virtual void UpdateBehavior() 
    {
        m_enemyOwner.SetTargetVelocityX(0);
    }
}
