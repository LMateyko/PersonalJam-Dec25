using UnityEngine;

public class PitCollider : MonoBehaviour
{
    private void OnTriggerExit2D(Collider2D collision)
    {
        var foundCharacter = collision.attachedRigidbody.GetComponent<BaseCharacterController>();
        foundCharacter.EnterPit();
    }
}
