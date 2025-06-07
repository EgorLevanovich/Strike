using UnityEngine;
using UnityEngine.Advertisements;

public class AdsInitializer : MonoBehaviour
{
    [SerializeField] private string androidGameId = "5870473";
    [SerializeField] private string iosGameId = "ВАШ_GAME_ID_ДЛЯ_IOS";
    [SerializeField] private bool testMode = true;

    void Awake()
    {
#if UNITY_ANDROID
        Debug.Log("Initializing Unity Ads with Game ID: " + androidGameId);
        Advertisement.Initialize(androidGameId, testMode);
#elif UNITY_IOS
        Debug.Log("Initializing Unity Ads with Game ID: " + iosGameId);
        Advertisement.Initialize(iosGameId, testMode);
#endif
    }
} 