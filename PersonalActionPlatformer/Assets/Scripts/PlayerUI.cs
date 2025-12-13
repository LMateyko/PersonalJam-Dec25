using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
    [SerializeField] private Transform m_heartContainer;
    [SerializeField] private Image m_heartImage;
    [SerializeField] private Image m_skullImage;

    private PlayerController m_foundPlayer;
    private List<Image> m_playerHearts = new List<Image>();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        m_foundPlayer = FindFirstObjectByType<PlayerController>(FindObjectsInactive.Exclude);
        m_playerHearts.Add(m_heartImage);

        for (int i = 1; i <= m_foundPlayer.MaxHealth; i++)
        {
            m_playerHearts.Add(Instantiate(m_heartImage, m_heartImage.transform.parent));
        }

        m_foundPlayer.OnPlayerHurt += UpdateForPlayerDamage;
        UpdateForPlayerDamage(m_foundPlayer.MaxHealth);
    }

    private void UpdateForPlayerDamage(int playerHealth)
    {
        m_skullImage.gameObject.SetActive(playerHealth <= 0);

        for (int i = 0; i < m_playerHearts.Count; i++)
        {
            m_playerHearts[i].gameObject.SetActive(i < playerHealth);
        }
    }

    private void OnDestroy()
    {
        m_foundPlayer.OnPlayerHurt -= UpdateForPlayerDamage;
    }
}
