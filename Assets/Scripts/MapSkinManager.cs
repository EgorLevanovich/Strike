using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

[System.Serializable]
public class MapSkin
{
    public string mapName;
    public int price;
    public Sprite mapPreview;
    public bool isPurchased;
}

public class MapSkinManager : MonoBehaviour
{
    public List<MapSkin> maps = new List<MapSkin>();
    public Button[] mapButtons;
    public Button[] selectButtons;
    public Text[] priceTexts;
    public Text[] buttonTexts;
    public Image[] mapPreviews;

    private const string PURCHASED_MAPS_KEY = "PurchasedMaps";
    private int currentMapIndex = 0;

    void Start()
    {
        LoadPurchasedMaps();
        InitializeButtons();
        ShowMapList();
        UpdateAllDisplays();
        // Если ни одна карта не выбрана, выбираем карту с индексом 0
        if (PlayerPrefs.GetInt("SelectedMap", -1) == -1)
        {
            PlayerPrefs.SetInt("SelectedMap", 0);
            PlayerPrefs.Save();
            ApplyMap(0);
        }
    }

    void LoadPurchasedMaps()
    {
        string purchasedMaps = PlayerPrefs.GetString(PURCHASED_MAPS_KEY, "");
        string[] purchasedMapIndices = purchasedMaps.Split(',');
        for (int i = 0; i < maps.Count; i++)
        {
            maps[i].isPurchased = false;
        }
        foreach (string index in purchasedMapIndices)
        {
            if (int.TryParse(index, out int mapIndex) && mapIndex < maps.Count)
            {
                maps[mapIndex].isPurchased = true;
            }
        }
        for (int i = 0; i < maps.Count; i++)
        {
            if (PlayerPrefs.GetInt("MapBought_" + i, i == 0 ? 1 : 0) == 1)
                maps[i].isPurchased = true;
        }
    }

    public void SavePurchasedMaps()
    {
        List<string> purchasedIndices = new List<string>();
        for (int i = 0; i < maps.Count; i++)
        {
            if (maps[i].isPurchased)
            {
                purchasedIndices.Add(i.ToString());
                PlayerPrefs.SetInt("MapBought_" + i, 1);
            }
        }
        PlayerPrefs.SetString(PURCHASED_MAPS_KEY, string.Join(",", purchasedIndices));
        PlayerPrefs.Save();
    }

    void InitializeButtons()
    {
        int count = Mathf.Min(maps.Count, mapButtons.Length);
        for (int i = 0; i < count; i++)
        {
            int mapIndex = i;
            if (mapButtons[i] != null)
            {
                mapButtons[i].onClick.RemoveAllListeners();
                mapButtons[i].onClick.AddListener(() => TryPurchaseMap(mapIndex));
            }
        }
    }

    void Update()
    {
        int count = Mathf.Min(maps.Count, mapButtons.Length);
        for (int i = 0; i < count; i++)
        {
            if (mapButtons[i] != null && !maps[i].isPurchased)
            {
                int allTimeKills = PlayerPrefs.GetInt("AllTimeKills", 0);
                mapButtons[i].interactable = allTimeKills >= maps[i].price;
            }
        }
    }

    public void TryPurchaseMap(int mapIndex)
    {
        if (mapIndex >= 0 && mapIndex < maps.Count)
        {
            MapSkin map = maps[mapIndex];
            int allTimeKills = PlayerPrefs.GetInt("AllTimeKills", 0);
            if (allTimeKills >= map.price && !map.isPurchased)
            {
                allTimeKills -= map.price;
                PlayerPrefs.SetInt("AllTimeKills", allTimeKills);
                PlayerPrefs.Save();
                map.isPurchased = true;
                SavePurchasedMaps();
                
                // Обновляем все кнопки
                foreach (var btn in FindObjectsOfType<UniversalMapSkinButton>())
                {
                    btn.UpdateButtonState();
                }
                
                UpdateAllDisplays();
                // Обновляем фон в меню сразу после покупки
                var loader = FindObjectOfType<BackGroundLoader>();
                if (loader != null) loader.LoadSelectedBackground();
            }
        }
    }

    public void SelectMap(int idx)
    {
        PlayerPrefs.SetInt("SelectedMap", idx);
        PlayerPrefs.Save();
        ApplyMap(idx);
        // Обновляем фон в меню мгновенно по индексу, но если не куплено — фон 0
        var loader = FindObjectOfType<BackGroundLoader>();
        if (loader != null)
        {
            if (maps[idx].isPurchased)
                loader.ActivateBackgroundByIndex(idx);
            else
                loader.ActivateBackgroundByIndex(0);
        }
        // Обновляем все кнопки сразу
        foreach (var btn in FindObjectsOfType<UniversalMapSkinButton>())
        {
            btn.UpdateButtonState();
        }
        UpdateAllDisplays();
        // Мгновенно обновляем карту в menu через MenuMapLoader
        var menuMapLoader = FindObjectOfType<MenuMapLoader>();
        if (menuMapLoader != null)
            menuMapLoader.LoadSelectedMenuMap();
    }

