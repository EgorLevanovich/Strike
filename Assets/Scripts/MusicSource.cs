using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class MusicSource : MonoBehaviour
{
    private AudioSource audioSource;
    private float baseVolume = 1f;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        baseVolume = audioSource.volume;
        UpdateVolume();
    }

    public void UpdateVolume()
    {
        if (AudioManager.Instance != null && audioSource != null)
        {
            audioSource.volume = baseVolume * AudioManager.Instance.CurrentMusicVolume;
        }
    }

    private void OnEnable()
    {
        UpdateVolume();
    }
} 