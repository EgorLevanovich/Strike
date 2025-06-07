using UnityEngine;

public class BackGroundLoader : MonoBehaviour
{
    [SerializeField] private GameObject[] backgrounds; // Префабы фонов
    private const string SELECTED_MAP_KEY = "SelectedMap";

    void Start()
    {
        LoadSelectedBackground();
    }

    public void LoadSelectedBackground()
    {
        int selectedMapIndex = PlayerPrefs.GetInt(SELECTED_MAP_KEY, -1);
        Debug.Log($"[BackGroundLoader] LoadSelectedBackground called. selectedMapIndex={selectedMapIndex}, куплена? {IsMapBought(selectedMapIndex)}");
        // Если карта не выбрана (-1) или не куплена, не показываем фон
        if (selectedMapIndex == -1 || !IsMapBought(selectedMapIndex))
        {
            foreach (GameObject bg in backgrounds)
            {
                if (bg != null) bg.SetActive(false);
            }
            return;
        }
        // Деактивируем все фоны
        for (int i = 0; i < backgrounds.Length; i++)
        {
            if (backgrounds[i] != null)
                backgrounds[i].SetActive(i == selectedMapIndex);
        }
    }

    private bool IsMapBought(int index)
    {
        return PlayerPrefs.GetInt("MapBought_" + index, index == 0 ? 1 : 0) == 1;
    }

    public void ActivateBackgroundByIndex(int index)
    {
        int toActivate = IsMapBought(index) ? index : 0;
        for (int i = 0; i < backgrounds.Length; i++)
        {
            if (backgrounds[i] != null)
                backgrounds[i].SetActive(i == toActivate);
        }
    }
}
