using UnityEngine;
using UnityEngine.Advertisements;

public class CurrencyManager : MonoBehaviour
{
    private static CurrencyManager instance;
    public static CurrencyManager Instance
    {
        get { return instance; }
    }

    private const string TOTAL_KILLS_KEY = "AllTimeKills";
    private const string POINTS_PER_HUNDRED_KEY = "PointsPerHundred";

    [SerializeField] private string androidAdUnitId = "Rewarded_Android";
    [SerializeField] private string iosAdUnitId = "Rewarded_iOS";

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        Advertisement.Initialize("5855765", false); // false — если не тестовый режим
    }

    // Получить обычные очки
    public int GetRegularPoints()
    {
        return PlayerPrefs.GetInt("TotalKills", 0);
    }

    // Получить очки за 100 убийств
    public int GetPointsPerHundred()
    {
        int totalKills = PlayerPrefs.GetInt(TOTAL_KILLS_KEY, 0);
        return totalKills / 100;
    }

    // Потратить обычные очки
    public bool SpendRegularPoints(int amount)
    {
        int currentPoints = GetRegularPoints();
        if (currentPoints >= amount)
        {
            PlayerPrefs.SetInt("TotalKills", currentPoints - amount);
            PlayerPrefs.Save();
            return true;
        }
        return false;
    }

    // Потратить очки за 100 убийств
    public bool SpendPointsPerHundred(int amount)
    {
        int currentPoints = GetPointsPerHundred();
        if (currentPoints >= amount)
        {
            // Вычитаем из общего количества убийств
            int totalKills = PlayerPrefs.GetInt(TOTAL_KILLS_KEY, 0);
            PlayerPrefs.SetInt(TOTAL_KILLS_KEY, totalKills - (amount * 100));
            PlayerPrefs.Save();
            return true;
        }
        return false;
    }
} 