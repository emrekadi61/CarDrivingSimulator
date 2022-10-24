using UnityEngine;
using UnityEngine.Events;
using Firebase;
using Firebase.Extensions;

public class FirebaseCore : ManagerBase
{
    [HideInInspector] public FirebaseAnalyticsManager analytics;
    [HideInInspector] public FirebaseRemoteConfig remoteConfig;
    [HideInInspector] public FirebaseCloudMessaging messaging;

    [HideInInspector] public bool initialized;

    private UnityAction onInitialized;

    public void Construct(UnityAction onInitialized)
    {
        this.onInitialized = onInitialized;
        Initialize();
    }

    private void Initialize()
    {
        Firebase
        .FirebaseApp
        .CheckAndFixDependenciesAsync()
        .ContinueWithOnMainThread(task =>
        {
            if (task.Result == DependencyStatus.Available)
            {
                remoteConfig = gameObject.AddComponent<FirebaseRemoteConfig>().Initialize(this.onInitialized);
                analytics = gameObject.AddComponent<FirebaseAnalyticsManager>().Initialize();
                messaging = gameObject.AddComponent<FirebaseCloudMessaging>().Initialize(() => Destroy(messaging));
            }
            else
            {
                OnInitialized();
            }
        });
    }

    private void OnInitialized()
    {
        initialized = true;
        return;
    }
}