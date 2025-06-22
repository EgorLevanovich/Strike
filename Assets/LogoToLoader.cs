using UnityEngine;
using UnityEngine.SceneManagement;

public class LogoToLoader : MonoBehaviour
{
    public float delay = 3f; // Задержка в секундах

    void Start()
    {
        Invoke("LoadLoaderScene", delay);
    }

    void LoadLoaderScene()
    {
        SceneManager.LoadScene("Loader");
    }
} 