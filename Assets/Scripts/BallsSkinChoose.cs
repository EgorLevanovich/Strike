using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class BallsSkinChoose : MonoBehaviour
{
    public int skinIndex;
    public int price;
    public Button buyButton;
    public Button selectButton;
    public GameObject checkmark;
    public GameObject ballPrefab; // Префаб мяча для загрузки в сцену

    private bool wasBought;
    private bool isSelected;

    private static List<BallsSkinChoose> allButtons = new List<BallsSkinChoose>();

    void Awake()
    {
        allButtons.Add(this);
    }

    void OnDestroy()
    {
        allButtons.Remove(this);
    }

    void Start()
    {
        if (buyButton != null)
            buyButton.onClick.AddListener(BuySkin);
        if (selectButton != null)
            selectButton.onClick.AddListener(SelectSkin);

        UpdateButtonState();
    }

    void OnEnable()
    {
        UpdateButtonState();
    }

    void Update()
    {
        if (!wasBought && buyButton != null)
        {
            int points = PlayerPrefs.GetInt("AllTimeKills", 0);
            buyButton.interactable = points >= price;
        }
    }

    void BuySkin()
    {
        int points = PlayerPrefs.GetInt("AllTimeKills", 0);
        if (points >= price)
        {
            points -= price;
            PlayerPrefs.SetInt("AllTimeKills", points);
            PlayerPrefs.SetInt("BallSkinBought_" + skinIndex, 1);
            PlayerPrefs.Save();
            UpdateAllButtons();
        }
    }

    void SelectSkin()
    {
        if (!wasBought) return;
        PlayerPrefs.SetInt("SelectedBallSkin", skinIndex);
        PlayerPrefs.Save();
        UpdateAllButtons();
        LoadBallInGameScene();
    }

    public void UpdateButtonState()
    {
        wasBought = PlayerPrefs.GetInt("BallSkinBought_" + skinIndex, skinIndex == 0 ? 1 : 0) == 1;
        isSelected = PlayerPrefs.GetInt("SelectedBallSkin", -1) == skinIndex;

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

    private static void UpdateAllButtons()
    {
        foreach (var button in allButtons)
        {
            button.UpdateButtonState();
        }
    }

    private void LoadBallInGameScene()
    {
        // Пример загрузки мяча в сцену Game
        // Можно доработать под вашу систему спавна
        if (ballPrefab != null && UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == "Game")
        {
            Instantiate(ballPrefab, Vector3.zero, Quaternion.identity);
        }
    }
} 