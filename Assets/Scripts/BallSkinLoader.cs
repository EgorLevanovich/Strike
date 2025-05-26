using UnityEngine;

public class BallSkinLoader : MonoBehaviour
{
    public GameObject[] ballPrefabs; // Префабы или объекты мячей на сцене

    void Start()
    {
        LoadSelectedBall();
    }

    public void LoadSelectedBall()
    {
        int selectedBallIndex = PlayerPrefs.GetInt("SelectedBall", -1);

        // Если мяч не выбран или не куплен, деактивируем все
        if (selectedBallIndex == -1 || !IsBallBought(selectedBallIndex))
        {
            foreach (GameObject ball in ballPrefabs)
            {
                if (ball != null)
                    ball.SetActive(false);
            }
            return;
        }

        // Деактивируем все мячи
        foreach (GameObject ball in ballPrefabs)
        {
            if (ball != null)
                ball.SetActive(false);
        }

        // Активируем только выбранный и купленный
        if (selectedBallIndex < ballPrefabs.Length && ballPrefabs[selectedBallIndex] != null)
        {
            ballPrefabs[selectedBallIndex].SetActive(true);
        }
    }

    private bool IsBallBought(int index)
    {
        return PlayerPrefs.GetInt("BallBought_" + index, index == 0 ? 1 : 0) == 1;
    }
} 