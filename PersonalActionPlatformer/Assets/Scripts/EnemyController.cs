using UnityEngine;

public class EnemyController : BaseCharacterController
{
    protected override void OnDeath()
    {
        base.OnDeath();

        Destroy(gameObject);
    }
}
