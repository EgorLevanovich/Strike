using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

[System.Serializable]
public class PlatformSkin
{
    public string platformName;
    public int price;
    public Sprite platformPreview;
    public bool isPurchased;
}

public class PlatformSkinManager : MonoBehaviour
{
    public List<PlatformSkin> platforms = new List<PlatformSkin>();
    public Button[] platformButtons;
    public Button[] selectButtons;
    public Text[] priceTexts;
    public Text[] buttonTexts;
    public Image[] platformPreviews;

    private const string PURCHASED_PLATFORMS_KEY = "PurchasedPlatforms";
    private int currentPlatformIndex = 0;

    void Start()
    {
        LoadPurchasedPlatforms();
        InitializeButtons();
        UpdateAllDisplays();
        // Скрываем все selectButtons для некупленных платформ
        if (selectButtons != null)
        {
            for (int i = 0; i < selectButtons.Length; i++)
            {
                if (selectButtons[i] != null)
                    selectButtons[i].gameObject.SetActive(false);
            }
        }
        // Если ни одна платформа не выбрана, выбираем платформу с индексом 0
        if (PlayerPrefs.GetInt("SelectedPlatform", -1) == -1)
        {
            PlayerPrefs.SetInt("SelectedPlatform", 0);
            PlayerPrefs.Save();
            ApplyPlatform(0);
        }
    }

    void LoadPurchasedPlatforms()
    {
        string purchasedPlatforms = PlayerPrefs.GetString(PURCHASED_PLATFORMS_KEY, "");
        string[] purchasedPlatformIndices = purchasedPlatforms.Split(',');
        for (int i = 0; i < platforms.Count; i++)
        {
            platforms[i].isPurchased = false;
        }
        foreach (string index in purchasedPlatformIndices)
        {
            if (int.TryParse(index, out int platformIndex) && platformIndex < platforms.Count)
            {
                platforms[platformIndex].isPurchased = true;
            }
        }
        for (int i = 0; i < platforms.Count; i++)
        {
            if (PlayerPrefs.GetInt("PlatformBought_" + i, i == 0 ? 1 : 0) == 1)
                platforms[i].isPurchased = true;
        }
    }

    public void SavePurchasedPlatforms()
    {
        List<string> purchasedIndices = new List<string>();
        for (int i = 0; i < platforms.Count; i++)
        {
            if (platforms[i].isPurchased)
            {
                purchasedIndices.Add(i.ToString());
                PlayerPrefs.SetInt("PlatformBought_" + i, 1);
            }
        }
        PlayerPrefs.SetString(PURCHASED_PLATFORMS_KEY, string.Join(",", purchasedIndices));
        PlayerPrefs.Save();
    }

    void InitializeButtons()
    {
        for (int i = 0; i < platformButtons.Length; i++)
        {
            int platformIndex = i;
            if (platformButtons[i] != null)
            {
                platformButtons[i].onClick.RemoveAllListeners();
                platformButtons[i].onClick.AddListener(() => TryPurchasePlatform(platformIndex));
            }
        }
    }

    void Update()
    {
        for (int i = 0; i < platforms.Count; i++)
        {
            if (platformButtons[i] != null && !platforms[i].isPurchased)
            {
                int allTimeKills = PlayerPrefs.GetInt("AllTimeKills", 0);
                platformButtons[i].interactable = allTimeKills >= platforms[i].price;
            }
        }
    }

    public void TryPurchasePlatform(int platformIndex)
    {
        if (platformIndex >= 0 && platformIndex < platforms.Count)
        {
            PlatformSkin platform = platforms[platformIndex];
            int allTimeKills = PlayerPrefs.GetInt("AllTimeKills", 0);
            if (allTimeKills >= platform.price && !platform.isPurchased)
            {
                allTimeKills -= platform.price;
                PlayerPrefs.SetInt("AllTimeKills", allTimeKills);
                PlayerPrefs.Save();
                platform.isPurchased = true;
                SavePurchasedPlatforms();
                UpdateButtonState(platformIndex);
                // Скрываем кнопку покупки
                if (platformIndex < platformButtons.Length && platformButtons[platformIndex] != null)
                    platformButtons[platformIndex].gameObject.SetActive(false);
                // Скрываем текст с ценой
                if (platformIndex < priceTexts.Length && priceTexts[platformIndex] != null)
                    priceTexts[platformIndex].gameObject.SetActive(false);
                // Показываем кнопку выбрать
                if (selectButtons != null && platformIndex < selectButtons.Length && selectButtons[platformIndex] != null)
                {
                    selectButtons[platformIndex].gameObject.SetActive(true);
                    selectButtons[platformIndex].onClick.RemoveAllListeners();
                    int idx = platformIndex;
                    selectButtons[platformIndex].onClick.AddListener(() => {
                        SelectPlatform(idx);
                    });
                }
                UpdateAllDisplays();
                UpdateAllSkinButtons();
            }
        }
    }

