using System.Collections.Generic;
using UnityEngine;
using Utils.Data;

[System.Serializable]
public class IAPObjects
{
    public string version;
    public List<IAPObject> objects = new List<IAPObject>();
    private string path = "_data/iap";

    public void Save()
    {
        JsonUtility.ToJson(this, true).Save(path);
    }

    public IAPObjects Deserialize()
    {
        string currentRaw = path.Get();
        string incomeRaw = GameManager.Instance.sdkManager?.firebase?.remoteConfig?.iap;

        IAPObjects data;

        if (!string.IsNullOrEmpty(incomeRaw) && incomeRaw != currentRaw)
        {
            data = JsonUtility.FromJson<IAPObjects>(incomeRaw);

            if (data == null)
            {
                data = JsonUtility.FromJson<IAPObjects>(currentRaw);
            }
        }
        else
        {
            data = JsonUtility.FromJson<IAPObjects>(currentRaw);
        }

        return data;
    }

    #region VersionBase
    // public IAPObjects Deserialize()
    // {
    //     IAPObjects iapCurrent = JsonUtility.FromJson<IAPObjects>(path.Get());
    //     string income = CheckAndMigrate(iapCurrent.version);

    //     if (income != null)
    //     {
    //         iapCurrent = JsonUtility.FromJson<IAPObjects>(income);
    //         this.version = iapCurrent.version;
    //         this.objects = iapCurrent.objects;
    //         this.Save();
    //     }

    //     return iapCurrent;
    // }

    // public string CheckAndMigrate(string version)
    // {
    //     string raw = GameManager.Instance.sdkManager?.firebase?.remoteConfig?.iap;
    //     if (string.IsNullOrEmpty(raw)) return null;
    //     IAPObjects iapIncome = JsonUtility.FromJson<IAPObjects>(raw);

    //     if (iapIncome.version != version) return raw;

    //     return null;
    // }
    #endregion

    public string GetRaw()
    {
        return path.Get();
    }
}

[System.Serializable]
public class IAPObject
{
    public string id;
    public string title;
    public string icon;
    public Price price;
    public string description;
    public bool consumable;
}