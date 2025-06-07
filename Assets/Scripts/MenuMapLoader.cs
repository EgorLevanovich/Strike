using UnityEngine;

public class MenuMapLoader : MonoBehaviour
{
    [SerializeField] private GameObject[] mapPrefabs; // Префабы карт для меню
    private const string SELECTED_MAP_KEY = "SelectedMap";

    void Start()
    {
        LoadSelectedMenuMap();
    }

    public void LoadSelectedMenuMap()
    {
        int selectedMapIndex = PlayerPrefs.GetInt(SELECTED_MAP_KEY, 0);
        // Проверяем, куплена ли карта
        if (!IsMapBought(selectedMapIndex))
        {
            // Если не куплена, ищем первую купленную
            selectedMapIndex = 0;
            for (int i = 0; i < mapPrefabs.Length; i++)
            {
                if (IsMapBought(i))
                {
                    selectedMapIndex = i;
                    break;
                }
            }
            PlayerPrefs.SetInt(SELECTED_MAP_KEY, selectedMapIndex);
            PlayerPrefs.Save();
        }
        // Деактивируем все карты
        foreach (GameObject map in mapPrefabs)
        {
            if (map != null)
                map.SetActive(false);
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
} 