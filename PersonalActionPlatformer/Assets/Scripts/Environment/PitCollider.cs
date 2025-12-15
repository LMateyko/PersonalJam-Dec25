using UnityEngine;

public class PitCollider : MonoBehaviour
{
    [SerializeField] private AudioClip m_fallSFX;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        var foundCharacter = collision.attachedRigidbody.GetComponent<BaseCharacterController>();
        foundCharacter.PlaySFX(m_fallSFX);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        var foundCharacter = collision.attachedRigidbody.GetComponent<BaseCharacterController>();
        foundCharacter.EnterPit();
    }
}
