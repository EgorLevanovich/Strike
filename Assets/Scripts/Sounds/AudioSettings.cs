using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Rendering;

public class AudioSettings : MonoBehaviour
{
    public static AudioSettings Instance { get; private set; }

    [Header("Настройки")]
    [SerializeField] private AudioMixerGroup _masterMixerGroup; // Группа микшера для управления громкостью
    private float _currentVolume = 0.75f; // Значение по умолчанию (75%)


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

        Debug.Log("[AudioManager] Awake called");
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            Debug.Log("[AudioManager] Instance created");
        }
        else
        {
            Debug.Log("[AudioManager] Duplicate destroyed");
            Destroy(gameObject);
        }
    }

    // Устанавливает громкость для всех аудио-источников в Maps
    public void SetVolume(float volume)
    {
        _currentVolume = volume;

        // Сохраняем в PlayerPrefs
        PlayerPrefs.SetFloat("MasterVolume", volume);

        // Применяем к микшеру (если используется)
        if (_masterMixerGroup != null)
        {
            _masterMixerGroup.audioMixer.SetFloat("MasterVolume", Mathf.Log10(volume) * 20);
        }

        // Находим объект Maps в игровой сцене и обновляем громкость
        GameObject mapsObject = GameObject.Find("Maps");
        if (mapsObject != null)
        {
            AudioSource[] audioSources = mapsObject.GetComponentsInChildren<AudioSource>(true);
            foreach (AudioSource source in audioSources)
            {
                source.volume = _currentVolume;
            }
        }
    }

    private void LoadVolume()
    {
        _currentVolume = PlayerPrefs.GetFloat("MasterVolume", 0.75f);
        SetVolume(_currentVolume); // Применяем загруженное значение
    }

    public float GetCurrentVolume()
    {
        return _currentVolume;
    }

  

}
