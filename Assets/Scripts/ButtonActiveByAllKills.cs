using UnityEngine;
using UnityEngine.UI;

public class ButtonActiveByAllKills : MonoBehaviour
{
    public Button buyButton;
    public int price = 10; // Сколько нужно поинтов для активации
    public string buttonKey = "ButtonBought"; // Уникальный ключ для этой кнопки

    private const string POINTS_PER_HUNDRED_KEY = "PointsPerHundredKills";
    private bool wasBought = false;

    void Start()
    {
        wasBought = PlayerPrefs.GetInt(buttonKey, 0) == 1;
        buyButton.onClick.AddListener(TryBuy);
    }

    void Update()
    {
        int colorPoints = PlayerPrefs.GetInt(POINTS_PER_HUNDRED_KEY, 0);
        buyButton.interactable = (colorPoints >= price) || wasBought;
    }

    void TryBuy()
    {
        if (wasBought) return;
        int colorPoints = PlayerPrefs.GetInt(POINTS_PER_HUNDRED_KEY, 0);
        if (colorPoints >= price)
        {
            PlayerPrefs.SetInt(POINTS_PER_HUNDRED_KEY, colorPoints - price);
            PlayerPrefs.SetInt(buttonKey, 1);
            PlayerPrefs.Save();
            wasBought = true;
        }
    }
} 