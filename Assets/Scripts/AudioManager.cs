using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    private const string MUSIC_VOLUME_KEY = "MusicVolume";
    private float currentMusicVolume = 1f;

    public float CurrentMusicVolume => currentMusicVolume;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadVolume();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void LoadVolume()
    {
        currentMusicVolume = PlayerPrefs.GetFloat(MUSIC_VOLUME_KEY, 1f);
        UpdateAllMusicVolume();
    }

    public void SetMusicVolume(float volume)
    {
        currentMusicVolume = Mathf.Clamp01(volume);
        PlayerPrefs.SetFloat(MUSIC_VOLUME_KEY, currentMusicVolume);
        PlayerPrefs.Save();
        UpdateAllMusicVolume();
    }

    private void UpdateAllMusicVolume()
    {
        // Найти все источники музыки в сцене и обновить их громкость
        var musicSources = FindObjectsOfType<MusicSource>();
        foreach (var source in musicSources)
        {
            source.UpdateVolume();
        }
    }
} 