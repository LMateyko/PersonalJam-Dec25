using UnityEngine;

public class EnemyController : CharacterController
{
    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log($"On Collision Enter hitting  {gameObject.name} | Collision Name: {collision.gameObject.name} | Other Collider Name: {collision.otherCollider.gameObject.name}");
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log($"On Trigger Enter hitting {gameObject.name} | {collision.name} | {collision.attachedRigidbody.gameObject.name}");

        if (collision.tag == "Attack")
        {
            Debug.Log($"{collision.attachedRigidbody.gameObject.name} hit {gameObject.name}. ");
            Destroy(gameObject);
        }

    }
}
