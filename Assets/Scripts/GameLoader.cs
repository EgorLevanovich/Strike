using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadSceneAfterDelay : MonoBehaviour
{
    [Tooltip("��� ����� ��� ��������")]
    public string sceneName = "MainMenu"; // ������� ��� ����� �����

    [Tooltip("�������� � ��������")]
    public float delay = 10f; // 10 ������ ��������

    void Start()
    {
        // ��������� ��������
        Invoke("LoadTargetScene", delay);
    }

    void LoadTargetScene()
    {
        // ��������� ���������� �� �����
        if (SceneUtility.GetBuildIndexByScenePath(sceneName) >= 0)
        {
            SceneManager.LoadScene(sceneName);
        }
        else
        {
            // :     
            int nextSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;
            if (nextSceneIndex < SceneManager.sceneCountInBuildSettings)
            {
                SceneManager.LoadScene(nextSceneIndex);
            }
        }
    }
}