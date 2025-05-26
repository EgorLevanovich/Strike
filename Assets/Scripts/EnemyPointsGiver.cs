using UnityEngine;
using UnityEngine.SceneManagement;

public class EnemyPointsGiver : MonoBehaviour
{
    public int pointsForKill = 1; // Сколько поинтов даётся за убийство
    public static int totalKills = 0; // Общее количество убитых врагов
    private const string KILLS_KEY = "TotalKills";
    private const string TOTAL_KILLS_KEY = "AllTimeKills";
    private const string HIGHSCORE_KEY = "KillHighScore";

    public AudioSource deathAudio;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void OnSceneLoaded()
    {
        SceneManager.sceneLoaded += (scene, mode) =>
        {
            if (scene.name == "Game")
            {
                totalKills = 0;
                PlayerPrefs.SetInt(KILLS_KEY, 0);
                PlayerPrefs.Save();
                // Обновляем UI после сброса
                var killDisplay = Object.FindObjectOfType<KillCountDisplay>();
                if (killDisplay != null) killDisplay.Refresh();
            }
            else if (scene.name == "Menu")
            {
                // Сохраняем рекорд, если побит
                int prevHighScore = PlayerPrefs.GetInt(HIGHSCORE_KEY, 0);
                if (totalKills > prevHighScore)
                {
                    PlayerPrefs.SetInt(HIGHSCORE_KEY, totalKills);
                    PlayerPrefs.Save();
                }
                totalKills = PlayerPrefs.GetInt(KILLS_KEY, 0);
                var killDisplay = Object.FindObjectOfType<KillCountDisplay>();
                if (killDisplay != null) killDisplay.Refresh();
            }
        };
    }

    // Вызывай этот метод, когда враг должен быть уничтожен
    public void KillEnemy()
    {
        if (Money.Instance != null)
            Money.Instance.AddCoins(pointsForKill);

        // Получаем множитель очков (для Double Points)
        int multiplier = 1;
        if (NewBehaviourScript.Instance != null)
            multiplier = NewBehaviourScript.Instance.ScoreMultiplier;

        Debug.Log($"Kill! Multiplier: {multiplier}");

        totalKills += multiplier;
        PlayerPrefs.SetInt(KILLS_KEY, totalKills);
        // Увеличиваем общий счётчик убийств за все сессии
        int allTimeKills = PlayerPrefs.GetInt(TOTAL_KILLS_KEY, 0) + multiplier;
        PlayerPrefs.SetInt(TOTAL_KILLS_KEY, allTimeKills);
        
        // Увеличиваем очки за 10 киллов только если достигли десятка
        if (allTimeKills % 10 == 0)
        {
            int pointsPerHundred = PlayerPrefs.GetInt("PointsPerHundredKills", 0) + 1;
            PlayerPrefs.SetInt("PointsPerHundredKills", pointsPerHundred);
        }
        
        PlayerPrefs.Save();
        // Обновляем текст с монетами, если есть CoinsDisplay на сцене
        CoinsDisplay display = FindObjectOfType<CoinsDisplay>();
        if (display != null)
            display.Refresh();
        // Обновляем текст с количеством убийств, если есть KillCountDisplay на сцене
        KillCountDisplay killDisplay = FindObjectOfType<KillCountDisplay>();
        if (killDisplay != null)
            killDisplay.Refresh();

        if (deathAudio != null)
            deathAudio.Play();

        Destroy(gameObject);
    }

    // Уничтожаем врага и начисляем поинты при столкновении с Ball
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ball"))
        {
            KillEnemy();
        }

        if (collision.gameObject.CompareTag("Ground"))
        {
            KillEnemy();
        }
    }

    // Публичный метод для получения количества убийств
    public static int GetTotalKills()
    {
        return totalKills;
    }

    // Метод для вычитания очков при покупке
    public static void SubtractPoints(int amount)
    {
        totalKills -= amount;
        PlayerPrefs.SetInt(KILLS_KEY, totalKills);
        PlayerPrefs.Save();
        
        // Обновляем отображение
        KillCountDisplay killDisplay = FindObjectOfType<KillCountDisplay>();
        if (killDisplay != null)
            killDisplay.Refresh();
    }
} 