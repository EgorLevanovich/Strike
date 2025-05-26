using UnityEngine;
using UnityEngine.SceneManagement;

public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance { get; private set; }

    [SerializeField] private AudioSource loaderMusicSource; // В Loader назначь сюда StrikeTheme
    public AudioSource CurrentMusicSource => loaderMusicSource;

    private const string MUSIC_VOLUME_KEY = "MusicVolume";
    private float volume = 1f;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            volume = PlayerPrefs.GetFloat(MUSIC_VOLUME_KEY, 1f);
            ApplyVolume();
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SetVolume(float newVolume)
    {
        volume = newVolume;
        PlayerPrefs.SetFloat(MUSIC_VOLUME_KEY, volume);
        PlayerPrefs.Save();
        ApplyVolume();
    }

    private void ApplyVolume()
    {
        if (loaderMusicSource != null)
            loaderMusicSource.volume = volume;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (loaderMusicSource == null) return;
        if (scene.name == "Game")
        {
            if (loaderMusicSource.isPlaying)
                loaderMusicSource.Stop();
        }
        else if (scene.name == "Menu")
        {
            if (!loaderMusicSource.isPlaying)
                loaderMusicSource.Play();
        }
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
} 