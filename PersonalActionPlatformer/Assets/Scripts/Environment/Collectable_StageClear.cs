using UnityEngine;
using UnityEngine.SceneManagement;

public class Collectable_StageClear : MonoBehaviour
{
    [SerializeField] private string m_nextStage = "Menu";
    [SerializeField] private AudioSource m_audioSource;

    private bool m_starActivated = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (m_starActivated)
            return;

        ClearStage();
    }

    private void ClearStage()
    {
        m_starActivated = true;
        m_audioSource.Play();
    }

    private void Update()
    {
        if (!m_starActivated || m_audioSource.isPlaying)
            return;

        SceneManager.LoadScene(m_nextStage, LoadSceneMode.Single);
    }
}
