using UnityEngine;
using UnityEngine.UI;

public class MenuScoring : MonoBehaviour
{
    public static MenuScoring Instance;
    public Text totalText;
    public Text pointsText;
    private const string TOTAL_SCORE_KEY = "TotalScore";

    private void Start()
    {
        UpdateTotalScore();
    }

    private void Update()
    {
        UpdateTotalScore();
    }

    private void UpdateTotalScore()
    {
        if (totalText != null)
        {
            if (NewBehaviourScript.Instance != null)
            {
                totalText.text = "" + NewBehaviourScript.Instance.GetTotalCount();
                if (pointsText != null)
                {
                    pointsText.text = "" + NewBehaviourScript.Instance.GetPoints();
                }
            }
            else
            {
                // Если Instance еще не создан, берем значение напрямую из PlayerPrefs
                totalText.text = "" + PlayerPrefs.GetInt(TOTAL_SCORE_KEY, 0);
                if (pointsText != null)
                {
                    pointsText.text = "" + PlayerPrefs.GetInt("Points", 0);
                }
            }
        }
    }
}