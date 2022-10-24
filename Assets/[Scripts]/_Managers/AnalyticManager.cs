public class AnalyticManager : ManagerBase
{
    private void Awake() => base.onConstructed.AddListener(this.OnConstructed);

    private void OnConstructed()
    {
        base.onConstructed.RemoveListener(this.OnConstructed);
    }

    public void Showed(string panelName)
    {
        GameManager.Instance.sdkManager?.firebase?.analytics?.Show(panelName);
    }

    public void Clicked(string panelName, string buttonName)
    {
        GameManager.Instance.sdkManager?.firebase?.analytics?.Click(panelName, buttonName);
    }

    public void Progression(string type, object[] data)
    {
        GameManager.Instance.sdkManager?.firebase?.analytics?.Progression(type, data);
    }

    public void Ads(string adType, string process)
    {
        GameManager.Instance.sdkManager?.firebase?.analytics?.Ads(adType, process);
    }

    public void IAP(string id)
    {
        GameManager.Instance.sdkManager?.firebase?.analytics?.Iap(id);
    }
}