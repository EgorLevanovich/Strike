using UnityEngine;
using UnityEngine.UI;

public class MenuSoundSetup : MonoBehaviour
{
    public Slider soundSlider;
    private const string VolumeKey = "GameVolume";

    void Start()
    {
        if (soundSlider == null)
        {
            Debug.LogError("[MenuSoundSetup] Sound volume slider not assigned!");
            return;
        }
        float savedVolume = PlayerPrefs.GetFloat(VolumeKey, 1f);
        soundSlider.value = savedVolume;
        soundSlider.onValueChanged.AddListener(OnSliderChanged);
        AudioListener.volume = savedVolume;
    }

    void OnSliderChanged(float value)
    {
        AudioListener.volume = value;
        PlayerPrefs.SetFloat(VolumeKey, value);
        PlayerPrefs.Save();
    }
} 