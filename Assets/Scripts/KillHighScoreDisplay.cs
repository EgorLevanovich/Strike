using UnityEngine;
using UnityEngine.UI;

public class KillHighScoreDisplay : MonoBehaviour
{
    public Text highScoreText;
    private const string HIGHSCORE_KEY = "KillHighScore";

    void Update()
    {
        int highScore = PlayerPrefs.GetInt(HIGHSCORE_KEY, 0);
        if (highScoreText != null)
            highScoreText.text = "" + highScore;
    }
} 