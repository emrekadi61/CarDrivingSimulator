using System.Collections.Generic;
using UnityEngine;
using Utils.Data;

[System.Serializable]
public class User
{
    public string id;
    public string username;
    public bool boughtedNoAds;
    public GameData gameData;

    [System.Serializable]
    public class GameData
    {
        public int dollar;
        public int diamond;
        public Plate plate;
        public int carIndex;
        public GameSettings settings;
        public UserStatistics statistics;
        public List<CarData> cars = new List<CarData>();
    }

    [System.Serializable]
    public class GameSettings
    {
        public int qualityLevel;
        public int controllerType;
        public float musicLevel;
        public float fxLevel;
        public bool haptic;
    }

    [System.Serializable]
    public class UserStatistics
    {
        public int experience;
        public float trip;
        public float highSpeedTrip;
        public int drift;
        public int bestDrift;
    }

    [System.Serializable]
    public class Plate
    {
        public string text;
        public string textColor;
        public string baseColor;
    }

    public void Save()
    {
        JsonUtility.ToJson(this, true).Save("_data/user");
    }

    public User Deserialize()
    {
        return JsonUtility.FromJson<User>("_data/user".Get());
    }
}