using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class GameManagerTest : Singleton<GameManagerTest>
{
    [HideInInspector] public string currentScene;

    [HideInInspector] public DataManager dataManager;
    [HideInInspector] public DelayManager delayManager;
    [HideInInspector] public CurrencyManager currencyManager;
    [HideInInspector] public AudioManager audioManager;

    [HideInInspector] public bool onMobileDevice { get { return Application.platform == RuntimePlatform.IPhonePlayer || Application.platform == RuntimePlatform.Android; } }
    public GameStaticVariables.VariablesComponent statics;

    protected override void Awake()
    {
        base.Awake();
        Application.targetFrameRate = 60;

        delayManager = gameObject.AddComponent<DelayManager>().Construct().GetComponent<DelayManager>();

        dataManager = gameObject.AddComponent<DataManager>().Construct().GetComponent<DataManager>();
        currencyManager = gameObject.AddComponent<CurrencyManager>().Construct().GetComponent<CurrencyManager>();
        audioManager = gameObject.AddComponent<AudioManager>().Construct().GetComponent<AudioManager>();

        statics = new GameStaticVariables.VariablesComponent(new GameStaticVariables().Deserialize().variables);
        OnChangedGraphicQuality(dataManager.user.gameData.settings.qualityLevel);
    }

    public void OnChangedGraphicQuality(int index)
    {
        UniversalRenderPipelineAsset rp = Resources.Load<UniversalRenderPipelineAsset>("_rp/" + index);
        GraphicsSettings.renderPipelineAsset = rp;
        QualitySettings.SetQualityLevel(dataManager.user.gameData.settings.qualityLevel);
        QualitySettings.renderPipeline = rp;
    }
}