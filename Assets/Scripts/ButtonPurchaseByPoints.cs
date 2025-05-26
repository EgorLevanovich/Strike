using UnityEngine;
using UnityEngine.UI;

public class ButtonPurchaseByPoints : MonoBehaviour
{
    public Button buyButton;
    public Money money;
    public int price = 10;
    private bool wasBought = false;

    void Start()
    {
        if (money == null)
            money = FindObjectOfType<Money>();
        buyButton.onClick.AddListener(TryBuy);
        UpdateButton();
    }

    void Update()
    {
        UpdateButton();
    }

    void UpdateButton()
    {
        if (money == null)
        {
            buyButton.interactable = false;
            return;
        }
        // Кнопка активна, если хватает поинтов, или если уже куплено
        buyButton.interactable = (money.Coins >= price) || wasBought;
    }

    void TryBuy()
    {
        if (wasBought) return;
        if (money != null && money.Coins >= price)
        {
            money.AddCoins(-price);
            wasBought = true;
        }
    }
} 