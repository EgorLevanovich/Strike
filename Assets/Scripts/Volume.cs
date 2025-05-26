using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class Volume : MonoBehaviour
{
    [SerializeField]
    private Slider volumeSlider;
    public AudioSource volumeSource;


    public void Start()
    {
        volumeSource = GetComponent<AudioSource>();
    }

    public void Update() 
    {
        if (volumeSlider.value != volumeSource.volume)
        {
            volumeSource.volume = volumeSlider.value;
        }
    }
}
