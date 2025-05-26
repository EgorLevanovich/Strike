using UnityEngine;
using UnityEngine.UI;

public class CoinsDisplay : MonoBehaviour
{
    public Text coinsText; // Перетащи сюда свой UI Text

    void Update()
    {
        Refresh();
    }

    public void Refresh()
    {
        if (coinsText != null && Money.Instance != null)
        {
            coinsText.text = "" + Money.Instance.Coins;
        }
    }
} 