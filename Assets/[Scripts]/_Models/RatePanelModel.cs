using UnityEngine;
using System.Collections.Generic;
using Utils.Data;

[System.Serializable]
public class RatePanelModel
{
    public string version;
    public List<int> levels = new List<int>();
    public List<RatePanelData> datas = new List<RatePanelData>();
    private string path = "_data/rate-panel";

    public void Save()
    {
        JsonUtility.ToJson(this, true).Save(path);
    }

    public RatePanelModel Deserialize()
    {
        string currentRaw = path.Get();
        string incomeRaw = GameManager.Instance.sdkManager?.firebase?.remoteConfig?.rate_panel;

        RatePanelModel data;

        if (!string.IsNullOrEmpty(incomeRaw) && incomeRaw != currentRaw)
        {
            data = JsonUtility.FromJson<RatePanelModel>(incomeRaw);

            if (data == null)
            {
                data = JsonUtility.FromJson<RatePanelModel>(currentRaw);
            }
        }
        else
        {
            data = JsonUtility.FromJson<RatePanelModel>(currentRaw);
        }

        return data;
    }

    #region VersionBase
    // public RatePanelModel Deserialize()
    // {
    //     RatePanelModel rpCurrent = JsonUtility.FromJson<RatePanelModel>(path.Get());
    //     string income = CheckAndMigrate(rpCurrent.version);

    //     if (income != null)
    //     {
    //         rpCurrent = JsonUtility.FromJson<RatePanelModel>(income);
    //         this.version = rpCurrent.version;
    //         this.levels = rpCurrent.levels;
    //         this.datas = rpCurrent.datas;
    //         this.Save();
    //     }

    //     return rpCurrent;
    // }

    // public string CheckAndMigrate(string version)
    // {
    //     string raw = GameManager.Instance.sdkManager?.firebase?.remoteConfig?.rate_panel;
    //     if (string.IsNullOrEmpty(raw)) return null;
    //     RatePanelModel rpIncome = JsonUtility.FromJson<RatePanelModel>(raw);

    //     if (rpIncome.version != version) return raw;

    //     return null;
    // }
    #endregion

    public string GetRaw()
    {
        return path.Get();
    }
}

[System.Serializable]
public class RatePanelData
{
    public string lang;
    public RateMain main;
    public MiniSuvey survey;
}

[System.Serializable]
public class RateMain
{
    public string titleText;
    public string bodyText;
    public string yesButtonText;
    public string noButtonText;
    public string laterButtonText;
}

[System.Serializable]
public class MiniSuvey
{
    public string titleText;
    public string yesButtonText;
    public string noButtonText;
}