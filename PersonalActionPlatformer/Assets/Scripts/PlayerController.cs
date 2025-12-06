using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private SpriteRenderer m_renderer;
    [SerializeField] private Animator m_animator;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
    }

    private void OnMove(InputValue value)
    {
        var moveValue = value.Get<Vector2>();
        Debug.Log($"Move Value: {moveValue}");
        if (moveValue.y > 0)
            m_animator.Play("Knight_Jump");
        else if (moveValue.x != 0)
            m_animator.Play("Knight_Run");
        else
            m_animator.Play("Knight_Idle");

        if (moveValue.x > 0)
            transform.localScale = new Vector3(1, 1, 1);
        else if (moveValue.x < 0)
            transform.localScale = new Vector3(-1, 1, 1);
    }

    private void OnJump(InputValue value)
    {
        var jumpValue = value.Get<float>();
        Debug.Log($"Jump Value: {jumpValue}");
        if (jumpValue > 0)
            m_animator.Play("Knight_Jump");
        else
            m_animator.Play("Knight_Idle");
    }

    // Update is called once per frame
    private void Update()
    {
        
    }

}
