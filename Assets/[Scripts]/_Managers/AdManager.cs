using UnityEngine.Events;

public class AdManager : ManagerBase
{
    private void Awake() => base.onConstructed.AddListener(this.OnConstructed);

    private void OnConstructed()
    {
        base.onConstructed.RemoveListener(this.OnConstructed);
    }

    public void SetBanner(bool show)
    {
        GameManager.Instance.sdkManager.adManager.SetBanner(show);
    }

    public void ShowInterstitial(UnityAction onInterstitialEnd = null)
    {
        GameManager.Instance.sdkManager.adManager.ShowInterstitial(b => onInterstitialEnd?.Invoke());
    }

    public void ShowRewarded(UnityAction<bool> onRewardedEnd = null)
    {
        GameManager.Instance.sdkManager.adManager.ShowRewarded(onRewardedEnd);
    }
}