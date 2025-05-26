using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Music : MonoBehaviour
{
    public static Music Instance;

    [SerializeField] private AudioClip menuMusic;
    [SerializeField] private AudioClip gameMusic;
    [SerializeField][Range(0f, 1f)] private float volume = 0.5f;
    [SerializeField] private Slider volumeSlider;

    private AudioSource audioSource;
    private float pauseTime;
    private bool isGameScene;

    private void Awake()
    {
        // Создаем Singleton
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeAudioSource();
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        SceneManager.activeSceneChanged += OnSceneChanged;
    }

    private void Start()
    {
        // Загружаем сохраненную громкость
        volume = PlayerPrefs.GetFloat("MusicVolume", 0.5f);
        if (volumeSlider != null)
        {
            volumeSlider.value = volume;
            volumeSlider.onValueChanged.AddListener(SetVolume);
        }
    }

    private void InitializeAudioSource()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.loop = true;
        audioSource.volume = volume;
    }

    public void SetVolume(float newVolume)
    {
        volume = newVolume;
        audioSource.volume = volume;
        PlayerPrefs.SetFloat("MusicVolume", volume);
        PlayerPrefs.Save();
    }

    private void OnSceneChanged(Scene previousScene, Scene newScene)
    {
        // Проверяем тип сцены (игровая или меню)
        isGameScene = newScene.name != "Menu";

        if (isGameScene)
        {
            PauseMusic();
        }
        else
        {
            PlayMenuMusic();
        }

        // Обновляем слайдер громкости
        volumeSlider = FindObjectOfType<Slider>();
        if (volumeSlider != null)
        {
            volumeSlider.value = volume;
            volumeSlider.onValueChanged.AddListener(SetVolume);
        }
    }

    public void PlayMenuMusic()
    {
        if (audioSource.clip != menuMusic)
        {
            audioSource.clip = menuMusic;
            audioSource.Play();
        }
        else if (!audioSource.isPlaying)
        {
            audioSource.time = pauseTime; // Восстанавливаем позицию
            audioSource.Play();
        }
    }

    public void PauseMusic()
    {
        if (audioSource.isPlaying)
        {
            pauseTime = audioSource.time;
            audioSource.Stop(); // Используем Stop вместо Pause
        }
    }

    public void PlayGameMusic()
    {
        audioSource.clip = gameMusic;
        audioSource.Play();
    }

    private void OnDestroy()
    {
        SceneManager.activeSceneChanged -= OnSceneChanged;
    }
}
