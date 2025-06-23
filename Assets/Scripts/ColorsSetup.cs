using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ColorsSetup : MonoBehaviour
{
    public GameObject colors;
   
    public GameObject Settings;
    
    public Button buyButton;
    public Image colorImage;
    public int colorIndex;
    public int price;
    public Text priceText;
    public Image coinImage;
    [Header("Audio")] public AudioSource buySound;

    void Start(){
        UpdateColorButtonState();
        if (buyButton != null)
            buyButton.onClick.AddListener(BuyColor);
    }

    public void ColorsSelect(){
        colors.SetActive(true);
        Settings.SetActive(false);
    }

    public void SettingsSelect(){
        colors.SetActive(false);
        Settings.SetActive(true);
    }

    public void BuyColor()
    {
        // Получаем текущее количество очков за киллы
        int colorPoints = PlayerPrefs.GetInt("PointsPerHundredKills", 0);
        if (colorPoints >= price && PlayerPrefs.GetInt("ColorBought_" + colorIndex, colorIndex == 0 ? 1 : 0) == 0)
        {
            colorPoints -= price;
            PlayerPrefs.SetInt("PointsPerHundredKills", colorPoints);
            PlayerPrefs.SetInt("ColorBought_" + colorIndex, 1);
            PlayerPrefs.Save();
            UpdateColorButtonState();
            if (buySound != null) buySound.Play();
            if (buyButton != null) buyButton.gameObject.SetActive(false);
            Debug.Log($"[ColorBuy] Покупка цвета: {colorIndex}, цена: {price}, осталось очков: {colorPoints}");
            Analytics.Instance.EnemyColorsBought();
        }
        else
        {
            Debug.Log($"[ColorBuy] Недостаточно очков для покупки цвета: {colorIndex}. Доступно: {colorPoints}, требуется: {price}");
        }
    }

    // Метод для обновления состояния цвета (прозрачность и кнопка покупки)
    public void UpdateColorButtonState()
    {
        bool wasBought = PlayerPrefs.GetInt("ColorBought_" + colorIndex, colorIndex == 0 ? 1 : 0) == 1;
        int colorPoints = PlayerPrefs.GetInt("PointsPerHundredKills", 0);
        bool canBuy = colorPoints >= price;
        // Цена и монетка
        if (priceText != null)
        {
            priceText.text = price.ToString();
            priceText.gameObject.SetActive(!wasBought);
            priceText.color = canBuy ? Color.yellow : Color.gray;
        }
        if (coinImage != null)
            coinImage.gameObject.SetActive(!wasBought);
        // Кнопка купить
        if (buyButton != null)
        {
            buyButton.gameObject.SetActive(!wasBought);
            buyButton.interactable = !wasBought && canBuy;
            var btnColor = buyButton.image.color;
            btnColor.a = (!wasBought && canBuy) ? 1f : 0.9f;
            buyButton.image.color = btnColor;
        }
        // Сама кнопка (Button на этом объекте)
        var selfButton = GetComponent<Button>();
        if (selfButton != null)
        {
            selfButton.interactable = wasBought;
        }
    }
}
