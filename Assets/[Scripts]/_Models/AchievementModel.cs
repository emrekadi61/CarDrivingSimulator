using System.Collections.Generic;
using UnityEngine;
using Utils.Data;

[System.Serializable]
public class Achievements
{
    public string version;
    public List<Achievement> achievements = new List<Achievement>();
    private string path = "_data/achievements";

    public void Save()
    {
        JsonUtility.ToJson(this, true).Save(path);
    }

    public Achievements Deserialize()
    {
        string current = path.Get();
        string income = GameManager.Instance.sdkManager?.firebase?.remoteConfig?.achievements;

        Achievements acCurrent;

        if (!string.IsNullOrEmpty(income) && income != current)
        {
            acCurrent = JsonUtility.FromJson<Achievements>(current);
            Achievements acIncome = JsonUtility.FromJson<Achievements>(income);

            if (income != null)
            {
                for (int i = 0; i < acIncome.achievements.Count; i++)
                    acIncome.achievements[i].completed = acCurrent.achievements[i].completed;

                acCurrent = acIncome;
                acCurrent.Save();

                return acCurrent;
            }
        }

        acCurrent = JsonUtility.FromJson<Achievements>(current);

        return acCurrent;
    }
    
    #region VersionBase
    // public Achievements Deserialize()
    // {
    //     Achievements achievementsCurrent = JsonUtility.FromJson<Achievements>(path.Get());
    //     string income = CheckAndMigrate(achievementsCurrent.version);

    //     if (income != null)
    //     {
    //         Achievements achievementsIncome = JsonUtility.FromJson<Achievements>(income);

    //         for (int i = 0; i < achievementsIncome.achievements.Count; i++)
    //             achievementsIncome.achievements[i].completed = achievementsCurrent.achievements[i].completed;

    //         achievementsCurrent.version = achievementsIncome.version;
    //         achievementsCurrent.achievements = achievementsIncome.achievements;
    //         this.Save();
    //     }

    //     return achievementsCurrent;
    // }

    // public string CheckAndMigrate(string version)
    // {
    //     string raw = GameManager.Instance.sdkManager?.firebase?.remoteConfig?.achievements;
    //     if (string.IsNullOrEmpty(raw)) return null;
    //     Achievements achievementsIncome = JsonUtility.FromJson<Achievements>(raw);

    //     if (achievementsIncome.version != version) return raw;

    //     return null;
    // }
    #endregion

    public string GetRaw()
    {
        return path.Get();
    }
}

[System.Serializable]
public class Achievement
{
    public string id;
    public string name;
    public string description;
    public int type;
    public int target;
    public bool completed;
}

//  ||||||||||||||||||||||||||||||||
//  ||                            ||
//  ||   ACHIEVEMENT TYPE         ||
//  ||||||||||||||||||||||||||||||||
//  ||    0 => trip               ||
//  ||    1 => drift              ||
//  ||    2 => high speed trip    ||
//  ||||||||||||||||||||||||||||||||
