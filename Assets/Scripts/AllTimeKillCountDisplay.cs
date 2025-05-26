using UnityEngine;
using UnityEngine.UI;

public class AllTimeKillCountDisplay : MonoBehaviour
{
    public Text allTimeKillCountText;
    private const string TOTAL_KILLS_KEY = "AllTimeKills";

    void Update()
    {
        int allTimeKills = PlayerPrefs.GetInt(TOTAL_KILLS_KEY, 0);
        if (allTimeKillCountText != null)
            allTimeKillCountText.text = "" + allTimeKills;
    }
} 