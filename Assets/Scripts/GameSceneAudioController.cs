using UnityEngine;
using UnityEngine.Audio;

public class GameSceneAudioController : MonoBehaviour
{
    public AudioSource gameAudioSource;
    private const string GAME_SOUND_VOLUME_KEY = "GameSoundVolume";

    void Start()
    {
        if (gameAudioSource != null)
        {
            // Загружаем сохраненное значение громкости при старте сцены
            float savedVolume = PlayerPrefs.GetFloat(GAME_SOUND_VOLUME_KEY, 1f);
            gameAudioSource.volume = savedVolume;
        }
    }

    void OnEnable()
    {
        // Подписываемся на событие изменения громкости
        GameSoundSlider.OnVolumeChanged += UpdateVolume;
    }

    void OnDisable()
    {
        // Отписываемся от события при отключении объекта
        GameSoundSlider.OnVolumeChanged -= UpdateVolume;
    }

    private void UpdateVolume(float newVolume)
    {
        if (gameAudioSource != null)
        {
            gameAudioSource.volume = newVolume;
        }
    }
} 