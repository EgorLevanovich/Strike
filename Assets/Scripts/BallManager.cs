using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

[System.Serializable]
public class BallSkin
{
    public string ballName;
    public int price;
    public Sprite ballPreview;
    public bool isPurchased;
}

public class BallManager : MonoBehaviour
{
    public List<BallSkin> balls = new List<BallSkin>();
    public Button[] ballButtons;
    public Button[] selectButtons;
    public Text[] priceTexts;
    public Text[] buttonTexts;
    public Image[] ballPreviews;

    private const string PURCHASED_BALLS_KEY = "PurchasedBalls";
    private int currentBallIndex = 0;

    void Start()
    {
        LoadPurchasedBalls();
        InitializeButtons();
        ShowBallList();
        UpdateAllDisplays();
        // Если ни один мяч не выбран, выбираем первый
        if (PlayerPrefs.GetInt("SelectedBall", -1) == -1)
        {
            PlayerPrefs.SetInt("SelectedBall", 0);
            PlayerPrefs.Save();
            ApplyBall(0);
        }
        // Скрываем все selectButtons для некупленных мячей
        if (selectButtons != null)
        {
            for (int i = 0; i < selectButtons.Length; i++)
            {
                if (selectButtons[i] != null)
                    selectButtons[i].gameObject.SetActive(false);
            }
        }
    }

    void LoadPurchasedBalls()
    {
        string purchasedBalls = PlayerPrefs.GetString(PURCHASED_BALLS_KEY, "");
        string[] purchasedBallIndices = purchasedBalls.Split(',');
        for (int i = 0; i < balls.Count; i++)
        {
            balls[i].isPurchased = false;
        }
        foreach (string index in purchasedBallIndices)
        {
            if (int.TryParse(index, out int ballIndex) && ballIndex < balls.Count)
            {
                balls[ballIndex].isPurchased = true;
            }
        }
        for (int i = 0; i < balls.Count; i++)
        {
            if (PlayerPrefs.GetInt("BallBought_" + i, i == 0 ? 1 : 0) == 1)
                balls[i].isPurchased = true;
        }
    }

    public void SavePurchasedBalls()
    {
        List<string> purchasedIndices = new List<string>();
        for (int i = 0; i < balls.Count; i++)
        {
            if (balls[i].isPurchased)
            {
                purchasedIndices.Add(i.ToString());
                PlayerPrefs.SetInt("BallBought_" + i, 1);
            }
        }
        PlayerPrefs.SetString(PURCHASED_BALLS_KEY, string.Join(",", purchasedIndices));
        PlayerPrefs.Save();
    }

    void InitializeButtons()
    {
        for (int i = 0; i < ballButtons.Length; i++)
        {
            int ballIndex = i;
            if (ballButtons[i] != null)
            {
                ballButtons[i].onClick.RemoveAllListeners();
                ballButtons[i].onClick.AddListener(() => TryPurchaseBall(ballIndex));
            }
        }
    }

    void Update()
    {
        for (int i = 0; i < balls.Count; i++)
        {
            if (ballButtons[i] != null && !balls[i].isPurchased)
            {
                int allTimeKills = PlayerPrefs.GetInt("AllTimeKills", 0);
                ballButtons[i].interactable = allTimeKills >= balls[i].price;
            }
        }
    }

    public void TryPurchaseBall(int ballIndex)
    {
        if (ballIndex >= 0 && ballIndex < balls.Count)
        {
            BallSkin ball = balls[ballIndex];
            int allTimeKills = PlayerPrefs.GetInt("AllTimeKills", 0);
            if (allTimeKills >= ball.price && !ball.isPurchased)
            {
                allTimeKills -= ball.price;
                PlayerPrefs.SetInt("AllTimeKills", allTimeKills);
                PlayerPrefs.Save();
                ball.isPurchased = true;
                SavePurchasedBalls();
                UpdateButtonState(ballIndex);
                // Скрываем кнопку покупки
                if (ballIndex < ballButtons.Length && ballButtons[ballIndex] != null)
                    ballButtons[ballIndex].gameObject.SetActive(false);
                // Скрываем текст с ценой
                if (ballIndex < priceTexts.Length && priceTexts[ballIndex] != null)
                    priceTexts[ballIndex].gameObject.SetActive(false);
                // Показываем кнопку выбрать
                if (selectButtons != null && ballIndex < selectButtons.Length && selectButtons[ballIndex] != null)
                {
                    selectButtons[ballIndex].gameObject.SetActive(true);
                    selectButtons[ballIndex].onClick.RemoveAllListeners();
                    int idx = ballIndex;
                    selectButtons[ballIndex].onClick.AddListener(() => {
                        PlayerPrefs.SetInt("SelectedBall", idx);
                        PlayerPrefs.Save();
                        ApplyBall(idx);
                        UpdateAllDisplays();
                        UpdateAllSkinButtons();
                    });
                }
                UpdateAllDisplays();
                UpdateAllSkinButtons();
            }
        }
    }

