using UnityEngine;
using UnityEngine.UI;

public class SkinsUI : MonoBehaviour
{
    public SkinsManager skinsManager;
    public Transform skinsPanel;
    public GameObject skinButtonPrefab;
    public Text scoreText;

    void Start()
    {
        UpdateUI();
    }

    public void UpdateUI()
    {
        foreach (Transform child in skinsPanel)
            Destroy(child.gameObject);

        for (int i = 0; i < skinsManager.skinPrefabs.Count; i++)
        {
            var skin = skinsManager.skinPrefabs[i].GetComponent<SkinData>();
            GameObject btnObj = Instantiate(skinButtonPrefab, skinsPanel);
            SkinButton btn = btnObj.GetComponent<SkinButton>();
            btn.Setup(i, skin, this);
        }

        if (scoreText != null)
            scoreText.text = skinsManager.score.ToString();
    }

    public void OnSkinButtonClicked(int index)
    {
        var skin = skinsManager.skinPrefabs[index].GetComponent<SkinData>();
        if (skin.isUnlocked)
            skinsManager.SelectSkin(index);
        else
            skinsManager.BuySkin(index);
        UpdateUI();
    }
} 