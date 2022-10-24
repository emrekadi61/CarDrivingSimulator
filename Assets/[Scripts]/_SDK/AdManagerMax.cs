using UnityEngine;
using UnityEngine.Events;

public class AdManagerMax : ManagerBase
{
    public string maxSDKKey = "7msACipWDNUHD5zxIRq5wz7hOD76ku7PEhIxWPgergsQNg7oSpTcGDiX_O0Rf2nOVx4S5DhcGRVL0UR_XCyJ85";
    public string bannerID = "687821ef0e3fc226";
    public string interstitialID = "e34fc5de1de74daa";
    public string rewardedID = "395a88f16e2fad4c";

    private void Awake()
    {
        base.onConstructed.AddListener(this.OnConstructed);
    }

    private void OnConstructed()
    {
        base.onConstructed.RemoveListener(this.OnConstructed);
        MaxSdk.SetSdkKey(maxSDKKey);
        MaxSdk.InitializeSdk();
        if (!GameManager.Instance.dataManager.user.boughtedNoAds)
        {
            InitializeBanner();
            InitializeInterstitial();
        }
        InitializeRewarded();
    }

    #region Banner
    public void SetBanner(bool show)
    {
        if (!GameManager.Instance.dataManager.user.boughtedNoAds && show) MaxSdk.ShowBanner(bannerID);
        else MaxSdk.HideBanner(bannerID);
        GameManager.Instance.analyticManager.Ads("banner", show ? "show" : "hide");
    }

    private void InitializeBanner()
    {
        MaxSdk.CreateBanner(bannerID, MaxSdkBase.BannerPosition.BottomCenter);
        MaxSdk.SetBannerBackgroundColor(bannerID, Color.black);

        MaxSdkCallbacks.Banner.OnAdLoadedEvent += OnBannerAdLoadedEvent;
        MaxSdkCallbacks.Banner.OnAdLoadFailedEvent += OnBannerAdLoadFailedEvent;
        MaxSdkCallbacks.Banner.OnAdClickedEvent += OnBannerAdClickedEvent;
        MaxSdkCallbacks.Banner.OnAdRevenuePaidEvent += OnBannerAdRevenuePaidEvent;
        MaxSdkCallbacks.Banner.OnAdExpandedEvent += OnBannerAdExpandedEvent;
        MaxSdkCallbacks.Banner.OnAdCollapsedEvent += OnBannerAdCollapsedEvent;
    }

