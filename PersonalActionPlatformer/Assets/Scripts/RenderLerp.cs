using UnityEngine;

public class RenderLerp : MonoBehaviour
{
    [SerializeField] private Vector2 m_targetLocalPosition = default;
    [SerializeField] private float m_lerpDuration = 1f;

    private Vector2 m_startPosition;
    private bool m_forward = true;
    private float m_lerpTimer = 0f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        m_startPosition = transform.localPosition;
    }

    // Update is called once per frame
    void Update()
    {
        if (m_forward)
        {
            transform.localPosition = Vector2.Lerp(m_startPosition, m_targetLocalPosition, m_lerpTimer/m_lerpDuration);
        }
        else
        {
            transform.localPosition = Vector2.Lerp(m_targetLocalPosition, m_startPosition, m_lerpTimer/m_lerpDuration);
        }

        m_lerpTimer += Time.deltaTime;
        if (m_lerpTimer >= m_lerpDuration)
        {
            m_forward = !m_forward;
            m_lerpTimer = 0;
        }
    }
}