    public void SelectPlatform(int idx)
    {
        PlayerPrefs.SetInt("SelectedPlatform", idx);
        PlayerPrefs.Save();
        ApplyPlatform(idx);
        
        // Обновляем все кнопки сразу
        foreach (var btn in FindObjectsOfType<UniversalPlatformSkinButton>())
        {
            btn.UpdateButtonState();
        }
        
        UpdateAllDisplays();
        UpdateAllSkinButtons();
    }

    public void UpdateAllDisplays()
    {
        int selectedPlatform = PlayerPrefs.GetInt("SelectedPlatform", -1);
        for (int i = 0; i < platforms.Count; i++)
        {
            UpdatePriceDisplay(i);
            UpdateButtonState(i);
            UpdatePlatformPreview(i);
            bool wasBought = platforms[i].isPurchased;
            bool isSelected = selectedPlatform == i;
            if (platformButtons != null && i < platformButtons.Length && platformButtons[i] != null)
                platformButtons[i].gameObject.SetActive(!wasBought);
            if (selectButtons != null && i < selectButtons.Length && selectButtons[i] != null)
            {
                bool showSelect = wasBought && !isSelected;
                selectButtons[i].gameObject.SetActive(showSelect);
                selectButtons[i].onClick.RemoveAllListeners();
                int idx = i;
                selectButtons[i].onClick.AddListener(() => {
                    SelectPlatform(idx);
                });
            }
        }
    }

    void UpdatePriceDisplay(int platformIndex)
    {
        if (platformIndex < priceTexts.Length && priceTexts[platformIndex] != null)
        {
            priceTexts[platformIndex].text = platforms[platformIndex].price.ToString();
        }
    }

    void UpdateButtonState(int platformIndex)
    {
        if (platformIndex < buttonTexts.Length && buttonTexts[platformIndex] != null)
        {
            buttonTexts[platformIndex].text = platforms[platformIndex].isPurchased ? "Куплено" : "Купить";
        }
        if (platformIndex < platformButtons.Length && platformButtons[platformIndex] != null)
        {
            platformButtons[platformIndex].gameObject.SetActive(true);
            platformButtons[platformIndex].interactable = !platforms[platformIndex].isPurchased;
        }
    }

    void UpdatePlatformPreview(int platformIndex)
    {
        if (platformIndex < platformPreviews.Length && platformPreviews[platformIndex] != null && platforms[platformIndex].platformPreview != null)
        {
            platformPreviews[platformIndex].sprite = platforms[platformIndex].platformPreview;
        }
    }

    public void ApplyPlatform(int platformIndex)
    {
        PlayerPrefs.SetInt("SelectedPlatform", platformIndex);
        PlayerPrefs.Save();
        Debug.Log($"Платформа {platforms[platformIndex].platformName} успешно применена!");
        // Здесь добавьте код для применения скина платформы в вашей игре
    }

    public int GetCurrentPlatformIndex()
    {
        return PlayerPrefs.GetInt("SelectedPlatform", -1);
    }

    public void NextPlatform()
    {
        int startIndex = currentPlatformIndex;
        do
        {
            currentPlatformIndex = (currentPlatformIndex + 1) % platforms.Count;
            if (platforms[currentPlatformIndex].isPurchased)
            {
                PlayerPrefs.SetInt("SelectedPlatform", currentPlatformIndex);
                PlayerPrefs.Save();
                ApplyPlatform(currentPlatformIndex);
                return;
            }
        } while (currentPlatformIndex != startIndex);
        currentPlatformIndex = 0;
        PlayerPrefs.SetInt("SelectedPlatform", 0);
        PlayerPrefs.Save();
        ApplyPlatform(0);
    }

    public void PrevPlatform()
    {
        int startIndex = currentPlatformIndex;
        do
        {
            currentPlatformIndex = (currentPlatformIndex - 1 + platforms.Count) % platforms.Count;
            if (platforms[currentPlatformIndex].isPurchased)
            {
                PlayerPrefs.SetInt("SelectedPlatform", currentPlatformIndex);
                PlayerPrefs.Save();
                ApplyPlatform(currentPlatformIndex);
                return;
            }
        } while (currentPlatformIndex != startIndex);
        currentPlatformIndex = 0;
        PlayerPrefs.SetInt("SelectedPlatform", 0);
        PlayerPrefs.Save();
        ApplyPlatform(0);
    }

    public void RefreshAllButtons()
    {
        UpdateAllDisplays();
    }

    void OnEnable()
    {
        LoadPurchasedPlatforms();
        UpdateAllDisplays();
        UpdateAllSkinButtons();
    }

    public int GetSelectedPlatformIndex()
    {
        return PlayerPrefs.GetInt("SelectedPlatform", -1);
    }

    public void UpdateAllSkinButtons()
    {
        foreach (var btn in FindObjectsOfType<UniversalSkinButton>())
            btn.UpdateButtonState();
    }
} 