using System;
using UnityEngine;

public class DailyRewardSystem : MonoBehaviour
{
    public int activeDay;

    private string lastLoginString;
    private readonly string LAST_LOGIN_DATA = "daily-reward-last-login";
    private readonly string ACTIVE_DAY_DATA = "daily-reward-active-day";
    private DateTime? nextDayTime;

    private void Start()
    {
        lastLoginString = PlayerPrefs.GetString(LAST_LOGIN_DATA, "none");

        DateTime compareTime = DateTime.Now;
        float elapsedHours = (float)(lastLoginString == "none" ? -1 : (compareTime - DateTime.Parse(lastLoginString)).TotalHours);

        if (lastLoginString == "none" || elapsedHours > 48f)
        {
            activeDay = 0;
            SetPanel();
        }
        else if (elapsedHours >= 24f)
        {
            activeDay = PlayerPrefs.GetInt(ACTIVE_DAY_DATA, 0);
            SetPanel();
        }
    }

    private void SetPanel()
    {
        UIManager.Instance.Push(DailyRewardPanel.Get(activeDay, () =>
        {
            PlayerPrefs.SetString(LAST_LOGIN_DATA, DateTime.Now.ToString());
            PlayerPrefs.SetInt(ACTIVE_DAY_DATA, activeDay + 1);
            PlayerPrefs.Save();
        }));
    }

    public void OnEarnedReward()
    {
        nextDayTime = DateTime.Now.AddHours(24);
    }

    private void OnApplicationPause(bool pauseStatus)
    {
        if (!pauseStatus || nextDayTime == null) return;

        GameManager
        .Instance
        .sdkManager?
        .notificationManager?
        .SetNotification(
            new Notification(
                "Lucky Time!",
                "Don't Miss Chance, come and get your reward",
                "lucky_wheel",
                nextDayTime
        ));
    }
}