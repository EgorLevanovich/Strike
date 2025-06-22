using UnityEngine;

public class MusicVolumeApplier : MonoBehaviour
{
    void Start()
    {
        float volume = PlayerPrefs.GetFloat("MusicVolume", 1f);
        var targets = FindObjectsOfType<TargetMusic>();
        foreach (var target in targets)
        {
            var src = target.GetComponent<AudioSource>();
            if (src != null)
                src.volume = volume;
        }
    }
}