    private void OnBannerAdLoadedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo) { }

    private void OnBannerAdLoadFailedEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo) { }

    private void OnBannerAdClickedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo) { }

    private void OnBannerAdRevenuePaidEvent(string adUnitId, MaxSdkBase.AdInfo adInfo) { }

    private void OnBannerAdExpandedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo) { }

    private void OnBannerAdCollapsedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo) { }
    #endregion

    #region Interstitial
    private int retryAttemptInterstitial;
    private UnityAction<bool> onCompleteInterstitial;

    public void ShowInterstitial(UnityAction<bool> onCompleteInterstitial = null)
    {
        if (GameManager.Instance.dataManager.user.boughtedNoAds) return;

        this.onCompleteInterstitial = onCompleteInterstitial;
        if (MaxSdk.IsInterstitialReady(interstitialID))
        {
            MaxSdk.ShowInterstitial(interstitialID);
        }
        else
        {
            this.onCompleteInterstitial?.Invoke(false);
            this.onCompleteInterstitial = null;
        }
    }

    private void InitializeInterstitial()
    {
        MaxSdkCallbacks.Interstitial.OnAdLoadedEvent += OnInterstitialLoadedEvent;
        MaxSdkCallbacks.Interstitial.OnAdLoadFailedEvent += OnInterstitialLoadFailedEvent;
        MaxSdkCallbacks.Interstitial.OnAdDisplayedEvent += OnInterstitialDisplayedEvent;
        MaxSdkCallbacks.Interstitial.OnAdClickedEvent += OnInterstitialClickedEvent;
        MaxSdkCallbacks.Interstitial.OnAdHiddenEvent += OnInterstitialHiddenEvent;
        MaxSdkCallbacks.Interstitial.OnAdDisplayFailedEvent += OnInterstitialAdFailedToDisplayEvent;

        LoadInterstitial();
    }

    private void LoadInterstitial()
    {
        MaxSdk.LoadInterstitial(interstitialID);
    }

    private void OnInterstitialLoadedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        retryAttemptInterstitial = 0;
    }

    private void OnInterstitialLoadFailedEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo)
    {
        retryAttemptInterstitial++;
        double retryDelay = Mathf.Pow(2, Mathf.Min(6, retryAttemptInterstitial));
        Invoke("LoadInterstitial", (float)retryDelay);
    }

    private void OnInterstitialDisplayedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        GameManager.Instance.analyticManager.Ads("interstitial", "showed");
    }

    private void OnInterstitialAdFailedToDisplayEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo, MaxSdkBase.AdInfo adInfo)
    {
        LoadInterstitial();
        onCompleteInterstitial?.Invoke(false);
        this.onCompleteInterstitial = null;
    }

    private void OnInterstitialClickedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {

    }

    private void OnInterstitialHiddenEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        LoadInterstitial();
        onCompleteInterstitial?.Invoke(true);
        this.onCompleteInterstitial = null;
    }
    #endregion

    #region Rewarded
    private int retryAttemptRewarded;
    private bool deservedReward;
    private UnityAction<bool> onCompletedRewarded;

    public void ShowRewarded(UnityAction<bool> onCompletedRewarded)
    {
        this.onCompletedRewarded = onCompletedRewarded;
        deservedReward = false;

        if (MaxSdk.IsRewardedAdReady(rewardedID))
        {
            MaxSdk.ShowRewardedAd(rewardedID);
        }
        else
        {
            this.onCompletedRewarded?.Invoke(deservedReward);
            this.onCompletedRewarded = null;
            NotificationPanel.Get("NO REWARDED ADS AVAILABLE");
        }
    }

    private void InitializeRewarded()
    {
        // Attach callback
        MaxSdkCallbacks.Rewarded.OnAdLoadedEvent += OnRewardedAdLoadedEvent;
        MaxSdkCallbacks.Rewarded.OnAdLoadFailedEvent += OnRewardedAdLoadFailedEvent;
        MaxSdkCallbacks.Rewarded.OnAdDisplayedEvent += OnRewardedAdDisplayedEvent;
        MaxSdkCallbacks.Rewarded.OnAdClickedEvent += OnRewardedAdClickedEvent;
        MaxSdkCallbacks.Rewarded.OnAdRevenuePaidEvent += OnRewardedAdRevenuePaidEvent;
        MaxSdkCallbacks.Rewarded.OnAdHiddenEvent += OnRewardedAdHiddenEvent;
        MaxSdkCallbacks.Rewarded.OnAdDisplayFailedEvent += OnRewardedAdFailedToDisplayEvent;
        MaxSdkCallbacks.Rewarded.OnAdReceivedRewardEvent += OnRewardedAdReceivedRewardEvent;

        // Load the first rewarded ad
        LoadRewardedAd();
    }

    private void LoadRewardedAd()
    {
        MaxSdk.LoadRewardedAd(rewardedID);
    }

    private void OnRewardedAdLoadedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        retryAttemptRewarded = 0;
    }

    private void OnRewardedAdLoadFailedEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo)
    {
        retryAttemptRewarded++;
        double retryDelay = Mathf.Pow(2, Mathf.Min(6, retryAttemptRewarded));

        Invoke("LoadRewardedAd", (float)retryDelay);
    }

    private void OnRewardedAdDisplayedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        GameManager.Instance.analyticManager.Ads("rewarded", "showed");
    }

    private void OnRewardedAdFailedToDisplayEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo, MaxSdkBase.AdInfo adInfo)
    {
        LoadRewardedAd();
        onCompletedRewarded?.Invoke(deservedReward);
        onCompletedRewarded = null;
    }

    private void OnRewardedAdClickedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {

    }

    private void OnRewardedAdHiddenEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        onCompletedRewarded?.Invoke(deservedReward);
        onCompletedRewarded = null;
        LoadRewardedAd();
    }

    private void OnRewardedAdReceivedRewardEvent(string adUnitId, MaxSdk.Reward reward, MaxSdkBase.AdInfo adInfo)
    {
        deservedReward = true;
    }

    private void OnRewardedAdRevenuePaidEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {

    }
    #endregion
}