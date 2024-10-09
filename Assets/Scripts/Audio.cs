using UnityEngine;

public enum AudioClipID 
{
    THUD = 0,
    FLAP = 1
}

public class Audio : MonoBehaviour
{
    private static Audio s_instance;

    private AudioSource m_AudioSource;
    private bool m_isMuted;

    [SerializeField] private AudioClip[] m_clips;

    private void Awake()
    {
        s_instance = this;

        if (s_instance != null) 
        {
            Destroy(s_instance);
        }

        m_isMuted = false;
        m_AudioSource = GetComponent<AudioSource>();

        DontDestroyOnLoad(s_instance);
    }

    private void SetMute(bool value)
    {
        m_isMuted = value;
    }
    public static void Mute(bool value) 
    {
        s_instance.SetMute(value);
    }
    public static void PlayClip(AudioClipID id)
    {
        s_instance.PlayOneShot(id);
    }

    public void PlayOneShot(AudioClipID id)
    {
        if (m_isMuted) return;

        m_AudioSource.PlayOneShot(m_clips[(int)id]);
    }
}
