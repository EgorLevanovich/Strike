using System.Collections.Generic;
using UnityEngine;

public class SkinsManager : MonoBehaviour
{
    public List<GameObject> skinPrefabs;
    public int score;
    public static SkinsManager Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        LoadSkins();
        LoadScore();
    }

    public bool CanBuySkin(int index)
    {
        var skinData = skinPrefabs[index].GetComponent<SkinData>();
        return !skinData.isUnlocked && score >= skinData.price;
    }

    public void BuySkin(int index)
    {
        var skinData = skinPrefabs[index].GetComponent<SkinData>();
        if (CanBuySkin(index))
        {
            score -= skinData.price;
            skinData.isUnlocked = true;
            SaveSkins();
            SaveScore();
        }
    }

    public void SelectSkin(int index)
    {
        var skinData = skinPrefabs[index].GetComponent<SkinData>();
        if (skinData.isUnlocked)
        {
            PlayerPrefs.SetInt("SelectedSkin", index);
        }
    }

    private void LoadSkins()
    {
        for (int i = 0; i < skinPrefabs.Count; i++)
        {
            var skinData = skinPrefabs[i].GetComponent<SkinData>();
            skinData.isUnlocked = PlayerPrefs.GetInt("SkinUnlocked" + i, i == 0 ? 1 : 0) == 1;
        }
    }

    private void SaveSkins()
    {
        for (int i = 0; i < skinPrefabs.Count; i++)
        {
            var skinData = skinPrefabs[i].GetComponent<SkinData>();
            PlayerPrefs.SetInt("SkinUnlocked" + i, skinData.isUnlocked ? 1 : 0);
        }
    }

    private void LoadScore()
    {
        score = PlayerPrefs.GetInt("Score", 0);
    }

    private void SaveScore()
    {
        PlayerPrefs.SetInt("Score", score);
    }

    public void AddScore(int amount)
    {
        score += amount;
        SaveScore();
    }
} 