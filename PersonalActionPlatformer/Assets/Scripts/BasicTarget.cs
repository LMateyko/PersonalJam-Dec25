using UnityEngine;

public class BasicTarget : MonoBehaviour
{
    [Header("Target Settings")]
    [SerializeField] private float m_spinSpeed = 5f;
    [SerializeField] private BoxCollider2D m_spawnBounds = default;

    [Header("Object References")]
    [SerializeField] private SpriteRenderer m_renderer;

    private const int m_flickerFrames = 25;

    private Color m_startingColor = default;
    private int m_flickerCount = 0;
    private bool m_isHit = false;

    private void Awake()
    {
        m_startingColor = m_renderer.color;
    }

    // Update is called once per frame
    private void Update()
    {
        transform.Rotate(Vector3.back * Time.deltaTime * m_spinSpeed);

        if(m_isHit)
        {
            m_renderer.color = Color.red;
            m_flickerCount--;
            if (m_flickerCount <= 0)
                m_renderer.color = m_startingColor;
            
            if(m_flickerCount < -m_flickerFrames)
            {
                Vector3 newPos = Vector3.zero;
                newPos.x = Random.Range(m_spawnBounds.bounds.min.x, m_spawnBounds.bounds.max.x);
                newPos.y = Random.Range(m_spawnBounds.bounds.min.y, m_spawnBounds.bounds.max.y);

                transform.position = newPos;
                m_isHit = false;

                m_startingColor = new Color(Random.value, Random.value, Random.value);
                m_renderer.color = m_startingColor;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (m_isHit)
            return;

        if(collision.tag == "Attack")
        {
            Debug.Log($"{collision.attachedRigidbody.gameObject.name} hit target. ");
            m_spinSpeed *= 1.05f;
            m_flickerCount = m_flickerFrames;
            m_isHit = true;
        }
            
    }
}
