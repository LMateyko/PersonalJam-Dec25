using UnityEngine;

public class MusicInitializer : MonoBehaviour
{
    [SerializeField] private AudioClip m_levelMusic = default;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        MusicManager.Instance.PlayMusic(m_levelMusic);   
    }
}
