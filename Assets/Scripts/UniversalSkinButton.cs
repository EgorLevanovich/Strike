using UnityEngine;
using UnityEngine.UI;

public class UniversalSkinButton : MonoBehaviour
{
    public int skinIndex;
    public int price;
    public Button buyButton;
    public Button selectButton;
    public GameObject checkmark;

    private bool wasBought;
    private bool isSelected;

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
        // Проверяем доступность покупки в реальном времени
        if (!wasBought && buyButton != null)
        {
            int points = PlayerPrefs.GetInt("AllTimeKills", 0);
            buyButton.interactable = points >= price;
        }
    }

    void BuySkin()
    {
        // BallManager управляет покупкой и обновлением UI
    }

    void SelectSkin()
    {
        // BallManager управляет выбором и обновлением UI
    }

    public void UpdateButtonState()
    {
        wasBought = PlayerPrefs.GetInt("BallBought_" + skinIndex, skinIndex == 0 ? 1 : 0) == 1;
        isSelected = PlayerPrefs.GetInt("SelectedBall", -1) == skinIndex;

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
} 