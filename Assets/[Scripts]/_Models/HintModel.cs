using UnityEngine;
using System.Collections.Generic;
using Utils.Data;

public class HintModel
{
    public string version;
    public List<string> hints = new List<string>();
    private string path = "_data/hints";

    public void Save()
    {
        JsonUtility.ToJson(this, true).Save(path);
    }

    public HintModel Deserialize()
    {
        string currentRaw = path.Get();
        string incomeRaw = GameManager.Instance.sdkManager?.firebase?.remoteConfig?.hints;

        HintModel data;

        if (!string.IsNullOrEmpty(incomeRaw) && incomeRaw != currentRaw)
        {
            data = JsonUtility.FromJson<HintModel>(incomeRaw);

            if (data == null)
            {
                data = JsonUtility.FromJson<HintModel>(currentRaw);
            }
        }
        else
        {
            data = JsonUtility.FromJson<HintModel>(currentRaw);
        }

        return data;
    }

    #region VersionBase
    //    public HintModel Deserialize()
    //     {
    //         HintModel hintsCurrent = JsonUtility.FromJson<HintModel>(path.Get());
    //         string income = CheckAndMigrate(hintsCurrent.version);

    //         if (income != null)
    //         {
    //             hintsCurrent = JsonUtility.FromJson<HintModel>(income);
    //             this.version = hintsCurrent.version;
    //             this.hints = hintsCurrent.hints;
    //             this.Save();
    //         }

    //         return hintsCurrent;
    //     }

    //     public string CheckAndMigrate(string version)
    //     {
    //         string raw = GameManager.Instance.sdkManager?.firebase?.remoteConfig?.hints;
    //         if (string.IsNullOrEmpty(raw)) return null;
    //         HintModel hintsIncome = JsonUtility.FromJson<HintModel>(raw);

    //         if (hintsIncome.version != version) return raw;

    //         return null;
    //     }
    #endregion

    public string GetRaw()
    {
        return path.Get();
    }
}