    void UpdateAllDisplays()
    {
        int selectedBall = PlayerPrefs.GetInt("SelectedBall", -1);
        for (int i = 0; i < balls.Count; i++)
        {
            UpdatePriceDisplay(i);
            UpdateButtonState(i);
            UpdateBallPreview(i);
            bool wasBought = balls[i].isPurchased;
            bool isSelected = selectedBall == i;
            if (ballButtons != null && i < ballButtons.Length && ballButtons[i] != null)
                ballButtons[i].gameObject.SetActive(!wasBought);
            if (selectButtons != null && i < selectButtons.Length && selectButtons[i] != null)
            {
                bool showSelect = wasBought && !isSelected;
                selectButtons[i].gameObject.SetActive(showSelect);
                selectButtons[i].onClick.RemoveAllListeners();
                int idx = i;
                selectButtons[i].onClick.AddListener(() => {
                    PlayerPrefs.SetInt("SelectedBall", idx);
                    PlayerPrefs.Save();
                    ApplyBall(idx);
                    UpdateAllDisplays();
                    UpdateAllSkinButtons();
                });
            }
        }
    }

    void UpdatePriceDisplay(int ballIndex)
    {
        if (ballIndex < priceTexts.Length && priceTexts[ballIndex] != null)
        {
            priceTexts[ballIndex].text = balls[ballIndex].price.ToString();
        }
    }

    void UpdateButtonState(int ballIndex)
    {
        if (ballIndex < buttonTexts.Length && buttonTexts[ballIndex] != null)
        {
            buttonTexts[ballIndex].text = balls[ballIndex].isPurchased ? "Куплено" : "Купить";
        }
        if (ballIndex < ballButtons.Length && ballButtons[ballIndex] != null)
        {
            ballButtons[ballIndex].gameObject.SetActive(!balls[ballIndex].isPurchased);
            ballButtons[ballIndex].interactable = !balls[ballIndex].isPurchased;
        }
    }

    void UpdateBallPreview(int ballIndex)
    {
        if (ballIndex < ballPreviews.Length && ballPreviews[ballIndex] != null && balls[ballIndex].ballPreview != null)
        {
            ballPreviews[ballIndex].sprite = balls[ballIndex].ballPreview;
        }
    }

    public void ApplyBall(int ballIndex)
    {
        if (ballIndex < 0 || ballIndex >= balls.Count) return;
        
        PlayerPrefs.SetInt("SelectedBall", ballIndex);
        PlayerPrefs.Save();
        
        // Обновляем UI только для измененного мяча
        UpdateButtonState(ballIndex);
        UpdateBallPreview(ballIndex);
        
        // Обновляем состояние кнопок выбора
        if (selectButtons != null && ballIndex < selectButtons.Length)
        {
            selectButtons[ballIndex].gameObject.SetActive(false);
        }
        
        // Обновляем все кнопки скинов
        UpdateAllSkinButtons();
    }

    public int GetCurrentBallIndex()
    {
        return PlayerPrefs.GetInt("SelectedBall", -1);
    }

    public void NextBall()
    {
        int startIndex = currentBallIndex;
        do
        {
            currentBallIndex = (currentBallIndex + 1) % balls.Count;
            if (balls[currentBallIndex].isPurchased)
            {
                PlayerPrefs.SetInt("SelectedBall", currentBallIndex);
                PlayerPrefs.Save();
                ApplyBall(currentBallIndex);
                return;
            }
        } while (currentBallIndex != startIndex);
        currentBallIndex = 0;
        PlayerPrefs.SetInt("SelectedBall", 0);
        PlayerPrefs.Save();
        ApplyBall(0);
    }

    public void PrevBall()
    {
        int startIndex = currentBallIndex;
        do
        {
            currentBallIndex = (currentBallIndex - 1 + balls.Count) % balls.Count;
            if (balls[currentBallIndex].isPurchased)
            {
                PlayerPrefs.SetInt("SelectedBall", currentBallIndex);
                PlayerPrefs.Save();
                ApplyBall(currentBallIndex);
                return;
            }
        } while (currentBallIndex != startIndex);
        currentBallIndex = 0;
        PlayerPrefs.SetInt("SelectedBall", 0);
        PlayerPrefs.Save();
        ApplyBall(0);
    }

    public void RefreshAllButtons()
    {
        UpdateAllDisplays();
    }

    void OnEnable()
    {
        LoadPurchasedBalls();
        UpdateAllDisplays();
        UpdateAllSkinButtons();
    }

    public int GetSelectedBallIndex()
    {
        return PlayerPrefs.GetInt("SelectedBall", -1);
    }

    public void UpdateAllSkinButtons()
    {
        foreach (var btn in FindObjectsOfType<UniversalSkinButton>())
            btn.UpdateButtonState();
    }

    public void ShowBallList()
    {
        int firstIndex = 0;
        int selected = PlayerPrefs.GetInt("SelectedBall", 0);
        if (balls.Count == 0) return;
        if (balls[selected].isPurchased)
        {
            firstIndex = selected;
        }
        else
        {
            for (int i = 0; i < balls.Count; i++)
            {
                if (balls[i].isPurchased)
                {
                    firstIndex = i;
                    break;
                }
            }
        }
        if (firstIndex != 0)
        {
            var temp = balls[0];
            balls[0] = balls[firstIndex];
            balls[firstIndex] = temp;
        }
        UpdateAllDisplays();
    }
} 