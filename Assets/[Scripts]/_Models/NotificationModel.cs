using System;
using System.Collections.Generic;
using UnityEngine;
using Utils.Data;

[System.Serializable]
public class Notifications
{
    public string version;
    public List<Notification> notifications = new List<Notification>();
    private string path = "_data/retention-notifications";

    public void Save()
    {
        JsonUtility.ToJson(this, true).Save(path);
    }

    public Notifications Deserialize()
    {
        string currentRaw = path.Get();
        string incomeRaw = GameManager.Instance.sdkManager?.firebase?.remoteConfig?.retention_notifications;

        Notifications data;

        if (!string.IsNullOrEmpty(incomeRaw) && incomeRaw != currentRaw)
        {
            data = JsonUtility.FromJson<Notifications>(incomeRaw);

            if (data == null)
            {
                data = JsonUtility.FromJson<Notifications>(currentRaw);
            }
        }
        else
        {
            data = JsonUtility.FromJson<Notifications>(currentRaw);
        }

        return data;
    }

    #region VersionBase
    // public Notifications Deserialize()
    // {
    //     Notifications notificationsCurrent = JsonUtility.FromJson<Notifications>(path.Get());
    //     string income = CheckAndMigrate(notificationsCurrent.version);

    //     if (income != null)
    //     {
    //         notificationsCurrent = JsonUtility.FromJson<Notifications>(income);
    //         this.version = notificationsCurrent.version;
    //         this.notifications = notificationsCurrent.notifications;
    //         this.Save();
    //     }

    //     return notificationsCurrent;
    // }

    // public string CheckAndMigrate(string version)
    // {
    //     string raw = GameManager.Instance.sdkManager?.firebase?.remoteConfig?.retention_notifications;
    //     if (string.IsNullOrEmpty(raw)) return null;
    //     Notifications notificationsIncome = JsonUtility.FromJson<Notifications>(raw);

    //     if (notificationsIncome.version != version) return raw;

    //     return null;
    // }
    #endregion

    public string GetRaw()
    {
        return path.Get();
    }
}

[System.Serializable]
public class Notification
{
    public string title;
    public string body;
    public string icon;
    public int hours;
    public int minutes;
    public int seconds;
    public DateTime? deliveryTime;

    public Notification(string title, string body, string icon = null, int hours = -1, int minutes = -1, int seconds = -1)
    {
        this.title = title;
        this.body = body;
        this.icon = icon;
        this.hours = hours;
        this.minutes = minutes;
        this.seconds = seconds;
    }

    public Notification(string title, string body, int hours = -1, int minutes = -1, int seconds = -1)
    {
        this.title = title;
        this.body = body;
        this.hours = hours;
        this.minutes = minutes;
        this.seconds = seconds;
    }

    public Notification(string title, string body, string icon = null, DateTime? deliveryTime = null)
    {
        this.title = title;
        this.body = body;
        this.icon = icon;
        this.deliveryTime = deliveryTime;
    }

    public Notification(string title, string body, DateTime? deliveryTime = null)
    {
        this.title = title;
        this.body = body;
        this.deliveryTime = deliveryTime;
    }
}
