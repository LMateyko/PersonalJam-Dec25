using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : BaseCharacterController, InputSystem_Player.IPlayerActions
{
    [Header("Player Settings")]
    [SerializeField] private float m_playerSpeed = 5f;
    [SerializeField] private float m_jumpForce = 5f;

    private InputSystem_Player m_playerInputSystem;
    private InputSystem_Player.PlayerActions m_playerActions;

    protected override void Awake()
    {
        base.Awake();

        m_playerInputSystem = new InputSystem_Player();
        m_playerActions = m_playerInputSystem.Player;
        m_playerActions.AddCallbacks(this);
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
                PlayCharacterAnimation("Attack_1");
            }
            else
            {
                PlayCharacterAnimation("Attack_2");
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

    protected override void SetAnimationState()
    {
        if(m_isAttacking)
        {
            if ((IsAnimationPlaying("Attack_1") || IsAnimationPlaying("Attack_2"))
                && AnimationHasFinished)
            {
                m_isAttacking = false;
            }
            else
                return;
        }

        if (m_rigidBody.linearVelocity == Vector2.zero)
            PlayCharacterAnimation("Idle");
        else if (m_rigidBody.linearVelocityY > 0.01f)
            PlayCharacterAnimation("Jump");
        else if (m_rigidBody.linearVelocityY < -0.01f)
            PlayCharacterAnimation("Fall");
        else if (m_rigidBody.linearVelocityX > 0.01f || m_rigidBody.linearVelocityX < -0.01f)
            PlayCharacterAnimation("Run");
        else
            PlayCharacterAnimation("Idle");            
    }
}
