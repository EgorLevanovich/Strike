using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioClip backgroundMusic;
    [SerializeField] private float fadeTime = 1f;
    [SerializeField][Range(0f, 1f)] private float volume = 0.5f;

    private bool isPaused = false;
    private Button currentPauseButton;
    private Button currentResumeButton;


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeAudioSource();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        PlayMusic();

        // ��������� ����������� ���������
        volume = PlayerPrefs.GetFloat("MusicVolume", 0.5f);
        if (musicSource != null)
        {
            musicSource.volume = volume;
        }
        UpdatePauseButtons();
    }

    private void InitializeAudioSource()
    {
        if (musicSource == null)
        {
            musicSource = gameObject.AddComponent<AudioSource>();
            musicSource.clip = backgroundMusic;
            musicSource.loop = true;
            musicSource.playOnAwake = false;
            musicSource.volume = volume;
        }
    }

    public void SetVolume(float newVolume)
    {
        volume = newVolume;
        if (musicSource != null)
        {
            musicSource.volume = volume;
        }
        PlayerPrefs.SetFloat("MusicVolume", volume);
        PlayerPrefs.Save();
    }

    public void PauseMusic()
    {
        if (musicSource != null && musicSource.isPlaying)
        {
            musicSource.Pause();
            isPaused = true;
            UpdatePauseButtons();
        }
    }

    public void ResumeMusic()
    {
        if (musicSource != null && isPaused)
        {
            musicSource.UnPause();
            isPaused = false;
            UpdatePauseButtons();
        }
    }

    private void UpdatePauseButtons()
    {
        if (currentPauseButton != null)
        {
            currentPauseButton.gameObject.SetActive(!isPaused);
        }

        if (currentResumeButton != null)
        {
            currentResumeButton.gameObject.SetActive(isPaused);
        }
    }

    public void RegisterPauseButton(Button button)
    {
        if (button == null) return;
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(PauseMusic);
        button.gameObject.SetActive(!isPaused);
        currentPauseButton = button;
    }

    public void RegisterResumeButton(Button button)
    {
        if (button == null) return;
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(ResumeMusic);
        button.gameObject.SetActive(isPaused);
        currentResumeButton = button;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        string sceneName = scene.name;

        if (sceneName == "Menu" || sceneName == "Game")
        {
            if (!musicSource.isPlaying && !isPaused)
            {
                PlayMusic();
            }

            // � ���� ������� ������� � ����������� ���
            if (sceneName == "Menu")
            {
                Slider menuSlider = FindObjectOfType<Slider>();
                if (menuSlider != null)
                {
                    menuSlider.value = volume;
                    menuSlider.onValueChanged.AddListener(SetVolume);
                }
            }
        }
        else
        {
            StopMusic();
        }
    }

    private void PlayMusic()
    {
        if (musicSource != null && backgroundMusic != null)
        {
            musicSource.clip = backgroundMusic;
            musicSource.Play();
            isPaused = false;
            UpdatePauseButtons();
        }
    }

    private void StopMusic()
    {
        if (musicSource != null && musicSource.isPlaying)
        {
            musicSource.Stop();
            isPaused = false;
            UpdatePauseButtons();
        }
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}
