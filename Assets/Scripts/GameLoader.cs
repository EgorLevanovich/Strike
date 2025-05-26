using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadSceneAfterDelay : MonoBehaviour
{
    [Tooltip("Имя сцены для загрузки")]
    public string sceneName = "MainMenu"; // Укажите имя вашей сцены

    [Tooltip("Задержка в секундах")]
    public float delay = 10f; // 10 секунд задержки

    void Start()
    {
        // Запускаем задержку
        Invoke("LoadTargetScene", delay);
    }

    void LoadTargetScene()
    {
        // Проверяем существует ли сцена
        if (SceneUtility.GetBuildIndexByScenePath(sceneName) >= 0)
        {
            SceneManager.LoadScene(sceneName);
        }
        else
        {
            Debug.LogError($"Сцена '{sceneName}' не найдена!");

            // Альтернатива: загрузить следующую сцену по индексу
            int nextSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;
            if (nextSceneIndex < SceneManager.sceneCountInBuildSettings)
            {
                SceneManager.LoadScene(nextSceneIndex);
            }
        }
    }
}