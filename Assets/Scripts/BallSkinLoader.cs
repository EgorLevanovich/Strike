using UnityEngine;

public class BallSkinLoader : MonoBehaviour
{
    public GameObject[] ballPrefabs; // Префабы или объекты мячей на сцене
    private const string SELECTED_BALL_KEY = "SelectedBall";
    private const string BALL_BOUGHT_KEY = "BallBought_";

    void Start()
    {
        LoadSelectedBall();
    }

    public void LoadSelectedBall()
    {
        int selectedBallIndex = PlayerPrefs.GetInt(SELECTED_BALL_KEY, -1);

        // Проверяем, что массив префабов не пустой
        if (ballPrefabs == null || ballPrefabs.Length == 0)
        {
            return;
        }

        // Если мяч не выбран или не куплен, используем первый мяч
        if (selectedBallIndex == -1 || !IsBallBought(selectedBallIndex))
        {
            selectedBallIndex = 0;
            PlayerPrefs.SetInt(SELECTED_BALL_KEY, selectedBallIndex);
            PlayerPrefs.Save();
        }

        // Деактивируем все мячи
        for (int i = 0; i < ballPrefabs.Length; i++)
        {
            if (ballPrefabs[i] != null)
            {
                ballPrefabs[i].SetActive(false);
            }
        }

        // Активируем только выбранный и купленный
        if (selectedBallIndex < ballPrefabs.Length && ballPrefabs[selectedBallIndex] != null)
        {
            ballPrefabs[selectedBallIndex].SetActive(true);
        }
    }

    private bool IsBallBought(int index)
    {
        bool isBought = PlayerPrefs.GetInt(BALL_BOUGHT_KEY + index, index == 0 ? 1 : 0) == 1;
        return isBought;
    }
} 