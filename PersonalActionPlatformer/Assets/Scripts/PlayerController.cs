using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour, InputSystem_Player.IPlayerActions
{
    [Header("Player Settings")]
    [SerializeField] private float m_playerSpeed = 5f;
    [SerializeField] private float m_jumpForce = 5f;

    [Header("Object References")]
    [SerializeField] private Rigidbody2D m_rigidBody;
    [SerializeField] private SpriteRenderer m_renderer;
    [SerializeField] private Animator m_animator;

    private InputSystem_Player m_playerInputSystem;
    private InputSystem_Player.PlayerActions m_playerActions;

    private bool m_isAttacking = false;
    private bool m_isGrounded = false;
    private float m_targetVelocityX = 0;

    private ContactFilter2D m_terrainFilter;
    private List<ContactPoint2D> m_contactCache = new List<ContactPoint2D>();

    private void Awake()
    {
        m_playerInputSystem = new InputSystem_Player();
        m_playerActions = m_playerInputSystem.Player;
        m_playerActions.AddCallbacks(this);

        m_terrainFilter = new ContactFilter2D { layerMask = LayerMask.GetMask("Terrain") };
        m_rigidBody.linearVelocity = Vector2.zero;
    }


    private void OnEnable()
    {
        m_playerActions.Enable();
    }

    private void OnDisable()
    {
        m_playerActions.Disable();
    }

    private void OnDestroy()
    {
        m_playerInputSystem.Dispose();
    }

    #region InputSystem_Player.IPlayerActions Implementation
    public void OnMove(InputAction.CallbackContext context)
    {
        var moveValue = context.ReadValue<Vector2>();
        if (moveValue.x != 0)
        {
            m_targetVelocityX = moveValue.x * m_playerSpeed;
        }
        else
        {
            m_targetVelocityX = 0;
        }

        if (moveValue.x > 0)
            transform.localScale = new Vector3(1, 1, 1);
        else if (moveValue.x < 0)
            transform.localScale = new Vector3(-1, 1, 1);
    }

    public void OnAttack(InputAction.CallbackContext context)
    {
        if (m_isAttacking)
            return;

        if (context.performed)
        {
            m_isAttacking = true;

            if (m_isGrounded)
            {
                m_rigidBody.linearVelocityX = 0;
                m_animator.Play("Knight_Attack_1", 0, 0);
            }
            else
            {
                m_animator.Play("Knight_Attack_2");
            }
        }
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.performed)
            StartJump();
    }

    #endregion

    private void StartJump()
    {
        if (m_isGrounded && !m_isAttacking)
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

        if (m_isAttacking && m_isGrounded)
            m_rigidBody.linearVelocityX = 0;
        else
            m_rigidBody.linearVelocityX = m_targetVelocityX;
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
        if(m_isAttacking)
        {
            if ((m_animator.GetCurrentAnimatorStateInfo(0).shortNameHash == Animator.StringToHash("Knight_Attack_1")
                || m_animator.GetCurrentAnimatorStateInfo(0).shortNameHash == Animator.StringToHash("Knight_Attack_2"))
                && m_animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f)
            {
                m_isAttacking = false;
            }
            else
                return;
        }

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
