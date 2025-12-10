using UnityEngine;

public class LaunchSpring : MonoBehaviour
{
    [Header("Spring Settings")]
    [SerializeField] private float m_springCooldown = 5f;
    [SerializeField] private Vector2 m_launchVelocity = default;
    [SerializeField] private Color m_activeColor = Color.green;
    [SerializeField] private Color m_cooldownColor = Color.red;

    [Header("Spring Object References")]
    [SerializeField] private SpriteRenderer[] m_renderers;
    private float m_cooldownTimer = 0;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SetRenderColor(m_activeColor);
    }

    // Update is called once per frame
    private void Update()
    {
        m_cooldownTimer -= Time.deltaTime;
        if (m_renderers.Length > 0)
        {
            if (m_renderers[0].color == m_activeColor && m_cooldownTimer > 0)
                SetRenderColor(m_cooldownColor);
            else if (m_renderers[0].color == m_cooldownColor && m_cooldownTimer <= 0)
                SetRenderColor(m_activeColor);
        }
    }

    private void SetRenderColor(Color newColor)
    {
        foreach (var rendeer in m_renderers)
            rendeer.color = newColor;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (m_cooldownTimer > 0)
            return;

        if (collision.tag == "Attack")
        {
            Debug.Log($"{collision.attachedRigidbody.gameObject.name} activated Spring! ");
            var foundPlayer = collision.attachedRigidbody.GetComponent<PlayerController>();
            if (foundPlayer != null)
            {
                var successfulLaunch = foundPlayer.LaunchPlayer(m_launchVelocity);

                if(successfulLaunch)
                    m_cooldownTimer = m_springCooldown;
            }
        }
    }
}
