using UnityEngine;
using UnityEngine.UI;

public class MenuMusicSlider : MonoBehaviour
{
    public Slider volumeSlider;

    void Start()
    {
        float savedVolume = PlayerPrefs.GetFloat("MusicVolume", 1f);
        if (volumeSlider != null)
        {
            volumeSlider.value = savedVolume;
            volumeSlider.onValueChanged.AddListener(OnVolumeChanged);
        }
        ApplyVolume(savedVolume);
    }

    void OnVolumeChanged(float value)
    {
        ApplyVolume(value);
        PlayerPrefs.SetFloat("MusicVolume", value);
        PlayerPrefs.Save();
    }

    void ApplyVolume(float value)
    {
        if (MusicManager.Instance != null && MusicManager.Instance.CurrentMusicSource != null)
            MusicManager.Instance.SetVolume(value);

        // Меняем громкость у всех AudioSource, где есть TargetMusic
        var targets = FindObjectsOfType<TargetMusic>();
        foreach (var target in targets)
        {
            var src = target.GetComponent<AudioSource>();
            if (src != null)
                src.volume = value;
        }
    }
} 