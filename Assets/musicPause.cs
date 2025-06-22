using UnityEngine;

public class musicPause : MonoBehaviour
{
    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void PauseMusic()
    {
        if (audioSource != null && audioSource.isPlaying)
        {
            audioSource.Pause();
            Debug.Log($"[musicPause] Поставлена на паузу музыка на объекте: {gameObject.name}");
        }
    }

    public void UnPauseMusic()
    {
        if (audioSource != null)
        {
            audioSource.UnPause();
            Debug.Log($"[musicPause] Возобновлена музыка на объекте: {gameObject.name}");
        }
    }
}
