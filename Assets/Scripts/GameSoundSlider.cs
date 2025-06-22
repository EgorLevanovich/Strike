using UnityEngine;
using UnityEngine.UI;
using System;

public class GameSoundSlider : MonoBehaviour
{
    public Slider gameSoundSlider;
    public AudioSource extraSource1;
    public AudioSource extraSource2;
    public AudioSource extraSource3;
    private const string GAME_SOUND_VOLUME_KEY = "GameSoundVolume"; // Уникальный ключ для звуков игры
    
    // Событие для оповещения об изменении громкости
    public static event Action<float> OnVolumeChanged;

    void Start()
    {
        // Загружаем сохраненное значение громкости звуков игры
        float savedVolume = PlayerPrefs.GetFloat(GAME_SOUND_VOLUME_KEY, 1f);
        gameSoundSlider.value = savedVolume;
        
        // Устанавливаем начальное значение громкости
        SetGameSoundVolume(savedVolume);
        
        // Добавляем слушатель события изменения значения слайдера
        gameSoundSlider.onValueChanged.AddListener(SetGameSoundVolume);
    }

    public void SetGameSoundVolume(float volume)
    {
        // Сохраняем значение громкости звуков игры
        PlayerPrefs.SetFloat(GAME_SOUND_VOLUME_KEY, volume);
        PlayerPrefs.Save();

        // Вызываем событие изменения громкости
        OnVolumeChanged?.Invoke(volume);

        // Находим все объекты с GameAudioController и обновляем их громкость
        GameAudioController[] audioControllers = FindObjectsOfType<GameAudioController>();
        foreach (GameAudioController controller in audioControllers)
        {
            controller.SetVolume(volume);
        }

        // Управляем двумя дополнительными источниками звука
        if (extraSource1 != null)
            extraSource1.volume = volume;
        if (extraSource2 != null)
            extraSource2.volume = volume;
        if (extraSource3 != null)
            extraSource3.volume = volume;
    }

    void OnDestroy()
    {
        // Удаляем слушатель при уничтожении объекта
        if (gameSoundSlider != null)
            gameSoundSlider.onValueChanged.RemoveListener(SetGameSoundVolume);
    }
} 