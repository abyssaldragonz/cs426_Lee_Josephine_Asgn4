using UnityEngine;

public class MusicManager : MonoBehaviour
{
    private static MusicManager instance;
    private AudioSource audioSource;
    
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            audioSource = GetComponent<AudioSource>();
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    // Public method to toggle mute
    public static void ToggleMute()
    {
        if (instance != null && instance.audioSource != null)
        {
            instance.audioSource.mute = !instance.audioSource.mute;
            Debug.Log($"Music muted: {instance.audioSource.mute}");
        }
    }
    
    // Get current mute state
    public static bool IsMuted()
    {
        if (instance != null && instance.audioSource != null)
        {
            return instance.audioSource.mute;
        }
        return false;
    }
}