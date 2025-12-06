using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Player Settings")]
    [SerializeField] private float m_playerSpeed = 5f;
    [SerializeField] private float m_jumpForce = 5f;

    [Header("Object References")]
    [SerializeField] private Rigidbody2D m_rigidBody;
    [SerializeField] private SpriteRenderer m_renderer;
    [SerializeField] private Animator m_animator;

    private bool m_isGrounded;
    private ContactFilter2D m_terrainFilter;
    private List<ContactPoint2D> m_contactCache = new List<ContactPoint2D>();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Awake()
    {
        m_terrainFilter = new ContactFilter2D { layerMask = LayerMask.GetMask("Terrain") };
        m_rigidBody.linearVelocity = Vector2.zero;
    }

    private void OnMove(InputValue value)
    {
        var moveValue = value.Get<Vector2>();
        Debug.Log($"Move Value: {moveValue}");
        if (moveValue.y > 0)
        {
            StartJump();
        }
        else if (moveValue.x != 0)
        {
            m_rigidBody.linearVelocityX = moveValue.x * m_playerSpeed;
        }
        else
        {
            m_rigidBody.linearVelocityX = 0;
        }

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
            StartJump();
    }

    private void StartJump()
    {
        if (m_isGrounded)
            m_rigidBody.linearVelocityY = m_jumpForce;
    }

    // Update is called once per frame
    private void Update()
    {
        UpdateVelocity();
        SetAnimationState();
    }

    private void UpdateVelocity()
    {
        m_isGrounded = IsGrounded();

        //if (m_isGrounded)
        //    m_rigidBody.linearVelocityY = 0;
    }

    private bool IsGrounded()
    {
        var totalContacts = m_rigidBody.GetContacts(m_terrainFilter, m_contactCache);
        if (totalContacts == 0)
            return false;

        foreach (var contact in m_contactCache)
        {
            if (contact.normal == Vector2.up)
                return true;
        }

        return false;
    }

    private void SetAnimationState()
    {
        int targetAnimation = 0;
        if (m_rigidBody.linearVelocity == Vector2.zero)
            targetAnimation = Animator.StringToHash("Knight_Idle");
        else if (m_rigidBody.linearVelocityY > 0.01f)
            targetAnimation = Animator.StringToHash("Knight_Jump");
        else if (m_rigidBody.linearVelocityY < -0.01f)
            targetAnimation = Animator.StringToHash("Knight_Fall");
        else if (m_rigidBody.linearVelocityX > 0.01f || m_rigidBody.linearVelocityX < -0.01f)
            targetAnimation = Animator.StringToHash("Knight_Run");
        else
            targetAnimation = Animator.StringToHash("Knight_Idle");

        if (targetAnimation != m_animator.GetCurrentAnimatorStateInfo(0).shortNameHash)
        {
            m_animator.Play(targetAnimation);
        }
            
    }

}
