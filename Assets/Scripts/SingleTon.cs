using UnityEngine;
using UnityEngine.Audio;

public class SingleTon : MonoBehaviour
{
    public static SingleTon Instance;

    public AudioMixer audioMixer; // Подключи свой AudioMixer, если используешь
    private float _musicVolume = 1f;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public float MusicVolume
    {
        get => _musicVolume;
        set
        {
            _musicVolume = Mathf.Clamp01(value);
            // Если используешь AudioMixer
            // audioMixer.SetFloat("MusicVolume", Mathf.Log10(_musicVolume) * 20);

            // Сохраняем значение громкости в PlayerPrefs
            PlayerPrefs.SetFloat("MusicVolume", _musicVolume);
        }
    }

    private void Start()
    {
        // Загружаем сохраненное значение громкости
        _musicVolume = PlayerPrefs.GetFloat("MusicVolume", 0.75f);
    }
}