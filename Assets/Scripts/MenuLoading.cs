using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuLoading : MonoBehaviour
{
    public Text highScoreText;
    public GameObject _buttons;
    public GameObject _PlayerShop;
    public GameObject _platformskins;
    public GameObject _backgroundskins;
    public GameObject _BallSkins;
    public GameObject _settings;

    private const string HIGH_SCORE_KEY = "HighScore";

    void Start()
    {
        UpdateHighScoreDisplay();
    }

    private void UpdateHighScoreDisplay()
    {
        if (highScoreText != null)
        {
            int highScore = PlayerPrefs.GetInt(HIGH_SCORE_KEY, 0);
            highScoreText.text = "" + highScore.ToString();
        }
    }

    public void ResetAllScores()
    {
        if (NewBehaviourScript.Instance != null)
        {
            NewBehaviourScript.Instance.ResetAllScores();
            UpdateHighScoreDisplay();
        }
    }

    public void GoToMenu()
    {
        if (NewBehaviourScript.Instance != null)
        {
            int currentScore = NewBehaviourScript.Instance.GetSessionCount();
            int savedHighScore = PlayerPrefs.GetInt(HIGH_SCORE_KEY, 0);
            
            if (currentScore > savedHighScore)
            {
                PlayerPrefs.SetInt(HIGH_SCORE_KEY, currentScore);
                PlayerPrefs.Save();
                UpdateHighScoreDisplay();
            }
        }
    }

    public void GameLoader()
    {
        SceneManager.LoadScene("Game");
        if (NewBehaviourScript.Instance != null)
        {
            NewBehaviourScript.Instance.sessionBalls = 0;
        }
        
        
        Time.timeScale = 1.0f;
    }

    public void MenuLoadingScene()
    {
        SceneManager.LoadScene("Menu");
        Time.timeScale = 1.0f;
        var loader = FindObjectOfType<BackGroundLoader>();
        if (loader != null) loader.LoadSelectedBackground();
    }

    public void Exit()
    {
        _PlayerShop.SetActive(false);
        _buttons.SetActive(true);
        _platformskins.SetActive(false);
        _backgroundskins.SetActive(false);
        _BallSkins.SetActive(false);
        var loader = FindObjectOfType<BackGroundLoader>();
        if (loader != null) loader.LoadSelectedBackground();
    }

    public void ShopLoader()
    {
        _buttons.SetActive(false);
        _PlayerShop.SetActive(true);
        _platformskins.SetActive(true);
    }

    public void PlatformSkinsLoader()
    {
        _platformskins.SetActive(true);
        _backgroundskins.SetActive(false);
        _BallSkins.SetActive(false);
    }

    public void BackGroundSkinsLoader()
    {
        _platformskins.SetActive(false);
        _backgroundskins.SetActive(true);
        _BallSkins.SetActive(false);
        FindObjectOfType<MapSkinManager>()?.UpdateAllDisplays();
    }

    public void BallLoading()
    {
        _platformskins.SetActive(false);
        _backgroundskins.SetActive(false);
        _BallSkins.SetActive(true);
    }

    public void SettingsLoader()
    {
        _buttons.SetActive(false );
        _settings.SetActive(true);
    }
    
    public void SettingsExit()
    {
        _settings.SetActive(false);
        _buttons.SetActive(true);
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
