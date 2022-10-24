using System;
using UnityEngine;
using Unity.Notifications.Android;

public class NotificationManager : ManagerBase
{
    public void SetNotification(Notification notification)
    {
        AndroidNotificationChannel anc = new AndroidNotificationChannel()
        {
            Id = Application.identifier,
            Name = Application.productName + " Channel",
            Importance = Importance.High,
            Description = "Sports Car Sim Notification Channel Description",
            CanShowBadge = true
        };
        AndroidNotificationCenter.RegisterNotificationChannel(anc);

        DateTime? deliverTime = null;

        if (notification.deliveryTime != null) deliverTime = notification.deliveryTime;
        else if (notification.hours != -1) deliverTime = DateTime.Now.AddHours((double)notification.hours);
        else if (notification.minutes != -1) deliverTime = DateTime.Now.AddMinutes((double)notification.minutes);
        else if (notification.seconds != -1) deliverTime = DateTime.Now.AddSeconds((double)notification.seconds);

        AndroidNotification an = new AndroidNotification()
        {
            Title = notification.title,
            Text = notification.body,
            SmallIcon = "game_icon",
            LargeIcon = notification.icon,
            FireTime = deliverTime ?? DateTime.Now,
        };

        AndroidNotificationCenter.SendNotification(an, anc.Id);
    }

    public void Clear()
    {
        AndroidNotificationCenter.CancelAllNotifications();
        Debug.Log("All notifications cancelled");
    }
}