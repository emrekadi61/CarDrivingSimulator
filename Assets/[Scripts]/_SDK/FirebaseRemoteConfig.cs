using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Firebase.Extensions;

public class FirebaseRemoteConfig : MonoBehaviour
{
    [HideInInspector] public string cars;
    [HideInInspector] public string daily_rewards;
    [HideInInspector] public string game_static_variables;
    [HideInInspector] public string hints;
    [HideInInspector] public string iap;
    [HideInInspector] public string rate_panel;
    [HideInInspector] public string retention_notifications;
    [HideInInspector] public string sounds;
    [HideInInspector] public string achievements;

    private UnityAction onInitialized;

    public FirebaseRemoteConfig Initialize(UnityAction onInitialized)
    {
        this.onInitialized = onInitialized;

        Dictionary<string, object> defaults = new Dictionary<string, object>();

        defaults.Add("cars", new Cars().GetRaw());
        defaults.Add("daily_rewards", new DailyRewards().GetRaw());
        defaults.Add("game_static_variables", new GameStaticVariables().GetRaw());
        defaults.Add("hints", new HintModel().GetRaw());
        defaults.Add("iap", new IAPObjects().GetRaw());
        defaults.Add("rate_panel", new RatePanelModel().GetRaw());
        defaults.Add("retention_notifications", new Notifications().GetRaw());
        defaults.Add("sounds", new Audios().GetRaw());
        defaults.Add("achievements", new Achievements().GetRaw());

        Firebase
        .RemoteConfig
        .FirebaseRemoteConfig
        .DefaultInstance
        .SetDefaultsAsync(defaults)
        .ContinueWithOnMainThread(task =>
        {
            FetchDataAsync();
        });

        return this;
    }

    private Task FetchDataAsync()
    {
        Task fetchTask = Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.FetchAsync(TimeSpan.Zero);
        return fetchTask.ContinueWithOnMainThread(OnFetchCompleted);
    }

    private void OnFetchCompleted(Task fetchTask)
    {
        var info = Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.Info;
        if (info.LastFetchStatus == Firebase.RemoteConfig.LastFetchStatus.Success)
            Firebase
            .RemoteConfig
            .FirebaseRemoteConfig
            .DefaultInstance
            .ActivateAsync()
            .ContinueWithOnMainThread(task => OnInitialized());
        else
            OnInitialized();
    }

    private void OnInitialized()
    {
        cars = Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.GetValue("cars").StringValue;
        daily_rewards = Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.GetValue("daily_rewards").StringValue;
        game_static_variables = Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.GetValue("game_static_variables").StringValue;
        hints = Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.GetValue("hints").StringValue;
        iap = Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.GetValue("iap").StringValue;
        rate_panel = Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.GetValue("rate_panel").StringValue;
        retention_notifications = Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.GetValue("retention_notifications").StringValue;
        sounds = Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.GetValue("sounds").StringValue;
        achievements = Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.GetValue("achievements").StringValue;


        onInitialized?.Invoke();
    }
}