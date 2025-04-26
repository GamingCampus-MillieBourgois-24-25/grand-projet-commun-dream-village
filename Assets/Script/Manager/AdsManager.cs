using Unity.Services.LevelPlay;
using UnityEngine;
using UnityEngine.UI;

// Example for IronSource Unity.
public class AdsManager : MonoBehaviour
{

#if UNITY_ANDROID
    string appKey = "21d2c31b5";
#else
    string appKey = "unexpected_platform";
#endif

    [SerializeField] private Button skipActivityAdsButton;
    private System.Action onRewardedVideoAdWatched;

    private void Awake()
    {
        IronSource.Agent.setMetaData("is_test_suite", "enable"); 
    }

    private void Start()
    {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
        IronSource.Agent.validateIntegration();
#endif

        LevelPlay.Init(appKey, GetUserId(), adFormats: new[] { com.unity3d.mediation.LevelPlayAdFormat.REWARDED });
        LevelPlay.OnInitSuccess += SdkInitializationCompletedEvent;
        LevelPlay.OnInitFailed += SdkInitializationFailedEvent;

        LoadReward();
    }

    void OnApplicationPause(bool paused)
    {
        IronSource.Agent.onApplicationPause(paused);
    }

    private void OnEnable()
    {
        //Reward ADS
        IronSourceRewardedVideoEvents.onAdOpenedEvent += RewardedVideoOnAdOpenedEvent;
        IronSourceRewardedVideoEvents.onAdClosedEvent += RewardedVideoOnAdClosedEvent;
        IronSourceRewardedVideoEvents.onAdAvailableEvent += RewardedVideoOnAdAvailable;
        IronSourceRewardedVideoEvents.onAdUnavailableEvent += RewardedVideoOnAdUnavailable;
        IronSourceRewardedVideoEvents.onAdShowFailedEvent += RewardedVideoOnAdShowFailedEvent;
        IronSourceRewardedVideoEvents.onAdRewardedEvent += RewardedVideoOnAdRewardedEvent;
        IronSourceRewardedVideoEvents.onAdClickedEvent += RewardedVideoOnAdClickedEvent;

    }
    private void OnDisable()
    {

        // Reward Video Events
        IronSourceRewardedVideoEvents.onAdOpenedEvent -= RewardedVideoOnAdOpenedEvent;
        IronSourceRewardedVideoEvents.onAdClosedEvent -= RewardedVideoOnAdClosedEvent;
        IronSourceRewardedVideoEvents.onAdAvailableEvent -= RewardedVideoOnAdAvailable;
        IronSourceRewardedVideoEvents.onAdUnavailableEvent -= RewardedVideoOnAdUnavailable;
        IronSourceRewardedVideoEvents.onAdShowFailedEvent -= RewardedVideoOnAdShowFailedEvent;
        IronSourceRewardedVideoEvents.onAdRewardedEvent -= RewardedVideoOnAdRewardedEvent;
        IronSourceRewardedVideoEvents.onAdClickedEvent -= RewardedVideoOnAdClickedEvent;
    }

    void SdkInitializationCompletedEvent(LevelPlayConfiguration config)
    {
        Debug.Log("unity-script: I got SdkInitializationCompletedEvent with config: " + config);
        /*IronSource.Agent.launchTestSuite();*/
    }

    void SdkInitializationFailedEvent(LevelPlayInitError error)
    {
        Debug.Log("unity-script: I got SdkInitializationFailedEvent with error: " + error);
    }

    public void WatchRewardedAds(System.Action callback)
    {
        if (IronSource.Agent.isRewardedVideoAvailable())
        {
            onRewardedVideoAdWatched = callback;
            IronSource.Agent.showRewardedVideo();
            LoadReward();
        }
        else
        {
            Debug.Log("IronSource reward not available");
            LoadReward();
        }
    }

    #region Reward

    public void LoadReward()
    {
        IronSource.Agent.loadRewardedVideo();
    }

    void RewardedVideoOnAdAvailable(IronSourceAdInfo adInfo)
    {
        skipActivityAdsButton.interactable = true;
        Debug.Log("IronSource reward available");
    }

    void RewardedVideoOnAdUnavailable()
    {
        skipActivityAdsButton.interactable = false;
        Debug.Log("IronSource reward unavailable");
    }

    void RewardedVideoOnAdOpenedEvent(IronSourceAdInfo adInfo)
    {
    }

    void RewardedVideoOnAdClosedEvent(IronSourceAdInfo adInfo)
    {
    }

    void RewardedVideoOnAdRewardedEvent(IronSourcePlacement placement, IronSourceAdInfo adInfo)
    {
        onRewardedVideoAdWatched?.Invoke();
        Debug.Log("Rewarded video worked");
    }

    void RewardedVideoOnAdShowFailedEvent(IronSourceError error, IronSourceAdInfo adInfo)
    {
    }

    void RewardedVideoOnAdClickedEvent(IronSourcePlacement placement, IronSourceAdInfo adInfo)
    {
    }


    #endregion

    private string GetUserId()
    {
        return SystemInfo.deviceUniqueIdentifier;
    }
}
