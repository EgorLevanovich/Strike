using UnityEngine;
using UnityEngine.UI;

public class ButtonBuyOnce : MonoBehaviour
{
    public Button buyButton;
    public int price = 10; // Сколько нужно денег для покупки
    public int mapIndex = -1; // Индекс карты, если эта кнопка отвечает за покупку карты

    private bool wasBought = false;
    private NewBehaviourScript scoring;

    void Start()
    {
        if (buyButton == null)
        {
            Debug.LogError("Buy button is not assigned in ButtonBuyOnce!");
            return;
        }

        scoring = NewBehaviourScript.Instance;
        if (scoring == null)
        {
            Debug.LogError("Scoring system not found!");
            return;
        }

        // Проверяем, была ли карта уже куплена ранее
        if (mapIndex >= 0)
        {
            wasBought = PlayerPrefs.GetInt("MapBought_" + mapIndex, 0) == 1;
            if (wasBought)
            {
                // Вместо скрытия кнопки, делаем её неактивной и меняем текст
                buyButton.interactable = false;
                Text buttonText = buyButton.GetComponentInChildren<Text>();
                if (buttonText != null)
                {
                    buttonText.text = "Куплено";
                }
                return;
            }
        }

        buyButton.onClick.AddListener(TryBuy);
        UpdateButton();
    }

    void Update()
    {
        UpdateButton();
    }

    void UpdateButton()
    {
        if (buyButton == null || scoring == null) return;
        
        // Кнопка активна, если хватает денег или уже куплено
        buyButton.interactable = (scoring.GetTotalCount() >= price) || wasBought;
    }

    void TryBuy()
    {
        if (wasBought || scoring == null) return;
        
        int totalKills = scoring.GetTotalCount();
        if (totalKills >= price)
        {
            // Списываем очки
            scoring.SetTotalCount(totalKills - price);
            wasBought = true;

            // Если это покупка карты — сохраняем флаг покупки
            if (mapIndex >= 0)
            {
                PlayerPrefs.SetInt("MapBought_" + mapIndex, 1);
                PlayerPrefs.Save();
                Debug.Log("Карта куплена: " + mapIndex);
                
                // Вместо скрытия кнопки, делаем её неактивной и меняем текст
                buyButton.interactable = false;
                Text buttonText = buyButton.GetComponentInChildren<Text>();
                if (buttonText != null)
                {
                    buttonText.text = "Куплено";
                }

                // Обновляем состояние в MapSkinManager, если он есть
                var mapSkinManager = FindObjectOfType<MapSkinManager>();
                if (mapSkinManager != null && mapSkinManager.maps != null && mapIndex < mapSkinManager.maps.Count)
                {
                    mapSkinManager.maps[mapIndex].isPurchased = true;
                    mapSkinManager.SavePurchasedMaps();
                    mapSkinManager.UpdateAllDisplays();
                }
            }
        }
        else
        {
            Debug.Log("Недостаточно очков для покупки!");
        }
    }
} 