using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Slider))]
public class VolumeSlider : MonoBehaviour
{
    private Slider slider;
    private const string MUSIC_VOLUME_KEY = "MusicVolume";

    private void Awake()
    {
        slider = GetComponent<Slider>();
        slider.value = PlayerPrefs.GetFloat(MUSIC_VOLUME_KEY, 1f);
        slider.onValueChanged.AddListener(OnVolumeChanged);
    }

    private void OnEnable()
    {
        slider.value = PlayerPrefs.GetFloat(MUSIC_VOLUME_KEY, 1f);
    }

    private void OnVolumeChanged(float volume)
    {
        PlayerPrefs.SetFloat(MUSIC_VOLUME_KEY, volume);
        PlayerPrefs.Save();
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.SetMusicVolume(volume);
        }
    }

    private void OnDestroy()
    {
        slider.onValueChanged.RemoveListener(OnVolumeChanged);
    }
}