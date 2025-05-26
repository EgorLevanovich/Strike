using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Money : MonoBehaviour
{
    public static Money Instance { get; private set; }
    public int Coins { get; private set; }
    public Text cointsText;
    private const string CoinsKey = "PlayerCoins";
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadCoins();
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }

    public void AddCoins(int amount)
    {
        Coins += amount;
        if (Coins < 0) Coins = 0;
        Debug.Log("[Money] Coins now: " + Coins);
        UpdateCoinsText();
        SaveCoins();
    }
    public void UpdateCoinsText()
    {
        if (cointsText != null)
            cointsText.text = ":" + Coins;
    }

    private void SaveCoins()
    {
        PlayerPrefs.SetInt(CoinsKey, Coins);
        PlayerPrefs.Save();
    }

    private void LoadCoins()
    {
        Coins = PlayerPrefs.GetInt(CoinsKey,0);
    }

    public void ResetCoins()
    {
        Coins = 0;
        SaveCoins();
        UpdateCoinsText() ;
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnsceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnsceneLoaded;
    }

    private void OnsceneLoaded(Scene scene, LoadSceneMode mode)
    {
        cointsText = GameObject.Find("CoinsText")?.GetComponent<Text>();
        UpdateCoinsText();
    }
}
