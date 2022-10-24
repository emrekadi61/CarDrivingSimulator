using System;
using UnityEngine;

public class LuckyWheelSystem : MonoBehaviour
{
    private string rewardHourString;
    private readonly string REWARD_HOUR_DATA = "lucky-wheel-reward-hour";
    private DateTime rewardTime;
    public TimeSpan timeLeft { get { return (rewardTime - DateTime.Now); } }

    private void Start()
    {
        rewardHourString = PlayerPrefs.GetString(REWARD_HOUR_DATA, "none");

        if (rewardHourString == "none")
        {
            rewardHourString = DateTime.Now.AddHours(4).ToString();
            PlayerPrefs.SetString(REWARD_HOUR_DATA, rewardHourString);
            PlayerPrefs.Save();
        }

        rewardTime = DateTime.Parse(rewardHourString);
    }

    public void OnPanelEnd()
    {
        rewardTime = DateTime.Now.AddHours(4);
        PlayerPrefs.SetString(REWARD_HOUR_DATA, rewardTime.ToString());
    }

    private void OnApplicationPause(bool pauseStatus)
    {
        if (!pauseStatus || rewardTime == null) return;
        if (rewardTime < DateTime.Now) return;
        
        GameManager
        .Instance
        .sdkManager?
        .notificationManager?
        .SetNotification(
            new Notification(
                "Lucky Time!",
                "Don't Miss Chance, come and get your reward",
                "lucky_wheel",
                rewardTime
        ));
    }
}