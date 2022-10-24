using UnityEngine;
using Firebase.Analytics;

public class FirebaseAnalyticsManager : MonoBehaviour
{
    public FirebaseAnalyticsManager Initialize()
    {
        return this;
    }

    public void Show(string panelName)
    {
        FirebaseAnalytics.LogEvent("Showed", "Panel", panelName);
    }

    public void Click(string panelName, string context)
    {
        FirebaseAnalytics.LogEvent("Clicked", "Button", context);
    }

    public void Progression(string progressionType, object data)
    {
        string dataRaw = JsonUtility.ToJson(data, true);
        FirebaseAnalytics.LogEvent("Progression", progressionType, dataRaw);
    }

    public void Ads(string adType, string process)
    {
        FirebaseAnalytics.LogEvent("Ads", adType, process);
    }

    public void Iap(string id)
    {
        FirebaseAnalytics.LogEvent("IAP", "boughted", id);
    }
}