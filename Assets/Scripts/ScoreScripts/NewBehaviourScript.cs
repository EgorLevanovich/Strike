using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NewBehaviourScript : MonoBehaviour
{
    public static NewBehaviourScript Instance;

    public int sessionBalls = 0;
    private int totalBalls = 0;
    private int scoreMultiplier = 1;
    private const string HIGH_SCORE_KEY = "HighScore";
    private const string TOTAL_SCORE_KEY = "TotalScore";
    private const string POINTS_KEY = "Points";
    private const string LEVEL_KEY = "LevelCount";
    private int points = 0; // Количество поинтов за каждые 100 киллов
    private int lastPointsSessionBalls = 0; // Для отслеживания следующей сотни
    public int LevelCount {  get; private set; }
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadTotalScore();
            LoadPoints();
            LoadLevelCount();
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void ResetAllScores()
    {
        // Сбрасываем все переменные
        sessionBalls = 0;
        totalBalls = 0;
        scoreMultiplier = 1;
        points = 0;

        // Удаляем все сохраненные значения
        PlayerPrefs.DeleteKey(HIGH_SCORE_KEY);
        PlayerPrefs.DeleteKey(TOTAL_SCORE_KEY);
        PlayerPrefs.DeleteKey(POINTS_KEY);
        PlayerPrefs.Save();
    }

    private void LoadTotalScore()
    {
        totalBalls = PlayerPrefs.GetInt(TOTAL_SCORE_KEY, 0);
    }

    private void SaveTotalScore()
    {
        PlayerPrefs.SetInt(TOTAL_SCORE_KEY, totalBalls);
        PlayerPrefs.Save();
    }

    private void LoadPoints()
    {
        points = PlayerPrefs.GetInt(POINTS_KEY, 0);
    }

    private void LoadLevelCount()
    {
        LevelCount = PlayerPrefs.GetInt(LEVEL_KEY, 0);
    }

    public void SaveLevelCount()
    {
        LevelCount++;
        PlayerPrefs.SetInt(LEVEL_KEY, LevelCount);
        PlayerPrefs.Save();
        
    }
    private void SavePoints()
    {
        PlayerPrefs.SetInt(POINTS_KEY, points);
        PlayerPrefs.Save();
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "Game")
        {
            sessionBalls = 0;
            scoreMultiplier = 1;
            points = PlayerPrefs.GetInt(POINTS_KEY, 0);
        }
        else if (scene.name == "Menu")
        {
            // Добавляем очки текущей сессии к общему количеству
            if (sessionBalls > 0)  // Добавляем очки только если они были набраны
            {
                totalBalls += sessionBalls;
                SaveTotalScore();
            }

            // Проверяем рекорд
            int currentHighScore = PlayerPrefs.GetInt(HIGH_SCORE_KEY, 0);
            if (sessionBalls > currentHighScore)
            {
                PlayerPrefs.SetInt(HIGH_SCORE_KEY, sessionBalls);
                PlayerPrefs.Save();
            }

            SavePoints(); // Сохраняем поинты при возврате в меню
        }
    }

    public void AddDestroyedBall()
    {
        sessionBalls += scoreMultiplier;
        // Безопасно пытаемся добавить монеты за каждый килл
        try {
            if (Money.Instance != null)
                Money.Instance.AddCoins(1); // 1 монета за каждый килл
        } catch (System.Exception e) {
            Debug.LogWarning($"[AddDestroyedBall] Ошибка начисления монет: {e.Message}");
        }
        // Проверяем, достигли ли следующей сотни
        int newPoints = sessionBalls / 100;
        if (newPoints > points)
        {
            points = newPoints;
            SavePoints();
        }
    }

    public void SetScoreMultiplier(int multiplier)
    {
        scoreMultiplier = multiplier;
    }

    public int GetSessionCount()
    {
        return sessionBalls;
    }

    public int GetTotalCount()
    {
        return totalBalls;
    }

    public void SetTotalCount(int value)
    {
        totalBalls = value;
        SaveTotalScore();
    }

    public int GetPoints()
    {
        return points;
    }

    public int ScoreMultiplier => scoreMultiplier;

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnApplicationQuit()
    {
        SaveTotalScore(); // Сохраняем очки при выходе из игры
        SavePoints(); // Сохраняем поинты при выходе из игры
    }
}
