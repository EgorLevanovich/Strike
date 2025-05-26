using UnityEngine;
using UnityEngine.UI;

public class KillCountDisplay : MonoBehaviour
{
    [SerializeField] private Text killCountText; // Перетащи сюда свой UI Text
    [SerializeField] private Text deathMenuText; // Для экрана смерти
    [SerializeField] private Text pointsPerHundredText; // Текст для отображения поинтов за 100 убийств

    void Start()
    {
        // Устанавливаем центрирование текста при старте
        if (killCountText != null)
        {
            killCountText.alignment = TextAnchor.MiddleCenter;
            killCountText.rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
            killCountText.rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
            killCountText.rectTransform.pivot = new Vector2(0.5f, 0.5f);
        }
        if (deathMenuText != null)
        {
            deathMenuText.alignment = TextAnchor.MiddleCenter;
            deathMenuText.rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
            deathMenuText.rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
            deathMenuText.rectTransform.pivot = new Vector2(0.5f, 0.5f);
        }
        if (pointsPerHundredText != null)
        {
            pointsPerHundredText.alignment = TextAnchor.MiddleCenter;
            pointsPerHundredText.rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
            pointsPerHundredText.rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
            pointsPerHundredText.rectTransform.pivot = new Vector2(0.5f, 0.5f);
        }
    }

    void Update()
    {
        Refresh();
    }

    public void Refresh()
    {
        if (killCountText != null)
        {
            killCountText.text = "" + EnemyPointsGiver.GetTotalKills();
        }
        if (deathMenuText != null)
        {
            deathMenuText.text = "" + EnemyPointsGiver.GetTotalKills();
        }

        if (pointsPerHundredText != null)
        {
            int totalKills = EnemyPointsGiver.GetTotalKills();
            int pointsPerHundred = totalKills / 1;
            pointsPerHundredText.text = "" + pointsPerHundred;
        }
    }
} 