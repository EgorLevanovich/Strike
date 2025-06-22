using Assets.Scripts.Gameplay;
using UnityEngine;
using UnityEngine.UI;

public class UniversalSkinButton : MonoBehaviour
{
    [SerializeField] private string _name;
    public int skinIndex;
    public int price;
    public Button buyButton;
    public Button selectButton;
    public GameObject checkmark;
    public AudioSource purchaseSound;
    public AudioSource selectSound;
    public Image ballPreviewImage;
    public SpriteRenderer ballPreviewRenderer;

    private bool wasBought;
    private bool isSelected;
    private static bool needsUpdate = false;

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
            bool canBuy = points >= price;
            if (buyButton.interactable != canBuy)
            {
                buyButton.interactable = canBuy;
            }
        }
    }

    void BuySkin()
    {
        int points = PlayerPrefs.GetInt("AllTimeKills", 0);
        if (points >= price)
        {
            points -= price;
            PlayerPrefs.SetInt("AllTimeKills", points);
            PlayerPrefs.SetInt("BallBought_" + skinIndex, 1);
            PlayerPrefs.Save();
            
            wasBought = true;
            if (buyButton != null) buyButton.gameObject.SetActive(false);
            if (purchaseSound != null) purchaseSound.Play();
            UpdateButtonState();
            needsUpdate = true;
        }
    }

    void SelectSkin()
    {
        if (!wasBought) return;

        if (selectSound != null) selectSound.Play();

        PlayerPrefs.SetInt("SelectedBall", skinIndex);
        PlayerPrefs.Save();

        wasBought = true;
        isSelected = true;
        UpdateButtonState();
        needsUpdate = true;

        GameplayContainer.Instance.BallName = _name;
    }

    public void UpdateButtonState()
    {
        wasBought = PlayerPrefs.GetInt("BallBought_" + skinIndex, skinIndex == 0 ? 1 : 0) == 1;
        isSelected = PlayerPrefs.GetInt("SelectedBall", -1) == skinIndex;

        if (!wasBought)
        {
            if (buyButton != null) buyButton.gameObject.SetActive(true);
            if (selectButton != null)
            {
                selectButton.onClick.RemoveAllListeners();
                selectButton.onClick.AddListener(SelectSkin);
                selectButton.gameObject.SetActive(false);
                selectButton.interactable = true;
            }
            if (checkmark != null) checkmark.SetActive(false);
            int points = PlayerPrefs.GetInt("AllTimeKills", 0);
            bool canBuy = points >= price;
            if (buyButton != null) buyButton.interactable = canBuy;
            float alpha = 0.5f;
            if (ballPreviewRenderer != null)
            {
                var color = ballPreviewRenderer.color;
                color.a = alpha;
                ballPreviewRenderer.color = color;
            }
            else if (ballPreviewImage != null)
            {
                var color = ballPreviewImage.color;
                color.a = alpha;
                ballPreviewImage.color = color;
            }
        }
        else
        {
            if (buyButton != null) buyButton.gameObject.SetActive(false);
            if (selectButton != null)
            {
                selectButton.onClick.RemoveAllListeners();
                selectButton.onClick.AddListener(SelectSkin);
                selectButton.gameObject.SetActive(!isSelected);
                selectButton.interactable = true;
            }
            if (isSelected)
            {
                if (checkmark != null) checkmark.SetActive(true);
            }
            else
            {
                if (checkmark != null) checkmark.SetActive(false);
            }
            if (ballPreviewRenderer != null)
            {
                var color = ballPreviewRenderer.color;
                color.a = 1f;
                ballPreviewRenderer.color = color;
            }
            else if (ballPreviewImage != null)
            {
                var color = ballPreviewImage.color;
                color.a = 1f;
                ballPreviewImage.color = color;
            }
        }
    }
} 