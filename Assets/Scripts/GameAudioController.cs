using UnityEngine;
using UnityEngine.Audio;

public class GameAudioController : MonoBehaviour
{
    public AudioSource gameAudioSource;
    private const string GAME_SOUND_VOLUME_KEY = "GameSoundVolume";

    void Start()
    {
        // Получаем сохраненное значение громкости звуков игры
        float savedVolume = PlayerPrefs.GetFloat(GAME_SOUND_VOLUME_KEY, 1f);
        SetVolume(savedVolume);
    }

    public void SetVolume(float volume)
    {
        if (gameAudioSource != null)
        {
            gameAudioSource.volume = volume;
        }
    }
} 