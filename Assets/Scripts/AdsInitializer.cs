using UnityEngine;
using UnityEngine.Advertisements;

public class AdsInitializer : MonoBehaviour, IUnityAdsInitializationListener, IUnityAdsLoadListener, IUnityAdsShowListener
{
    [SerializeField] private string _adId;
    [SerializeField] private bool _testMode;
    [SerializeField] private string _interstitialId;
    [SerializeField] private string _rewardedId;
   
    public static AdsInitializer Instance { get; private set; }
    private System.Action _rewardedCompletedCallback;
    private System.Action _rewardedFailuredCallback;
    private System.Action _interstitialCompletedCallback;
    private System.Action _interstitialFailuredCallback;
    public event System.Action OnRewardedShowCompleted;
    public event System.Action OnRewardedShowFailured;
    public event System.Action OnInterstitialShowCompleted;
    public event System.Action OnInterstitialShowFailured;
    public event System.Action OnRewardedLoaded;
    public event System.Action OnInterstitialLoaded;

    private bool _isRewardedReady = false;
    private bool _isInterstitialReady = false;

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
            return;
        }

        Advertisement.Initialize(_adId, _testMode, this);
    }

    public void LoadRewarded()
    {
        Advertisement.Load(_rewardedId, this);
        
    }

    public void LoadInterstitial()
    {
        Advertisement.Load(_interstitialId, this);
    }

    public void ShowRewarded(System.Action completedCallback, System.Action failuredCallback)
    {
        _rewardedCompletedCallback = completedCallback;
        _rewardedFailuredCallback = failuredCallback;
        Advertisement.Show(_rewardedId, this);
    }

    public void ShowInterstitial(System.Action completedCallback, System.Action failuredCallback)
    {
        _interstitialCompletedCallback = completedCallback;
        _interstitialFailuredCallback = failuredCallback;
        Advertisement.Show(_interstitialId, this);
    }

    public void ShowInterstitial()
    {
        Advertisement.Show(_interstitialId, this);
    }

    public void OnInitializationComplete()
    {
        Debug.Log($"{nameof(OnInitializationComplete)}");
    }

    public void OnInitializationFailed(UnityAdsInitializationError error, string message)
    {
        Debug.Log($"{nameof(OnInitializationFailed)}");
        
    }

    private void OnDestroy()
    {
        //Instance = null;
    }

    public void OnUnityAdsShowFailure(string placementId, UnityAdsShowError error, string message)
    {
        Debug.Log($"{nameof(OnUnityAdsShowFailure)}");
        if (placementId == _interstitialId)
        {
            OnInterstitialShowFailured?.Invoke();
            _isInterstitialReady = false;
        }
        else
        {
            _rewardedFailuredCallback?.Invoke();
            _rewardedCompletedCallback = null;
            _rewardedFailuredCallback = null;
            OnRewardedShowFailured?.Invoke();
            if (placementId == _rewardedId)
                _isRewardedReady = false;
        }
    }

    public void OnUnityAdsShowStart(string placementId)
    {
        Debug.Log($"{nameof(OnUnityAdsShowStart)}");
    }

    public void OnUnityAdsShowClick(string placementId)
    {
        Debug.Log($"{nameof(OnUnityAdsShowClick)}");
    }

    public void OnUnityAdsShowComplete(string placementId, UnityAdsShowCompletionState showCompletionState)
    {
        Debug.Log($"{nameof(OnUnityAdsShowComplete)}: {placementId}, state: {showCompletionState}");
        if (placementId == _interstitialId)
        {
            if (showCompletionState == UnityAdsShowCompletionState.COMPLETED)
            {
                _interstitialCompletedCallback?.Invoke();
                _interstitialCompletedCallback = null;
                _interstitialFailuredCallback = null;
                OnInterstitialShowCompleted?.Invoke();
            }
            else
            {
                _interstitialFailuredCallback?.Invoke();
                _interstitialCompletedCallback = null;
                _interstitialFailuredCallback = null;
                OnInterstitialShowFailured?.Invoke();
            }
        }
        else if (placementId == _rewardedId)
        {
            if (showCompletionState == UnityAdsShowCompletionState.COMPLETED)
            {
                _rewardedCompletedCallback?.Invoke();
                _rewardedCompletedCallback = null;
                _rewardedFailuredCallback = null;
                OnRewardedShowCompleted?.Invoke();
            }
            else
            {
                _rewardedFailuredCallback?.Invoke();
                _rewardedCompletedCallback = null;
                _rewardedFailuredCallback = null;
                OnRewardedShowFailured?.Invoke();
            }
        }
    }

    public void OnUnityAdsAdLoaded(string placementId)
    {
        Debug.Log($"{nameof(OnUnityAdsAdLoaded)}");
        if (placementId == _rewardedId)
        {
            _isRewardedReady = true;
            OnRewardedLoaded?.Invoke();
        }
        if (placementId == _interstitialId)
        {
            _isInterstitialReady = true;
            OnInterstitialLoaded?.Invoke();
        }
    }

    public void OnUnityAdsFailedToLoad(string placementId, UnityAdsLoadError error, string message)
    {
        Debug.Log($"{nameof(OnUnityAdsFailedToLoad)}");
        if (placementId == _rewardedId)
            _isRewardedReady = false;
        if (placementId == _interstitialId)
            _isInterstitialReady = false;
    }

    public bool IsRewarded()
    {
        return _isRewardedReady;
    }

    public bool IsInterstitialReady()
    {
        return _isInterstitialReady;
    }
} 