using UnityEngine;
using System.Collections.Generic;
using Utils.Data;

[System.Serializable]
public class DailyRewards
{
    public string version;
    public List<int> rewards = new List<int>();
    private string path = "_data/daily-rewards";

    public void Save()
    {
        JsonUtility.ToJson(this, true).Save(path);
    }

    public DailyRewards Deserialize()
    {
        string currentRaw = path.Get();
        string incomeRaw = GameManager.Instance.sdkManager?.firebase?.remoteConfig?.daily_rewards;

        DailyRewards data;

        if (!string.IsNullOrEmpty(incomeRaw) && incomeRaw != currentRaw)
        {
            data = JsonUtility.FromJson<DailyRewards>(incomeRaw);

            if (data == null)
            {
                data = JsonUtility.FromJson<DailyRewards>(currentRaw);
            }
        }
        else
        {
            data = JsonUtility.FromJson<DailyRewards>(currentRaw);
        }

        return data;
    }

    #region VersionBase
    // public DailyRewards Deserialize()
    // {
    //     DailyRewards dailyRewardsCurrent = JsonUtility.FromJson<DailyRewards>(path.Get());
    //     string income = CheckAndMigrate(dailyRewardsCurrent.version);

    //     if (income != null)
    //     {
    //         dailyRewardsCurrent = JsonUtility.FromJson<DailyRewards>(income);
    //         this.version = dailyRewardsCurrent.version;
    //         this.rewards = dailyRewardsCurrent.rewards;
    //         this.Save();
    //     }

    //     return dailyRewardsCurrent;
    // }

    // public string CheckAndMigrate(string version)
    // {
    //     string raw = GameManager.Instance.sdkManager?.firebase?.remoteConfig?.daily_rewards;
    //     if (string.IsNullOrEmpty(raw)) return null;
    //     DailyRewards dailyRewardsIncome = JsonUtility.FromJson<DailyRewards>(raw);

    //     if (dailyRewardsIncome.version != version) return raw;

    //     return null;
    // }
    #endregion

    public string GetRaw()
    {
        return path.Get();
    }
}
