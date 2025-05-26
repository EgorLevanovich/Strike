using UnityEngine;

public class SoundsVolumeApplier : MonoBehaviour
{
    private const string SFX_VOLUME_KEY = "SFXVolume";

    void Start()
    {
        float volume = PlayerPrefs.GetFloat(SFX_VOLUME_KEY, 1f);
        AudioSource[] sources = GetComponentsInChildren<AudioSource>();
        foreach (var src in sources)
        {
            src.volume = volume;
        }
    }
} 