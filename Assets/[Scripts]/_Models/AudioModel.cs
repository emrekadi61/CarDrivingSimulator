using UnityEngine;
using System.Collections.Generic;
using Utils.Data;

[System.Serializable]
public class Audios
{
    public string version;
    public List<Audio> sounds = new List<Audio>();
    private string path = "_data/sounds";

    public void Save()
    {
        JsonUtility.ToJson(this, true).Save(path);
    }

    public Audios Deserialize()
    {
        string currentRaw = path.Get();
        string incomeRaw = GameManager.Instance.sdkManager?.firebase?.remoteConfig?.sounds;

        Audios data;

        if (!string.IsNullOrEmpty(incomeRaw) && incomeRaw != currentRaw)
        {
            data = JsonUtility.FromJson<Audios>(incomeRaw);

            if (data == null)
            {
                data = JsonUtility.FromJson<Audios>(currentRaw);
            }
        }
        else
        {
            data = JsonUtility.FromJson<Audios>(currentRaw);
        }

        return data;
    }

    #region VersionBase
    // public Audios Deserialize()
    // {
    //     Audios audiosCurrent = JsonUtility.FromJson<Audios>(path.Get());
    //     string income = CheckAndMigrate(audiosCurrent.version);

    //     if (income != null)
    //     {
    //         audiosCurrent = JsonUtility.FromJson<Audios>(income);
    //         this.version = audiosCurrent.version;
    //         this.sounds = audiosCurrent.sounds;
    //         this.Save();
    //     }

    //     return audiosCurrent;
    // }

    // public string CheckAndMigrate(string version)
    // {
    //     string raw = GameManager.Instance.sdkManager?.firebase?.remoteConfig?.sounds;
    //     if (string.IsNullOrEmpty(raw)) return null;
    //     Audios audiosIncome = JsonUtility.FromJson<Audios>(raw);

    //     if (audiosIncome.version != version) return raw;

    //     return null;
    // }
    #endregion

    public string GetRaw()
    {
        return path.Get();
    }
}

[System.Serializable]
public class Audio
{
    public string name;
    public string prefabName;
    public int type;
    public bool isLoop;
}
