using UnityEngine;
using UnityEngine.UI;

public class GameSoundManager : MonoBehaviour
{
    public Slider gameSoundSlider;
    private const string GAME_VOLUME_KEY = "GameVolume";

    void Start()
    {
        // Загружаем сохраненное значение громкости
        float savedVolume = PlayerPrefs.GetFloat(GAME_VOLUME_KEY, 1f);
        gameSoundSlider.value = savedVolume;
        
        // Устанавливаем начальное значение громкости
        SetGameVolume(savedVolume);
        
        // Добавляем слушатель события изменения значения слайдера
        gameSoundSlider.onValueChanged.AddListener(SetGameVolume);
    }

    public void SetGameVolume(float volume)
    {
        // Сохраняем значение громкости
        PlayerPrefs.SetFloat(GAME_VOLUME_KEY, volume);
        PlayerPrefs.Save();

        // Находим все объекты с GameAudioController и обновляем их громкость
        GameAudioController[] audioControllers = FindObjectsOfType<GameAudioController>();
        foreach (GameAudioController controller in audioControllers)
        {
            controller.SetVolume(volume);
        }
    }

    void OnDestroy()
    {
        // Удаляем слушатель при уничтожении объекта
        if (gameSoundSlider != null)
            gameSoundSlider.onValueChanged.RemoveListener(SetGameVolume);
    }
} 