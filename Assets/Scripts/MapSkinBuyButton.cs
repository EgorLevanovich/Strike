using UnityEngine;
using UnityEngine.UI;

public class MapSkinBuyButton : MonoBehaviour
{
    public Button buyButton;
    public Button selectButton;
    public GameObject checkmark;
    public int price = 10;
    public int mapIndex;

    private bool wasBought = false;
    private bool isSelected = false;

    void Start()
    {
        if (buyButton != null)
            buyButton.onClick.AddListener(TryBuy);
        if (selectButton != null)
            selectButton.onClick.AddListener(SelectMap);
        UpdateButtonState();
    }

    void UpdateButtonState()
    {
        wasBought = PlayerPrefs.GetInt("MapBought_" + mapIndex, mapIndex == 0 ? 1 : 0) == 1;
        isSelected = PlayerPrefs.GetInt("SelectedMap", -1) == mapIndex;

        if (!wasBought)
        {
            if (buyButton != null) buyButton.gameObject.SetActive(true);
            if (selectButton != null) selectButton.gameObject.SetActive(false);
            if (checkmark != null) checkmark.SetActive(false);

            int points = PlayerPrefs.GetInt("AllTimeKills", 0);
            if (buyButton != null) buyButton.interactable = points >= price;
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
        }
    }

    void TryBuy()
    {
        int points = PlayerPrefs.GetInt("AllTimeKills", 0);
        if (points >= price && !wasBought)
        {
            PlayerPrefs.SetInt("AllTimeKills", points - price);
            PlayerPrefs.SetInt("MapBought_" + mapIndex, 1);
            PlayerPrefs.Save();
            wasBought = true;
            UpdateButtonState();
        }
    }

    void SelectMap()
    {
        if (!wasBought) return;

        PlayerPrefs.SetInt("SelectedMap", mapIndex);
        PlayerPrefs.Save();

        // Обновить все кнопки на сцене
        foreach (var btn in FindObjectsOfType<MapSkinBuyButton>())
            btn.UpdateButtonState();
    }

    void OnEnable()
    {
        UpdateButtonState();
    }
} 