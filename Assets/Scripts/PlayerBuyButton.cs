using UnityEngine;
using UnityEngine.UI;

public class PlayerBuyButton : MonoBehaviour
{
    public int ballIndex;
    public int price;
    public Button buyButton;
    public Button selectButton;
    public GameObject checkmark;

    private bool wasBought;
    private bool isSelected;

    public void UpdateButtonState()
    {
        wasBought = PlayerPrefs.GetInt("BallSkinBought_" + ballIndex, ballIndex == 0 ? 1 : 0) == 1;
        isSelected = PlayerPrefs.GetInt("SelectedBall", -1) == ballIndex;

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

    void SelectBall()
    {
        if (!wasBought) return;

        PlayerPrefs.SetInt("SelectedBall", ballIndex);
        PlayerPrefs.Save();

        foreach (var btn in FindObjectsOfType<PlayerBuyButton>())
            btn.UpdateButtonState();
    }
} 