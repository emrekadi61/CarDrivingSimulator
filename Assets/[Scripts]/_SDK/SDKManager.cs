using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class SDKManager : ManagerBase
{
    [HideInInspector] public FirebaseCore firebase;
    [HideInInspector] public GPGSManager gpgManager;
    [HideInInspector] public IAPManager iapManager;
    [HideInInspector] public NotificationManager notificationManager;
    [HideInInspector] public AdManagerMax adManager;
    [HideInInspector] public AdManagerInGame adManagerInGame;

    [HideInInspector] public DateTime luckyWheelTime;

    private void Start()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    public void ConstructFirebase(UnityAction onInitialized)
    {
        firebase = gameObject.AddComponent<FirebaseCore>().Construct().GetComponent<FirebaseCore>();
        firebase.Construct(onInitialized);
    }

    public void ConstructOtherSDKs()
    {
        gpgManager = gameObject.AddComponent<GPGSManager>().Construct().GetComponent<GPGSManager>();
        iapManager = gameObject.AddComponent<IAPManager>().Construct().GetComponent<IAPManager>();
        notificationManager = gameObject.AddComponent<NotificationManager>().Construct().GetComponent<NotificationManager>();
        adManager = gameObject.AddComponent<AdManagerMax>().Construct().GetComponent<AdManagerMax>();
        adManager.SetBanner(GameManager.Instance.statics.bannerEnable);
        notificationManager.Clear();
    }

    public void OnBoughtedNoAds()
    {
        adManager.SetBanner(false);
        GameManager.Instance.dataManager.user.boughtedNoAds = true;
        GameManager.Instance.dataManager.SaveUser();
        if (adManagerInGame) Destroy(adManagerInGame);
    }

    private void OnApplicationPause(bool pauseStatus)
    {
        if (!pauseStatus)
        {
            notificationManager?.Clear();
            return;
        }

        List<Notification> notifications = new Notifications().Deserialize().notifications;
        for (int i = 0; i < notifications.Count; i++) notificationManager?.SetNotification(notifications[i]);
        notifications?.Clear();
    }

    private void OnSceneLoaded(Scene scens, LoadSceneMode mode)
    {
        if (GameManager.Instance.currentScene.Contains("game") && !GameManager.Instance.dataManager.user.boughtedNoAds)
        {
            if (!adManagerInGame && GameManager.Instance.statics.inGameAdsEnable)
                adManagerInGame = gameObject.AddComponent<AdManagerInGame>().Construct(GameManager.Instance.statics.interstitialFrequence);
        }
        else
        {
            if (adManagerInGame)
            {
                Destroy(adManagerInGame);
                adManagerInGame = null;
            }
        }
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}