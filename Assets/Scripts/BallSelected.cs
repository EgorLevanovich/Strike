using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallSelected : MonoBehaviour
{
    public GameObject[] ballCharacters; // Массив префабов мячей
    private const string SELECTED_BALL_KEY = "SelectedBall";
    private const string BALL_BOUGHT_KEY = "BallBought_";

    void Start()
    {
        LoadSelectedBall();
    }

    public void LoadSelectedBall()
    {
        int selectedBallIndex = PlayerPrefs.GetInt(SELECTED_BALL_KEY, 0);

        // Проверяем, что массив префабов не пустой
        if (ballCharacters == null || ballCharacters.Length == 0)
        {
            return;
        }

        // Деактивируем все мячи
        for (int i = 0; i < ballCharacters.Length; i++)
        {
            if (ballCharacters[i] != null)
            {
                ballCharacters[i].SetActive(false);
            }
        }

        // Активируем только выбранный и купленный
        if (selectedBallIndex < ballCharacters.Length && ballCharacters[selectedBallIndex] != null)
        {
            if (IsBallBought(selectedBallIndex))
            {
                ballCharacters[selectedBallIndex].SetActive(true);
            }
            else
            {
                // Если выбранный мяч не куплен, активируем первый мяч
                ballCharacters[0].SetActive(true);
                PlayerPrefs.SetInt(SELECTED_BALL_KEY, 0);
                PlayerPrefs.Save();
            }
        }
    }

    private bool IsBallBought(int index)
    {
        bool isBought = PlayerPrefs.GetInt(BALL_BOUGHT_KEY + index, index == 0 ? 1 : 0) == 1;
        return isBought;
    }

    public void SelectBall(int index)
    {
        if (index >= 0 && index < ballCharacters.Length && IsBallBought(index))
        {
            PlayerPrefs.SetInt(SELECTED_BALL_KEY, index);
            PlayerPrefs.Save();
            LoadSelectedBall();
        }
    }
}
 