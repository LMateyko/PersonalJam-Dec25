using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance { get; private set; }

    [SerializeField] private AudioSource m_musicAudioSource = default;
    private AudioClip m_currentMusic;

    protected virtual void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    protected virtual void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }

    public void PlayMusic(AudioClip newMusic)
    {
        if(m_currentMusic == null || newMusic.name != m_currentMusic.name)
        {
            m_musicAudioSource.clip = newMusic;
            m_musicAudioSource.Play();

            m_currentMusic = newMusic;
        }
    }
}