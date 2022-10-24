using UnityEngine;
using Utils.Data;

[System.Serializable]
public class GameStaticVariables
{
    public string version;
    public Variables variables;
    private string path = "_data/game-static-variables";

    public void Save()
    {
        JsonUtility.ToJson(this, true).Save(path);
    }

    public GameStaticVariables Deserialize()
    {
        string currentRaw = path.Get();
        string incomeRaw = GameManager.Instance.sdkManager?.firebase?.remoteConfig?.game_static_variables;

        GameStaticVariables data;

        if (!string.IsNullOrEmpty(incomeRaw) && incomeRaw != currentRaw)
        {
            data = JsonUtility.FromJson<GameStaticVariables>(incomeRaw);

            if (data == null)
            {
                data = JsonUtility.FromJson<GameStaticVariables>(currentRaw);
            }
        }
        else
        {
            data = JsonUtility.FromJson<GameStaticVariables>(currentRaw);
        }

        return data;
    }

    #region VersionBase
    // public GameStaticVariables Deserialize()
    // {
    //     GameStaticVariables gsVariables = JsonUtility.FromJson<GameStaticVariables>(path.Get());
    //     string income = CheckAndMigrate(gsVariables.version);

    //     if (income != null)
    //     {
    //         gsVariables = JsonUtility.FromJson<GameStaticVariables>(income);
    //         this.version = gsVariables.version;
    //         this.variables = gsVariables.variables;
    //         this.Save();
    //     }

    //     return gsVariables;
    // }

    // public string CheckAndMigrate(string version)
    // {
    //     string raw = GameManager.Instance.sdkManager?.firebase?.remoteConfig?.game_static_variables;
    //     if (string.IsNullOrEmpty(raw)) return null;
    //     GameStaticVariables gsvIncome = JsonUtility.FromJson<GameStaticVariables>(raw);

    //     if (gsvIncome.version != version) return raw;

    //     return null;
    // }
    #endregion

    public string GetRaw()
    {
        return path.Get();
    }

    [System.Serializable]
    public class Variables
    {
        public string dollarColor;
        public string diamondColor;
        public string buttonEnableColor;
        public string buttonDisableColor;
        public string dollarSpritePath;
        public string diamondSpritePath;

        public string instagramURL;
        public string facebookURL;
        public string twitterURL;
        public string websiteURL;
        public string privacyURL;
        public string marketURL;
        public string rateURL;

        public float cameraFocusDuration;

        public float maxCrashSound;
        public float maxBrakeSound;
        public float maxWindSound;
        public float maxgearShiftSound;
        public float maxEngineSound;

        public float gameSceneMusicLevelFactor;

        public string gpgTotalExperienceScoreboardID;
        public string gpgTravelersScoreboardID;
        public string gpgDriftersScoreboardID;
        public string gpgFastDriversScoreboardID;

        public int miniMapMode;
        public bool policeChase;
        public bool useInAppReview;

        public bool bannerEnable;
        public bool pauseButtonAdsEnable;
        public bool inGameAdsEnable;
        public float interstitialFrequence;
    }

    [System.Serializable]
    public class VariablesComponent
    {
        public Color dollarColor;
        public Color diamondColor;
        public Color buttonEnableColor;
        public Color buttonDisableColor;
        public Sprite dollarSprite;
        public Sprite diamondSprite;

        public string instagramURL;
        public string facebookURL;
        public string twitterURL;
        public string websiteURL;
        public string privacyURL;
        public string marketURL;
        public string rateURL;

        public float cameraFocusDuration;

        public float maxCrashSound;
        public float maxBrakeSound;
        public float maxWindSound;
        public float maxgearShiftSound;
        public float maxEngineSound;

        public float gameSceneMusicLevelFactor;

        public string gpgTotalExperienceScoreboardID;
        public string gpgTravelersScoreboardID;
        public string gpgDriftersScoreboardID;
        public string gpgFastDriversScoreboardID;

        public int miniMapMode;
        public bool policeChase;
        public bool useInAppReview;

        public bool bannerEnable;
        public bool inGameAdsEnable;
        public bool pauseButtonAdsEnable;
        public float interstitialFrequence;

        public VariablesComponent(Variables variables)
        {
            this.dollarColor = variables.dollarColor.GetColor();
            this.diamondColor = variables.diamondColor.GetColor();
            this.buttonEnableColor = variables.buttonEnableColor.GetColor();
            this.buttonDisableColor = variables.buttonDisableColor.GetColor();
            this.dollarSprite = Resources.Load<Sprite>(variables.dollarSpritePath);
            this.diamondSprite = Resources.Load<Sprite>(variables.diamondSpritePath);

            this.instagramURL = variables.instagramURL;
            this.facebookURL = variables.facebookURL;
            this.twitterURL = variables.twitterURL;
            this.websiteURL = variables.websiteURL;
            this.privacyURL = variables.privacyURL;
            this.marketURL = variables.marketURL;
            this.rateURL = variables.rateURL;

            this.cameraFocusDuration = variables.cameraFocusDuration;

            this.maxCrashSound = variables.maxCrashSound;
            this.maxBrakeSound = variables.maxBrakeSound;
            this.maxWindSound = variables.maxWindSound;
            this.maxgearShiftSound = variables.maxgearShiftSound;
            this.maxEngineSound = variables.maxEngineSound;

            this.gameSceneMusicLevelFactor = variables.gameSceneMusicLevelFactor;
            this.gpgTravelersScoreboardID = variables.gpgTravelersScoreboardID;
            this.gpgDriftersScoreboardID = variables.gpgDriftersScoreboardID;
            this.gpgFastDriversScoreboardID = variables.gpgFastDriversScoreboardID;

            this.gpgTotalExperienceScoreboardID = variables.gpgTotalExperienceScoreboardID;

            this.miniMapMode = variables.miniMapMode;
            this.policeChase = variables.policeChase;
            this.useInAppReview = variables.useInAppReview;
            this.bannerEnable = variables.bannerEnable;
            this.inGameAdsEnable = variables.inGameAdsEnable;
            this.pauseButtonAdsEnable = variables.pauseButtonAdsEnable;
            this.interstitialFrequence = variables.interstitialFrequence;
        }

        public void SetPoliceChase(bool chase)
        {
            GameStaticVariables gsv = new GameStaticVariables().Deserialize();
            gsv.variables.policeChase = chase;
            gsv.Save();
        }
    }
}