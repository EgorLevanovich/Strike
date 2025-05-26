using UnityEngine;

public class GameMapLoader : MonoBehaviour
{
    [SerializeField] private GameObject[] mapPrefabs; // Префабы карт
    private const string MAP_SELECTED_KEY = "MapsSelected";

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
} 