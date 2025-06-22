using UnityEngine;
using System.IO;
using UnityEngine.UI;

public class CoinsToFile : MonoBehaviour
{
    public string fileName = "coins.txt";
    public Text coinsTextFile; // UI-объект для отображения количества монет

    public void SaveCoinsToFile()
    {
        int coins = 0;
        if (Money.Instance != null)
            coins = Money.Instance.Coins;
        string path = Path.Combine(Application.persistentDataPath, fileName);
        File.WriteAllText(path, coins.ToString());
        Debug.Log($"Coins saved to: {path} (value: {coins})");
        UpdateCoinsTextFile();
    }

    public void UpdateCoinsTextFile()
    {
        int coins = 0;
        if (Money.Instance != null)
            coins = Money.Instance.Coins;
        if (coinsTextFile != null)
            coinsTextFile.text = coins.ToString();
    }
} 