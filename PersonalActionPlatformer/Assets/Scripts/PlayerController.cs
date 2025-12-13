using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerController : BaseCharacterController, InputSystem_Player.IPlayerActions
{
    [Header("Player Settings")]
    [SerializeField] private float m_playerSpeed = 5f;
    [SerializeField] private float m_jumpForce = 5f;

    public Action<int> OnPlayerHurt;

    private InputSystem_Player m_playerInputSystem;
    private InputSystem_Player.PlayerActions m_playerActions;

    private Vector2 m_lastSafePosition;
    public bool LaunchPlayer(Vector2 launchVelocity, bool includeFacing = false)
    {
        if (IsGrounded)
            return false;

        if (launchVelocity.y != 0)
            m_rigidBody.linearVelocityY = launchVelocity.y;

        if (launchVelocity.x != 0)
            m_rigidBody.linearVelocityX = includeFacing ? launchVelocity.x * transform.localScale.x : launchVelocity.x;

        return true;
    }

    protected override void Awake()
    {
        base.Awake();

        m_playerInputSystem = new InputSystem_Player();
        m_playerActions = m_playerInputSystem.Player;
        m_playerActions.AddCallbacks(this);

        m_lastSafePosition = transform.position;
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
            FaceRight();
        else if (moveValue.x < 0)
            FaceLeft();
    }

    public void OnAttack(InputAction.CallbackContext context)
    {
        if (IsAttacking)
            return;

        if (context.performed)
        {
            IsAttacking = true;

            if (IsGrounded)
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
        if (IsGrounded && !IsAttacking)
            m_rigidBody.linearVelocityY = m_jumpForce;
    }

    protected override void Update()
    {
        base.Update();

        // Disable Player Controls while stunned or Dead
        if (IsDead || IsHitStunned)
        {
            if (m_playerActions.enabled)
                m_playerActions.Disable();

            return;
        }
        else if (!m_playerActions.enabled)
            m_playerActions.Enable();

        if (IsGrounded && !IsFacingCliff)
            m_lastSafePosition = transform.position;

        SetAnimationState();
    }

    protected void SetAnimationState()
    {
        if(IsAttacking)
        {
            if ((IsAnimationPlaying("Attack_1") || IsAnimationPlaying("Attack_2"))
                && AnimationHasFinished)
            {
                IsAttacking = false;
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

    public override void EnterPit()
    {
        transform.position = m_lastSafePosition;

        m_currentHealth--;
        if (IsDead)
            PlayCharacterAnimation("Death");

        OnPlayerHurt?.Invoke(m_currentHealth);
    }

    protected override void TakeDamage()
    {
        base.TakeDamage();

        OnPlayerHurt?.Invoke(m_currentHealth);
        IsAttacking = false;

        if(IsHitStunned)
        {
            m_rigidBody.linearVelocityX = IsFacingRight? -4.5f : 4.5f;
            m_rigidBody.linearVelocityY = 4.5f;
        }
        else
        {
            m_rigidBody.linearVelocityX = 0f;
            m_rigidBody.linearVelocityY = 0f;
        }
    }

    protected override void OnDeath()
    {
        base.OnDeath();

        // TODO: Start a fade then reload the scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().name, LoadSceneMode.Single);
    }
}