    public void UpdateAllDisplays()
    {
        int selectedMap = PlayerPrefs.GetInt("SelectedMap", -1);
        int count = Mathf.Min(maps.Count, priceTexts.Length, buttonTexts.Length, mapPreviews.Length);
        for (int i = 0; i < count; i++)
        {
            UpdatePriceDisplay(i);
            UpdateButtonState(i);
            UpdateMapPreview(i);
        }
    }

    void UpdatePriceDisplay(int mapIndex)
    {
        if (mapIndex < priceTexts.Length && priceTexts[mapIndex] != null && mapIndex < maps.Count)
        {
            priceTexts[mapIndex].text = maps[mapIndex].price.ToString();
        }
    }

    void UpdateButtonState(int mapIndex)
    {
        if (mapIndex < buttonTexts.Length && buttonTexts[mapIndex] != null && mapIndex < maps.Count)
        {
            buttonTexts[mapIndex].text = maps[mapIndex].isPurchased ? "Куплено" : "Купить";
        }
    }

    void UpdateMapPreview(int mapIndex)
    {
        if (mapIndex < mapPreviews.Length && mapPreviews[mapIndex] != null && mapIndex < maps.Count && maps[mapIndex].mapPreview != null)
        {
            mapPreviews[mapIndex].sprite = maps[mapIndex].mapPreview;
        }
    }

    public void ApplyMap(int mapIndex)
    {
        PlayerPrefs.SetInt("SelectedMap", mapIndex);
        PlayerPrefs.Save();
        Debug.Log($"Карта {maps[mapIndex].mapName} успешно применена!");
        PlayerPrefs.SetInt("MapsSelected", mapIndex);
        PlayerPrefs.Save();
        // Обновляем фон в меню мгновенно по индексу, но если не куплено — фон 0
        var loader = FindObjectOfType<BackGroundLoader>();
        if (loader != null)
        {
            if (maps[mapIndex].isPurchased)
                loader.ActivateBackgroundByIndex(mapIndex);
            else
                loader.ActivateBackgroundByIndex(0);
        }
        // Здесь добавьте код для применения скина карты в вашей игре
    }

    public int GetCurrentMapIndex()
    {
        return PlayerPrefs.GetInt("SelectedMap", -1);
    }

    public void NextMap()
    {
        int startIndex = currentMapIndex;
        do
        {
            currentMapIndex = (currentMapIndex + 1) % maps.Count;
            if (maps[currentMapIndex].isPurchased)
            {
                PlayerPrefs.SetInt("SelectedMap", currentMapIndex);
                PlayerPrefs.Save();
                ApplyMap(currentMapIndex);
                return;
            }
        } while (currentMapIndex != startIndex);
        currentMapIndex = 0;
        PlayerPrefs.SetInt("SelectedMap", 0);
        PlayerPrefs.Save();
        ApplyMap(0);
    }

    public void PrevMap()
    {
        int startIndex = currentMapIndex;
        do
        {
            currentMapIndex = (currentMapIndex - 1 + maps.Count) % maps.Count;
            if (maps[currentMapIndex].isPurchased)
            {
                PlayerPrefs.SetInt("SelectedMap", currentMapIndex);
                PlayerPrefs.Save();
                ApplyMap(currentMapIndex);
                return;
            }
        } while (currentMapIndex != startIndex);
        currentMapIndex = 0;
        PlayerPrefs.SetInt("SelectedMap", 0);
        PlayerPrefs.Save();
        ApplyMap(0);
    }

    void OnEnable()
    {
        LoadPurchasedMaps();
        UpdateAllDisplays();
    }

    public void ShowMapList()
    {
        int firstIndex = 0;
        int selected = PlayerPrefs.GetInt("SelectedMap", 0);
        if (maps.Count == 0) return;
        if (maps[selected].isPurchased)
        {
            firstIndex = selected;
        }
        else
        {
            for (int i = 0; i < maps.Count; i++)
            {
                if (maps[i].isPurchased)
                {
                    firstIndex = i;
                    break;
                }
            }
        }
        if (firstIndex != 0)
        {
            var temp = maps[0];
            maps[0] = maps[firstIndex];
            maps[firstIndex] = temp;
        }
        UpdateAllDisplays();
    }
} 