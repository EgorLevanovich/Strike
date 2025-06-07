using UnityEngine;
using UnityEngine.UI;

public class UniversalMapSkinButton : MonoBehaviour
{
    public int skinIndex;
    public int price;
    public Button buyButton;
    public Button selectButton;
    public GameObject checkmark;
    public Image mapPreviewImage;
    public SpriteRenderer mapPreviewRenderer;
    public AudioSource purchaseSound;

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
            PlayerPrefs.SetInt("MapBought_" + skinIndex, 1);
            PlayerPrefs.Save();
            
            if (purchaseSound != null)
                purchaseSound.Play();
            
            wasBought = true;
            UpdateButtonState();
            
            foreach (var button in FindObjectsOfType<UniversalMapSkinButton>())
            {
                button.wasBought = PlayerPrefs.GetInt("MapBought_" + button.skinIndex, button.skinIndex == 0 ? 1 : 0) == 1;
                button.isSelected = PlayerPrefs.GetInt("SelectedMap", -1) == button.skinIndex;
                button.UpdateButtonState();
            }
        }
    }

    void SelectSkin()
    {
        if (!wasBought) return;

        PlayerPrefs.SetInt("SelectedMap", skinIndex);
        PlayerPrefs.Save();
        
        isSelected = true;
        UpdateButtonState();
        
        foreach (var button in FindObjectsOfType<UniversalMapSkinButton>())
        {
            button.wasBought = PlayerPrefs.GetInt("MapBought_" + button.skinIndex, button.skinIndex == 0 ? 1 : 0) == 1;
            button.isSelected = PlayerPrefs.GetInt("SelectedMap", -1) == button.skinIndex;
            button.UpdateButtonState();
        }
    }

    public void UpdateButtonState()
    {
        wasBought = PlayerPrefs.GetInt("MapBought_" + skinIndex, skinIndex == 0 ? 1 : 0) == 1;
        isSelected = PlayerPrefs.GetInt("SelectedMap", -1) == skinIndex;

        if (!wasBought)
        {
            if (buyButton != null) buyButton.gameObject.SetActive(true);
            if (selectButton != null) selectButton.gameObject.SetActive(false);
            if (checkmark != null) checkmark.SetActive(false);
            int points = PlayerPrefs.GetInt("AllTimeKills", 0);
            if (buyButton != null) buyButton.interactable = points >= price;
            if (mapPreviewRenderer != null)
            {
                var color = mapPreviewRenderer.color;
                color.a = 0.5f;
                mapPreviewRenderer.color = color;
            }
            else if (mapPreviewImage != null)
            {
                var color = mapPreviewImage.color;
                color.a = 0.5f;
                mapPreviewImage.color = color;
            }
        }
        else
        {
            if (buyButton != null) buyButton.gameObject.SetActive(false);

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
            if (mapPreviewRenderer != null)
            {
                var color = mapPreviewRenderer.color;
                color.a = 1f;
                mapPreviewRenderer.color = color;
            }
            else if (mapPreviewImage != null)
            {
                var color = mapPreviewImage.color;
                color.a = 1f;
                mapPreviewImage.color = color;
            }
        }
    }
} 