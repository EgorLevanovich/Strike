using Assets.Scripts.Gameplay;
using UnityEngine;
using UnityEngine.UI;

public class UniversalPlatformSkinButton : MonoBehaviour
{
    [SerializeField] private string _name;
    public int skinIndex;
    public int price;
    public Button buyButton;
    public Button selectButton;
    public GameObject checkmark;
    public Image platformPreviewImage;
    public SpriteRenderer platformPreviewRenderer;
    public AudioSource purchaseSound;
    public AudioSource selectSound;

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
        int points = PlayerPrefs.GetInt("AllTimeKills", 0);
        if (points >= price)
        {
            points -= price;
            PlayerPrefs.SetInt("AllTimeKills", points);
            PlayerPrefs.SetInt("PlatformBought_" + skinIndex, 1);
            PlayerPrefs.Save();
            
            if (purchaseSound != null)
                purchaseSound.Play();
            
            wasBought = true;
            UpdateButtonState();
            
            foreach (var button in FindObjectsOfType<UniversalPlatformSkinButton>())
            {
                button.wasBought = PlayerPrefs.GetInt("PlatformBought_" + button.skinIndex, button.skinIndex == 0 ? 1 : 0) == 1;
                button.isSelected = PlayerPrefs.GetInt("SelectedPlatform", -1) == button.skinIndex;
                button.UpdateButtonState();
            }
        }
    }

    void SelectSkin()
    {
        if (!wasBought) return;

        if (selectSound != null) selectSound.Play();

        PlayerPrefs.SetInt("SelectedPlatform", skinIndex);
        PlayerPrefs.Save();
        
        isSelected = true;
        UpdateButtonState();
        
        foreach (var button in FindObjectsOfType<UniversalPlatformSkinButton>())
        {
            button.wasBought = PlayerPrefs.GetInt("PlatformBought_" + button.skinIndex, button.skinIndex == 0 ? 1 : 0) == 1;
            button.isSelected = PlayerPrefs.GetInt("SelectedPlatform", -1) == button.skinIndex;
            button.UpdateButtonState();
        }

        GameplayContainer.Instance.PlatformName = _name;
    }

    public void UpdateButtonState()
    {
        wasBought = PlayerPrefs.GetInt("PlatformBought_" + skinIndex, skinIndex == 0 ? 1 : 0) == 1;
        isSelected = PlayerPrefs.GetInt("SelectedPlatform", -1) == skinIndex;

        if (!wasBought)
        {
            if (buyButton != null) buyButton.gameObject.SetActive(true);
            if (selectButton != null)
            {
                selectButton.onClick.RemoveAllListeners();
                selectButton.onClick.AddListener(SelectSkin);
                selectButton.gameObject.SetActive(false);
            }
            if (checkmark != null) checkmark.SetActive(false);
            int points = PlayerPrefs.GetInt("AllTimeKills", 0);
            if (buyButton != null) buyButton.interactable = points >= price;
            if (platformPreviewRenderer != null)
            {
                var color = platformPreviewRenderer.color;
                color.a = 0.5f;
                platformPreviewRenderer.color = color;
            }
            else if (platformPreviewImage != null)
            {
                var color = platformPreviewImage.color;
                color.a = 0.5f;
                platformPreviewImage.color = color;
            }
        }
        else
        {
            if (buyButton != null) buyButton.gameObject.SetActive(false);
            if (selectButton != null)
            {
                selectButton.onClick.RemoveAllListeners();
                selectButton.onClick.AddListener(SelectSkin);
                selectButton.gameObject.SetActive(!isSelected);
                selectButton.interactable = true;
            }
            if (isSelected)
            {
                if (checkmark != null) checkmark.SetActive(true);
            }
            else
            {
                if (checkmark != null) checkmark.SetActive(false);
            }
            if (platformPreviewRenderer != null)
            {
                var color = platformPreviewRenderer.color;
                color.a = 1f;
                platformPreviewRenderer.color = color;
            }
            else if (platformPreviewImage != null)
            {
                var color = platformPreviewImage.color;
                color.a = 1f;
                platformPreviewImage.color = color;
            }
        }
    }
} 