using UnityEngine;
using System.Collections.Generic;
using Utils.Data;

[System.Serializable]
public class Cars
{
    public string version;
    public List<CarData> cars = new List<CarData>();
    private string path = "_data/cars";

    public void Save()
    {
        JsonUtility.ToJson(this, true).Save(path);
    }

    public Cars Deserialize()
    {
        string currentRaw = path.Get();
        string incomeRaw = GameManager.Instance.sdkManager?.firebase?.remoteConfig?.cars;

        if (GameManager.Instance.readFromLocal) incomeRaw = null;

        Cars data;

        if (!string.IsNullOrEmpty(incomeRaw) && incomeRaw != currentRaw)
        {
            data = JsonUtility.FromJson<Cars>(incomeRaw);

            if (data == null)
            {
                data = JsonUtility.FromJson<Cars>(currentRaw);
            }
        }
        else
        {
            data = JsonUtility.FromJson<Cars>(currentRaw);
        }

        List<CarData> indexes = new List<CarData>();
        for (int i = 0; i < data.cars.Count; i++)
        {
            GameObject g = Resources.Load<GameObject>("_cars/" + data.cars[i].prefabName);
            if (g == null) indexes.Add(data.cars[i]);
        }

        for (int i = 0; i < indexes.Count; i++) data.cars.Remove(indexes[i]);

        return data;
    }

    #region VersionBase
    // public Cars Deserialize()
    // {
    //     Cars carsCurrent = JsonUtility.FromJson<Cars>(path.Get());
    //     string income = CheckAndMigrate(carsCurrent.version);

    //     if (income != null)
    //     {
    //         carsCurrent = JsonUtility.FromJson<Cars>(income);
    //         this.version = carsCurrent.version;
    //         this.cars = carsCurrent.cars;
    //         this.Save();
    //     }

    //     return carsCurrent;
    // }

    // public string CheckAndMigrate(string version)
    // {
    //     string raw = GameManager.Instance.sdkManager?.firebase?.remoteConfig?.cars;
    //     if (string.IsNullOrEmpty(raw)) return null;
    //     Cars carsIncome = JsonUtility.FromJson<Cars>(raw);

    //     if (carsIncome.version != version) return raw;

    //     return null;
    // }
    #endregion

    public string GetRaw()
    {
        return path.Get();
    }
}

[System.Serializable]
public class CarData
{
    public string id;
    public string brand;
    public string model;
    public string chasisName;
    public string prefabName;
    public float pointFactor;
    public Price price;
    public Costs costs;
    public int rimIndex;
    public Axles axles;
    public Upgrades upgrades;
    public UpgradeIncrements upgradeIncrements;
    public CarSpecifications specifications;
    public VisualSpecifications visual;
}

[System.Serializable]
public class Axles
{
    public Axle front;
    public Axle rear;

    public Axles(Axles axles)
    {
        this.front = new Axle(axles.front);
        this.rear = new Axle(axles.rear);
    }
}

[System.Serializable]
public class Axle
{
    public float suspensionDistance;
    public float wheelCamber;
    public float wheelOffset;

    public Axle(Axle axle)
    {
        this.suspensionDistance = axle.suspensionDistance;
        this.wheelCamber = axle.wheelCamber;
        this.wheelOffset = axle.wheelOffset;
    }
}

[System.Serializable]
public class Upgrades
{
    public int engine;
    public int transmission;
    public int steering;
    public int brakes;
}

[System.Serializable]
public class UpgradeIncrements
{
    public float engine;
    public float transmission;
    public float steering;
    public float brakes;
}

[System.Serializable]
public class Costs
{
    public Price engine;
    public Price transmission;
    public Price steering;
    public Price brakes;
    public Price body;
    public Price rim;
    public Price window;
    public Price suspension;
}

[System.Serializable]
public class CarSpecifications
{
    public float engineTorque;
    public float transmission;
    public float steering;
    public float brakeTorque;
}

[System.Serializable]
public class VisualSpecifications
{
    public Paint body;
    public Paint bodyDetail;
    public Paint rim;
    public Paint window;
    public string texturePath;
    public List<CarDecal> decals = new List<CarDecal>();
}

[System.Serializable]
public class Paint
{
    public string color;
    public float tint;
}

[System.Serializable]
public class CarDecal
{
    public string decalPath;
    public UnityEngine.Vector3 position;
    public UnityEngine.Vector3 rotation;
}

[System.Serializable]
public class Price
{
    public int type;
    public int amount;

    public Price() { }

    public Price(int type, int amount)
    {
        this.type = type;
        this.amount = amount;
    }

    public Price(Price val)
    {
        this.type = val.type;
        this.amount = val.amount;
    }
}