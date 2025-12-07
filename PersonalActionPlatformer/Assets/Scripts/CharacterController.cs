using System.Collections.Generic;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;

public class CharacterController : MonoBehaviour
{
    [Header("Character Object References")]
    [SerializeField] protected Rigidbody2D m_rigidBody;
    [SerializeField] protected Collider2D m_attackCollider;
    [SerializeField] protected SpriteRenderer m_renderer;
    [SerializeField] protected Animator m_animator;

    protected bool m_isAttacking = false;
    protected bool m_isGrounded = false;
    protected float m_targetVelocityX = 0;

    private ContactFilter2D m_terrainFilter;
    private List<ContactPoint2D> m_contactCache = new List<ContactPoint2D>();

    protected virtual void Awake()
    {
        m_terrainFilter = new ContactFilter2D { layerMask = LayerMask.GetMask("Terrain") };
        m_rigidBody.linearVelocity = Vector2.zero;

        Physics2D.IgnoreCollision(m_rigidBody.GetComponent<Collider2D>(), m_attackCollider, true);
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        UpdateVelocity();
        SetAnimationState();
    }

    protected virtual void SetAnimationState() { }

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
}
