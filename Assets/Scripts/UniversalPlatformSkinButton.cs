using UnityEngine;
using UnityEngine.UI;


public class UniversalPlatformSkinButton : MonoBehaviour
{
    public int skinIndex;
    public int price;
    public Button buyButton;
    public Button selectButton;
    public GameObject checkmark;
    public Text priceText;
    public Image coinImage;
    public GameObject CoinImage;
    public Image platformSkinImage;
    public SpriteRenderer platformSkinRenderer;
    private bool wasBought;
    private bool isSelected;

    void Start()
    {
        if (buyButton != null)
            buyButton.onClick.AddListener(BuySkin);
        if (selectButton != null)
            selectButton.onClick.AddListener(SelectSkin);

        UpdateButtonState();
    }

    void OnEnable()
    {
        UpdateButtonState();
    }

    void Update()
    {
        if (!wasBought && buyButton != null)
        {
            int points = PlayerPrefs.GetInt("AllTimeKills", 0);
            buyButton.interactable = points >= price;
        }
    }

    void BuySkin()
    {
        var manager = FindObjectOfType<PlatformSkinManager>();
        if (manager != null)
        {
            manager.TryPurchasePlatform(skinIndex);
            if (priceText != null) priceText.gameObject.SetActive(false);
            if (coinImage != null) coinImage.gameObject.SetActive(false);
        }
    }

    void SelectSkin()
    {
        var manager = FindObjectOfType<PlatformSkinManager>();
        if (manager != null)
        {
            manager.SelectPlatform(skinIndex);
        }
    }

    public void UpdateButtonState()
    {
        wasBought = PlayerPrefs.GetInt("PlatformBought_" + skinIndex, skinIndex == 0 ? 1 : 0) == 1;
        isSelected = PlayerPrefs.GetInt("SelectedPlatform", -1) == skinIndex;

        if (!wasBought)
        {
            if (buyButton != null) buyButton.gameObject.SetActive(true);
            if (selectButton != null) selectButton.gameObject.SetActive(false);
            if (checkmark != null) checkmark.SetActive(false);
            int points = PlayerPrefs.GetInt("AllTimeKills", 0);
            if (buyButton != null) buyButton.interactable = points >= price;
            if (priceText != null) priceText.gameObject.SetActive(true);
            if (coinImage != null) coinImage.gameObject.SetActive(true);
            if (platformSkinRenderer != null)
            {
                var color = platformSkinRenderer.color;
                color.a = 0.5f;
                platformSkinRenderer.color = color;
            }
        }
        else
        {
            if (buyButton != null) buyButton.gameObject.SetActive(false);
            if (priceText != null) priceText.gameObject.SetActive(false);
            if (coinImage != null) coinImage.gameObject.SetActive(false);

            if (isSelected)
            {
                if (selectButton != null) selectButton.gameObject.SetActive(false);
                if (checkmark != null) checkmark.SetActive(true);
            }
            else
            {
                if (selectButton != null)
                {
                    selectButton.gameObject.SetActive(true);
                    selectButton.interactable = true;
                }
                if (checkmark != null) checkmark.SetActive(false);
            }
            if (platformSkinRenderer != null)
            {
                var color = platformSkinRenderer.color;
                color.a = 1f;
                platformSkinRenderer.color = color;
            }
        }
    }
} 