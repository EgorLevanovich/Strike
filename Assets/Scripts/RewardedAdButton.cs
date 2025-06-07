using UnityEngine;
using UnityEngine.Advertisements;
using UnityEngine.UI;
using System.Collections;

public class RewardedAdButton : MonoBehaviour, IUnityAdsLoadListener, IUnityAdsShowListener
{
    [Header("Ad Settings")]
    [SerializeField] private string androidAdUnitId = "Rewarded_Android";
    [SerializeField] private string iosAdUnitId = "Rewarded_iOS";

    [Header("UI References")]
    [SerializeField] private GameObject deathMenu; // Меню смерти (GameObject)
    [SerializeField] public Button adButton;      // Кнопка показа рекламы

    private string adUnitId;
    private bool adLoaded = false;

    void Awake()
    {
#if UNITY_IOS
        adUnitId = iosAdUnitId;
#else
        adUnitId = androidAdUnitId;
#endif
        if (adButton != null)
        {
            adButton.onClick.AddListener(ShowAd);
            adButton.gameObject.SetActive(false); // Скрываем кнопку при запуске
        }
    }

    // Показывать кнопку перед появлением меню смерти
    public void ShowRewardButton()
    {
        adLoaded = false;
        if (adButton != null)
            adButton.gameObject.SetActive(false); // Скрываем кнопку до загрузки рекламы
        if (Advertisement.isInitialized)
        {
            Advertisement.Load(adUnitId, this);
        }
        else
        {
            StartCoroutine(WaitForAdsInitAndLoad());
        }
    }

    private IEnumerator WaitForAdsInitAndLoad()
    {
        while (!Advertisement.isInitialized)
        {
            yield return null;
        }
        Advertisement.Load(adUnitId, this);
    }

    public void ShowAd()
    {
        if (adLoaded)
        {
            Advertisement.Show(adUnitId, this);
        }
    }

    // --- Unity Ads Callbacks ---

    public void OnUnityAdsAdLoaded(string placementId)
    {
        adLoaded = true;
        if (adButton != null)
            adButton.gameObject.SetActive(true); // Показываем кнопку только после загрузки рекламы
    }

    public void OnUnityAdsShowComplete(string placementId, UnityAdsShowCompletionState showCompletionState)
    {
        if (showCompletionState == UnityAdsShowCompletionState.COMPLETED)
        {
            // Дать игроку жизнь и скрыть меню смерти
            if (deathMenu != null) deathMenu.SetActive(false);

            // Ваша логика: восстановить жизнь, продолжить игру
            var gm = FindObjectOfType<GameManager>();
            if (gm != null)
                gm.SendMessage("GiveExtraLife", SendMessageOptions.DontRequireReceiver);
        }
        else
        {
            if (deathMenu != null) deathMenu.SetActive(true);
        }
    }

    public void OnUnityAdsShowFailure(string placementId, UnityAdsShowError error, string message)
    {
        if (deathMenu != null) deathMenu.SetActive(true);
    }

    public void OnUnityAdsShowStart(string placementId) { }
    public void OnUnityAdsShowClick(string placementId) { }
    public void OnUnityAdsFailedToLoad(string placementId, UnityAdsLoadError error, string message)
    {
        Debug.LogError($"[ADS] Failed to load ad: {placementId}, error: {error}, message: {message}");
        if (deathMenu != null) deathMenu.SetActive(true);
    }
} 