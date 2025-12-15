using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Events;

public class BasicPlayerTrigger : MonoBehaviour
{
    [SerializeField] private UnityEvent m_onEnterEvent;
    [SerializeField] private UnityEvent m_onExitEvent;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.attachedRigidbody.CompareTag("Player") && !collision.CompareTag("Attack"))
            m_onEnterEvent?.Invoke();
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.attachedRigidbody.CompareTag("Player") && !collision.CompareTag("Attack"))
            m_onExitEvent?.Invoke();
    }
}
