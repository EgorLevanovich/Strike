using UnityEngine;
using UnityEngine.UI;

public class ButtonActiveByPoints : MonoBehaviour
{
    public Button buyButton;
    public Money money; // Ссылка на твой компонент с очками
    public int price = 10; // Сколько нужно очков для покупки

    void Start()
    {
        if (money == null)
            money = FindObjectOfType<Money>();
    }

    void Update()
    {
        if (money == null)
        {
            buyButton.interactable = false;
            return;
        }
        buyButton.interactable = (money.Coins >= price);
    }
} 