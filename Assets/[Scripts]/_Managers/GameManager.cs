using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class GameManager : SingletonPersistent<GameManager>
{
    [HideInInspector] public string currentScene;

    [HideInInspector] public DataManager dataManager;
    [HideInInspector] public DelayManager delayManager;
    [HideInInspector] public CurrencyManager currencyManager;
    [HideInInspector] public AudioManager audioManager;
    [HideInInspector] public AdManager adManager;
    [HideInInspector] public AnalyticManager analyticManager;
    [HideInInspector] public SDKManager sdkManager;

    [HideInInspector] public bool onMobileDevice { get { return Application.platform == RuntimePlatform.IPhonePlayer || Application.platform == RuntimePlatform.Android; } }
    [HideInInspector] public LuckyWheelSystem luckyWheelSystem;
    [HideInInspector] public DailyRewardSystem dailyRewardSystem;
    [HideInInspector] public string deviceLanguage;
    public GameStaticVariables.VariablesComponent statics;

    public bool readFromLocal;

    [HideInInspector] public bool readyForStart;

    protected override void Awake()
    {
        base.Awake();
        Application.targetFrameRate = 60;

        deviceLanguage = Application.systemLanguage.ToString().ToLower();
        currentScene = SceneManager.GetActiveScene().name.ToLower();

        delayManager = gameObject.AddComponent<DelayManager>().Construct().GetComponent<DelayManager>(); // this is completely local

        if (currentScene.Contains("test"))
        {
            dataManager = gameObject.AddComponent<DataManager>().Construct().GetComponent<DataManager>();
            currencyManager = gameObject.AddComponent<CurrencyManager>().Construct().GetComponent<CurrencyManager>();
            audioManager = gameObject.AddComponent<AudioManager>().Construct().GetComponent<AudioManager>();
            adManager = gameObject.AddComponent<AdManager>().Construct().GetComponent<AdManager>();
            analyticManager = gameObject.AddComponent<AnalyticManager>().Construct().GetComponent<AnalyticManager>();

            statics = new GameStaticVariables.VariablesComponent(new GameStaticVariables().Deserialize().variables);
            OnChangedGraphicQuality(dataManager.user.gameData.settings.qualityLevel);

            gameObject.GetComponent<LevelManager>().enabled = true;

            return;
        }

        Transform sdk = new GameObject("sdk-manager").transform; sdk.transform.SetParent(transform);
        sdkManager = sdk.gameObject.AddComponent<SDKManager>().Construct().GetComponent<SDKManager>();
        sdkManager.ConstructFirebase(() =>
        {
            dataManager = gameObject.AddComponent<DataManager>().Construct().GetComponent<DataManager>();
            currencyManager = gameObject.AddComponent<CurrencyManager>().Construct().GetComponent<CurrencyManager>();
            audioManager = gameObject.AddComponent<AudioManager>().Construct().GetComponent<AudioManager>();
            adManager = gameObject.AddComponent<AdManager>().Construct().GetComponent<AdManager>();
            analyticManager = gameObject.AddComponent<AnalyticManager>().Construct().GetComponent<AnalyticManager>();

            statics = new GameStaticVariables.VariablesComponent(new GameStaticVariables().Deserialize().variables);
            OnChangedGraphicQuality(dataManager.user.gameData.settings.qualityLevel);

            sdkManager.ConstructOtherSDKs();
            readyForStart = true;

            adManager.SetBanner(statics.bannerEnable);
        });

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
    {
        currentScene = scene.name.ToLower();

        if (currentScene.Contains("showroom"))
        {
            Instantiate(Resources.Load<GameObject>("_managers/ui-manager-showroom"));
            Instantiate(Resources.Load<GameObject>("_managers/showroom-manager"));
            if (!luckyWheelSystem) luckyWheelSystem = gameObject.AddComponent<LuckyWheelSystem>();
            if (!dailyRewardSystem) dailyRewardSystem = gameObject.AddComponent<DailyRewardSystem>();
        }
    }

    public void OnChangedGraphicQuality(int index)
    {
        UniversalRenderPipelineAsset rp = Resources.Load<UniversalRenderPipelineAsset>("_rp/" + index);
        GraphicsSettings.renderPipelineAsset = rp;
        QualitySettings.SetQualityLevel(dataManager.user.gameData.settings.qualityLevel);
        QualitySettings.renderPipeline = rp;
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}