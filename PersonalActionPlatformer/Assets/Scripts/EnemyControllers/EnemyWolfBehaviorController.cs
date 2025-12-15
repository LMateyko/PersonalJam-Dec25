using System;
using System.Runtime.CompilerServices;
using UnityEngine;

public class EnemyWolfBehaviorController : EnemyBaseBehaviorController
{
    [Header("Wolf Behavior Settings")]
    [Tooltip("How far the wolf patrols from their starting position. ")]
    [SerializeField] private float m_patrolRange = 3f;
    [Tooltip("Movement Speed of the wolf while patroling")]
    [SerializeField] private float m_patrolSpeed = 3f;

    [Tooltip("Distance from the player before growling")]
    [SerializeField] private float m_telegraphRange = 2f;
    [Tooltip("How long the wolf will growl before attacking.")]
    [SerializeField] private float m_telegraphDuration = 0.5f;
    [Tooltip("Leap velocity while attacking")]
    [SerializeField] private Vector2 m_attackLeapVelocity;

    [SerializeField] private AudioClip m_wolfGrowlSFX;
    [SerializeField] private AudioClip m_wolfAttackSFX;

    private bool InPatrolRange { get => Mathf.Abs(m_startingPosition.x - transform.position.x) < m_patrolRange - 0.25f; }

    private Vector3 m_startingPosition;
    private float m_telegraphTimer = 0f;

    protected override void Start()
    {
        base.Start();

        m_startingPosition = transform.position;
    }

    public override void UpdateBehavior()
    {
        if (m_foundPlayer == null)
        {
            UpdatePatrolBehavior();
            return;
        }

        // Wait for the attack to finish
        if (m_enemyOwner.IsAnimationPlaying("Attack") && !m_enemyOwner.AnimationHasFinished)
            return;

        if(m_telegraphTimer >= m_telegraphDuration)
            UpdateAttackBehavior();
        else if (DistanceToPlayer <= m_telegraphRange || m_enemyOwner.IsAnimationPlaying("Growl"))
            UpdateTelegraphBehavior();
        else
            UpdatePatrolBehavior();
    }

    private void UpdateTelegraphBehavior()
    {
        if (!m_enemyOwner.IsAnimationPlaying("Growl"))
            m_enemyOwner.PlaySFX(m_wolfGrowlSFX);

        if (m_foundPlayer.transform.position.x > m_enemyOwner.transform.position.x)
            m_enemyOwner.FaceRight();
        else
            m_enemyOwner.FaceLeft();

        m_enemyOwner.SetTargetVelocityX(0);
        m_enemyOwner.PlayCharacterAnimation("Growl");

        m_telegraphTimer += Time.deltaTime;
    }

    private void UpdateAttackBehavior()
    {
        m_telegraphTimer = 0f;
        m_enemyOwner.PlaySFX(m_wolfAttackSFX);
        m_enemyOwner.LeapForward(m_attackLeapVelocity);
        m_enemyOwner.PlayCharacterAnimation("Attack");
    }

    private void UpdatePatrolBehavior()
    {
        if (m_enemyOwner.IsFacingWall || m_enemyOwner.IsFacingCliff)
        {
            var localScale = transform.localScale;
            localScale.x *= -1;
            transform.localScale = localScale;
        }
        else if(!InPatrolRange)
        {
            if (m_startingPosition.x < transform.position.x)
                m_enemyOwner.FaceLeft();
            else
                m_enemyOwner.FaceRight();
        }

        m_telegraphTimer = 0f;
        m_enemyOwner.SetTargetVelocityX(m_patrolSpeed * transform.localScale.x);
        m_enemyOwner.PlayCharacterAnimation("Run");
    }

    public override void OnTakeDamage()
    {
        base.OnTakeDamage();

        m_telegraphTimer = 0f;
    }

    private void OnDrawGizmosSelected()
    {
        var drawPosition = Application.isPlaying ? m_startingPosition : transform.position;

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, m_telegraphRange);

        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(drawPosition, m_patrolRange);
    }
}
