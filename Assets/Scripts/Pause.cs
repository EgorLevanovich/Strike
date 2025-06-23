using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Pause : MonoBehaviour
{
    public GameObject pauseMenu;
    public GameObject _settings;
    public GameObject _pauseButton;
    private bool isPaused = false;
    private CanvasGroup pauseButtonCanvasGroup;
    private bool ignoreNextEscape = false;

    void Awake()
    {
        if (_pauseButton != null)
            pauseButtonCanvasGroup = _pauseButton.GetComponent<CanvasGroup>();
    }

    private void Update()
    {
        if (ignoreNextEscape)
        {
            if (Input.GetKeyUp(KeyCode.Escape))
                ignoreNextEscape = false;
            return;
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
    }

    public void TogglePause()
    {
        isPaused = !isPaused;
        pauseMenu.SetActive(isPaused);
        Time.timeScale = isPaused ? 0 : 1;
        if (pauseButtonCanvasGroup != null)
        {
            if (isPaused)
            {
                pauseButtonCanvasGroup.alpha = 0f;
                pauseButtonCanvasGroup.interactable = false;
                pauseButtonCanvasGroup.blocksRaycasts = false;
            }
            else
            {
                pauseButtonCanvasGroup.alpha = 1f;
                pauseButtonCanvasGroup.interactable = true;
                pauseButtonCanvasGroup.blocksRaycasts = true;
            }
        }
    }

    public void ResetGame()
    {
        SceneManager.LoadScene("Game");
        Analytics.Instance.LevelFinish("restart");
        Time.timeScale = 1;
        if (pauseButtonCanvasGroup != null)
        {
            pauseButtonCanvasGroup.alpha = 1f;
            pauseButtonCanvasGroup.interactable = true;
            pauseButtonCanvasGroup.blocksRaycasts = true;
        }
    }

    public void Exit()
    {
        var health = Object.FindObjectsByType<HealthSystem>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        
        var result = health.Any(any=> any.IsDead) ? "lose" : "closed";
        Analytics.Instance.LevelFinish(result);
        
        SceneManager.LoadScene("Menu");
        Time.timeScale = 1;
    }

    public void Play()
    {
        Time.timeScale = 1;
        pauseMenu.SetActive(false);
        isPaused = false;
        ignoreNextEscape = true;
        if (pauseButtonCanvasGroup != null)
        {
            pauseButtonCanvasGroup.alpha = 1f;
            pauseButtonCanvasGroup.interactable = true;
            pauseButtonCanvasGroup.blocksRaycasts = true;
        }
    }

    public void Settings()
    {
        _settings.SetActive(true);

    }
}
