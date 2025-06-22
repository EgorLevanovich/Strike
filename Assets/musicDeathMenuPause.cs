using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class musicDeathMenuPause : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StopMusic()
    {
        var audio = GetComponent<AudioSource>();
        if (audio != null && audio.isPlaying)
            audio.Stop();
    }

    public void ResumeMusic()
    {
        var audio = GetComponent<AudioSource>();
        if (audio != null && !audio.isPlaying)
            audio.Play();
    }
}
