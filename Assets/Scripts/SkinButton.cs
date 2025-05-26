using UnityEngine;
using UnityEngine.UI;

public class SkinButton : MonoBehaviour
{
    public Image icon;
    public Text priceText;
    public Button actionButton;
    public GameObject lockIcon;

    private int skinIndex;
    private SkinsUI skinsUI;

    public void Setup(int index, SkinData skin, SkinsUI ui)
    {
        skinIndex = index;
        skinsUI = ui;
        icon.sprite = skin.skinSprite;
        priceText.text = skin.isUnlocked ? "Выбрать" : skin.price.ToString();
        lockIcon.SetActive(!skin.isUnlocked);
        actionButton.interactable = skin.isUnlocked || ui.skinsManager.score >= skin.price;
        actionButton.onClick.RemoveAllListeners();
        actionButton.onClick.AddListener(() => skinsUI.OnSkinButtonClicked(skinIndex));
    }
} 