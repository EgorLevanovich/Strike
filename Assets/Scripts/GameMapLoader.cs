using UnityEngine;

public class GameMapLoader : MonoBehaviour
{
    [SerializeField] private GameObject[] mapPrefabs; // Префабы карт
    private const string MAP_SELECTED_KEY = "MapsSelected";
    private int currentMenuMapIndex = 0;

    void Start()
    {
        LoadSelectedMap();
    }

    private void LoadSelectedMap()
    {
        int selectedMapIndex = PlayerPrefs.GetInt("SelectedMap", -1);
        
        // Если карта не выбрана (-1) или не куплена, не загружаем никакую карту
        if (selectedMapIndex == -1 || !IsMapBought(selectedMapIndex))
        {
            // Деактивируем все карты
            foreach (GameObject map in mapPrefabs)
            {
                if (map != null)
                {
                    map.SetActive(false);
                }
            }
            return;
        }

        // Деактивируем все карты
        foreach (GameObject map in mapPrefabs)
        {
            if (map != null)
            {
                map.SetActive(false);
            }
        }

        // Активируем только выбранную карту
        if (selectedMapIndex < mapPrefabs.Length && mapPrefabs[selectedMapIndex] != null)
        {
            mapPrefabs[selectedMapIndex].SetActive(true);
        }
    }

    private bool IsMapBought(int index)
    {
        return PlayerPrefs.GetInt("MapBought_" + index, index == 0 ? 1 : 0) == 1;
    }

    // Новый публичный метод для мгновенной загрузки карты по индексу
    public void LoadMapByIndex(int index)
    {
        // Проверяем, куплена ли карта
        if (!IsMapBought(index))
        {
            // Если не куплена — ничего не делаем или можно активировать дефолтную карту (например, 0)
            index = 0;
        }
        // Деактивируем все карты
        foreach (GameObject map in mapPrefabs)
        {
            if (map != null)
                map.SetActive(false);
        }
        // Активируем только нужную карту
        if (index < mapPrefabs.Length && mapPrefabs[index] != null)
        {
            mapPrefabs[index].SetActive(true);
        }
    }

    // Показать следующую карту в меню
    public void NextMapInMenu()
    {
        if (mapPrefabs == null || mapPrefabs.Length == 0) return;
        currentMenuMapIndex = (currentMenuMapIndex + 1) % mapPrefabs.Length;
        LoadMapByIndex(currentMenuMapIndex);
        PlayerPrefs.SetInt("SelectedMap", currentMenuMapIndex);
        PlayerPrefs.Save();
    }

    // Показать предыдущую карту в меню
    public void PrevMapInMenu()
    {
        if (mapPrefabs == null || mapPrefabs.Length == 0) return;
        currentMenuMapIndex = (currentMenuMapIndex - 1 + mapPrefabs.Length) % mapPrefabs.Length;
        LoadMapByIndex(currentMenuMapIndex);
        PlayerPrefs.SetInt("SelectedMap", currentMenuMapIndex);
        PlayerPrefs.Save();
    }

    // Получить текущий индекс карты в меню
    public int GetCurrentMenuMapIndex() => currentMenuMapIndex;
} 