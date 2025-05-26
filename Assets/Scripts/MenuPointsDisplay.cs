using UnityEngine;
using UnityEngine.UI;

public class MenuPointsDisplay : MonoBehaviour
{
    public Text pointsPerHundredText; // Текст для отображения поинтов за 100 убийств

    void Update()
    {
        if (pointsPerHundredText != null)
        {
            int pointsPerHundred = PlayerPrefs.GetInt("PointsPerHundredKills", 0);
            pointsPerHundredText.text = "" + pointsPerHundred;
        }
    }
} 