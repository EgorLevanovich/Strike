using UnityEngine;

public class PlatformSkinLoader : MonoBehaviour
{
    public GameObject[] platformPrefabs; // Префабы или объекты платформ на сцене

    void Start()
    {
        LoadSelectedPlatform();
    }

    public void LoadSelectedPlatform()
    {
        int selectedPlatformIndex = PlayerPrefs.GetInt("SelectedPlatform", -1);

        // Если платформа не выбрана или не куплена, деактивируем все
        if (selectedPlatformIndex == -1 || !IsPlatformBought(selectedPlatformIndex))
        {
            foreach (GameObject platform in platformPrefabs)
            {
                if (platform != null)
                    platform.SetActive(false);
            }
            return;
        }

        // Деактивируем все платформы
        foreach (GameObject platform in platformPrefabs)
        {
            if (platform != null)
                platform.SetActive(false);
        }

        // Активируем только выбранную и купленную
        if (selectedPlatformIndex < platformPrefabs.Length && platformPrefabs[selectedPlatformIndex] != null)
        {
            platformPrefabs[selectedPlatformIndex].SetActive(true);
        }
    }

    private bool IsPlatformBought(int index)
    {
        return PlayerPrefs.GetInt("PlatformBought_" + index, index == 0 ? 1 : 0) == 1;
    }
} 