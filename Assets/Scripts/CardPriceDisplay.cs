using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CardPriceDisplay : MonoBehaviour
{
    [Header("UI Elements")]
    public TextMeshProUGUI priceText;
    public Image coinIcon;
    public CanvasGroup canvasGroup;
    public RectTransform priceContainer;
    
    [Header("Style Settings")]
    public Color affordableColor = new Color(1f, 0.92f, 0.016f, 1f); // Золотой цвет
    public Color unaffordableColor = new Color(0.5f, 0.5f, 0.5f, 1f); // Серый цвет
    
    private int currentPrice;
    private bool isAffordable;

    private void Awake()
    {
        if (canvasGroup == null)
            canvasGroup = GetComponent<CanvasGroup>();
            
        if (priceContainer == null)
            priceContainer = GetComponent<RectTransform>();
    }

    public void SetPrice(int price)
    {
        currentPrice = price;
        UpdatePriceDisplay();
    }

    public void UpdatePriceDisplay()
    {
        if (priceText == null) return;

        int playerPoints = PlayerPrefs.GetInt("AllTimeKills", 0);
        isAffordable = playerPoints >= currentPrice;

        // Обновляем текст цены
        priceText.text = currentPrice.ToString();
        
        // Обновляем цвет в зависимости от доступности
        Color targetColor = isAffordable ? affordableColor : unaffordableColor;
        priceText.color = targetColor;
        if (coinIcon != null)
            coinIcon.color = targetColor;

        // Просто показываем (без анимации)
        if (canvasGroup != null)
            canvasGroup.alpha = 1f;
        if (priceContainer != null)
            priceContainer.localScale = Vector3.one;
    }

    // Можно вызывать эти методы из EventTrigger для эффекта наведения
    public void OnHoverEnter()
    {
        if (isAffordable && priceContainer != null)
            priceContainer.localScale = Vector3.one * 1.1f;
    }

    public void OnHoverExit()
    {
        if (priceContainer != null)
            priceContainer.localScale = Vector3.one;
    }
} 