using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Utils.UI;
using Utils.Strings;

public class GamePauseMenu : MonoBehaviour
{
    public RectTransform backgroungRect;
    public RectTransform titleRect;
    public RectTransform stripRect;
    public RectTransform continueButtonRect;
    public RectTransform garageButtonRect;
    public RectTransform noAdsButtonRect;
    public RectTransform rewardButtonRect;
    public RectTransform currencyPanelRect;

    public StatisticsPanel tourStatistics;
    public StatisticsPanel gameStatistics;

    #region Components
    private Image background;
    private Image pattern;
    private Button continueButton;
    private Button garageButton;
    private GraphicQualitySettings graphicQualitySettings;
    private ControllerSettings controllerSettings;
    private SFXSettings sfxSettings;
    private PromoteButton promoteButton1;
    private PromoteButton promoteButton2;
    private GameMainPanel gmp;
    #endregion

    public GamePauseMenu Construct(GameMainPanel gmp = null, bool animate = true)
    {
        DOTween.defaultTimeScaleIndependent = true;

        this.gmp = gmp;

        background = backgroungRect.GetComponent<Image>();
        pattern = backgroungRect.GetChild(0).GetComponent<Image>();
        continueButton = continueButtonRect.GetButton(OnPressedContinueButton);
        garageButton = garageButtonRect.GetButton(OnPressedGarageButton);

        graphicQualitySettings = GetComponentInChildren<GraphicQualitySettings>().Construct(OnChangedGraphicQuality);
        controllerSettings = GetComponentInChildren<ControllerSettings>().Construct(OnChangedController);
        sfxSettings = GetComponentInChildren<SFXSettings>().Construct(OnChangedMusicLevel, OnChangedFXLevel, OnChangedHapticSelection);

        User.UserStatistics statistics = new User.UserStatistics();

        tourStatistics.Construct(LevelManager.Instance.statistics, LevelManager.Instance.earnedDollar, LevelManager.Instance.earnedDiamond);
        gameStatistics.Construct(GameManager.Instance.dataManager.user.gameData.statistics,
                                GameManager.Instance.dataManager.user.gameData.dollar,
                                GameManager.Instance.dataManager.user.gameData.diamond);

        Appear(animate ? 0.25f : 0f);
        ConstructPromotes();

        GameManager.Instance.analyticManager.Showed("game-pause-panel");

        return this;
    }

    private void Appear(float duration = 0.25f)
    {
        Color c = background.color; c.a = 0f; background.color = c;

        c = pattern.color;
        float a = c.a;
        c.a = 0f;
        pattern.color = c;

        background.DOFade(1f, duration);
        pattern.DOFade(a, duration);

        titleRect.anchoredPosition += new Vector2(0f, 1000f);
        stripRect.localScale = new Vector3(1f, 0f, 1f);

        graphicQualitySettings.rect.anchoredPosition += new Vector2(1000f, 0f);
        controllerSettings.rect.anchoredPosition += new Vector2(1000f, 0f);
        sfxSettings.rect.anchoredPosition += new Vector2(1000f, 0f);

        tourStatistics.rect.anchoredPosition += new Vector2(-1000f, 0f);
        gameStatistics.rect.anchoredPosition += new Vector2(-1000f, 0f);

        continueButtonRect.anchoredPosition += new Vector2(0f, -1000f);
        garageButtonRect.anchoredPosition += new Vector2(0f, -1000f);

        noAdsButtonRect.anchoredPosition += new Vector2(-1000f, 0f);
        rewardButtonRect.anchoredPosition += new Vector2(1000f, 0f);

        currencyPanelRect.anchoredPosition += new Vector2(0f, 1000f);

        titleRect.DOAnchorPosY(titleRect.anchoredPosition.y - 1000f, duration);
        stripRect.DOScaleY(1f, duration);

        graphicQualitySettings.rect.DOAnchorPosX(graphicQualitySettings.rect.anchoredPosition.x - 1000f, duration);
        controllerSettings.rect.DOAnchorPosX(controllerSettings.rect.anchoredPosition.x - 1000f, duration).SetDelay(0.1f);
        sfxSettings.rect.DOAnchorPosX(sfxSettings.rect.anchoredPosition.x - 1000f, duration).SetDelay(0.2f);

        tourStatistics.rect.DOAnchorPosX(tourStatistics.rect.anchoredPosition.x + 1000f, duration);
        gameStatistics.rect.DOAnchorPosX(gameStatistics.rect.anchoredPosition.x + 1000f, duration).SetDelay(0.1f);

        continueButtonRect.DOAnchorPosY(continueButtonRect.anchoredPosition.y + 1000f, duration);
        garageButtonRect.DOAnchorPosY(garageButtonRect.anchoredPosition.y + 1000f, duration);

        noAdsButtonRect?.DOAnchorPosX(noAdsButtonRect.anchoredPosition.x + 1000f, duration);
        rewardButtonRect.DOAnchorPosX(rewardButtonRect.anchoredPosition.x - 1000f, duration);

        currencyPanelRect.DOAnchorPosY(currencyPanelRect.anchoredPosition.y - 1000f, duration);

        SetSounds(true);
        DOTween.To(() => Time.timeScale, x => Time.timeScale = x, 0f, duration);
    }

    private void Disappear(float duration = 0.25f)
    {
        background.DOFade(0f, duration);
        pattern.DOFade(0f, duration);
        titleRect.DOAnchorPosY(titleRect.anchoredPosition.y + 1000f, duration);
        stripRect.DOScaleY(0f, duration);

        sfxSettings.rect.DOAnchorPosX(sfxSettings.rect.anchoredPosition.x + 1000f, duration);
        controllerSettings.rect.DOAnchorPosX(controllerSettings.rect.anchoredPosition.x + 1000f, duration).SetDelay(0.1f);
        graphicQualitySettings.rect.DOAnchorPosX(graphicQualitySettings.rect.anchoredPosition.x + 1000f, duration).SetDelay(0.2f).OnComplete(() => Destroy(gameObject));

        gameStatistics.rect.DOAnchorPosX(gameStatistics.rect.anchoredPosition.x - 1000f, duration);
        tourStatistics.rect.DOAnchorPosX(tourStatistics.rect.anchoredPosition.x - 1000f, duration).SetDelay(0.1f);

        continueButtonRect.DOAnchorPosY(continueButtonRect.anchoredPosition.y - 1000f, duration);
        garageButtonRect.DOAnchorPosY(garageButtonRect.anchoredPosition.y - 1000f, duration);

        noAdsButtonRect.DOAnchorPosX(noAdsButtonRect.anchoredPosition.x - 1000f, duration);
        rewardButtonRect.DOAnchorPosX(rewardButtonRect.anchoredPosition.x + 1000f, duration);

        currencyPanelRect.DOAnchorPosY(currencyPanelRect.anchoredPosition.y + 1000f, duration);

        SetSounds(false);
        DOTween.To(() => Time.timeScale, x => Time.timeScale = x, 1f, duration);
    }

    private void ConstructPromotes()
    {
        if (GameManager.Instance.dataManager.user.boughtedNoAds)
        {
            noAdsButtonRect.gameObject.SetActive(false);
        }
        else
        {
            noAdsButtonRect.gameObject.SetActive(true);
            promoteButton1 = noAdsButtonRect.GetComponent<PromoteButton>();
            promoteButton1.Construct(true, null, OnPressedNoAds);
        }

        promoteButton2 = rewardButtonRect.GetComponent<PromoteButton>();
        Price p = new Price();
        p.type = Random.Range(0f, 1f) < 0.15f ? 1 : 0;
        p.amount = p.type == 0 ? (Random.Range(1, 10) * 500) : Random.Range(1, 2);
        promoteButton2.Construct(false, p, OnPressedGetReward);
    }

    private void SetSounds(bool mute)
    {
        LevelManager.Instance.car.gameObject.GetComponentsInChildren<AudioSource>().ToList().ForEach(c => c.mute = mute);
    }

    private void OnPressedNoAds(Price price)
    {
        GameManager.Instance.analyticManager.Clicked("game-pause-panel", "no-ads-button");
        IAPObject obj = GameManager.Instance.dataManager.iap.objects.Find(c => c.id.Contains("no_ads"));
        GameManager.Instance.sdkManager.iapManager.Buy(obj.id, b =>
        {
            if (b)
            {
                GameManager.Instance.sdkManager?.OnBoughtedNoAds();
                noAdsButtonRect.gameObject.SetActive(false);
            }
        });
    }

    private void OnPressedGetReward(Price price)
    {
        GameManager.Instance.adManager.ShowRewarded(b =>
        {
            if (b)
            {
                GameManager.Instance.currencyManager.Earn(price);
                GameManager.Instance.analyticManager.Clicked("game-pause-panel", "reward-button");
                rewardButtonRect.gameObject.SetActive(false);
            }
        });
    }

    private void OnPressedContinueButton()
    {
        Disappear();

        GameManager.Instance.analyticManager.Clicked("game-pause-panel", "continue-button");
    }

    private void OnPressedGarageButton()
    {
        SceneLoader.Get("Showroom");
        Time.timeScale = 1f;

        GameManager.Instance.analyticManager.Clicked("game-pause-panel", "garage-button");
        ReportStatistics();
    }

    private void OnChangedGraphicQuality(int index)
    {

    }

    private void OnChangedController(int index)
    {
        this.gmp?.Construct(null);
        LevelManager.Instance.cameraTilt.enabled = index == 3;
    }

    private void OnChangedMusicLevel(float val)
    {

    }

    private void OnChangedFXLevel(float val)
    {
        LevelManager.Instance.car.SetSoundVolumes();
    }

    private void OnChangedHapticSelection(int index)
    {

    }

    [System.Serializable]
    public class ReportStatisticsData
    {
        public string dollar;
        public string diamond;
        public User.UserStatistics statistics;
    }

    private void ReportStatistics()
    {
        GameManager.Instance.sdkManager?.firebase?.analytics?.Progression("tour",
            new ReportStatisticsData()
            {
                dollar = LevelManager.Instance?.earnedDollar.ToCurrencyString(),
                diamond = LevelManager.Instance?.earnedDiamond.ToCurrencyString(),
                statistics = LevelManager.Instance?.statistics
            }
        );

        GameManager.Instance.sdkManager?.firebase?.analytics?.Progression("total",
            new ReportStatisticsData()
            {
                dollar = GameManager.Instance?.dataManager?.user.gameData.dollar.ToCurrencyString(),
                diamond = GameManager.Instance?.dataManager?.user.gameData.diamond.ToCurrencyString(),
                statistics = GameManager.Instance?.dataManager?.user.gameData.statistics
            }
        );
    }

    private void OnApplicationQuit()
    {
        ReportStatistics();
    }

    private void OnDestroy()
    {
        continueButton?.onClick.RemoveAllListeners();
        garageButton?.onClick.RemoveAllListeners();
        DOTween.defaultTimeScaleIndependent = false;
    }

    public static EKCanvas Get(GameMainPanel gmp = null, bool animate = true)
    {
        GamePauseMenu gpm = Instantiate(Resources.Load<GameObject>("_ui/_game/pause-canvas")).GetComponent<GamePauseMenu>().Construct(gmp, animate);
        return gpm.gameObject.AddComponent<EKCanvas>().Construct();
    }

    [System.Serializable]
    public class StatisticsPanel
    {
        public RectTransform rect;
        [HideInInspector] public RectTransform experienceTextRect;
        [HideInInspector] public RectTransform tripTextRect;
        [HideInInspector] public RectTransform driftTextRect;
        [HideInInspector] public RectTransform dollarTextRect;
        [HideInInspector] public RectTransform diamondTextRect;

        private Text experienceText;
        private Text tripText;
        private Text driftText;
        private Text dollarText;
        private Text diamondText;

        public void Construct(User.UserStatistics statistics, int dollar, int diamond)
        {
            experienceTextRect = rect.Find("panel/experience/value").GetComponent<RectTransform>();
            tripTextRect = rect.Find("panel/trip/value").GetComponent<RectTransform>();
            driftTextRect = rect.Find("panel/drift/value").GetComponent<RectTransform>();
            dollarTextRect = rect.Find("panel/dollar/value").GetComponent<RectTransform>();
            diamondTextRect = rect.Find("panel/diamond/value").GetComponent<RectTransform>();

            experienceText = experienceTextRect.GetComponent<Text>();
            tripText = tripTextRect.GetComponent<Text>();
            driftText = driftTextRect.GetComponent<Text>();
            dollarText = dollarTextRect.GetComponent<Text>();
            diamondText = diamondTextRect.GetComponent<Text>();

            experienceText.text = statistics.experience.ToString() + " POINT";
            tripText.text = statistics.trip.ToString("F1") + " KM";
            driftText.text = statistics.drift.ToCurrencyString();
            dollarText.text = dollar.ToCurrencyString();
            diamondText.text = diamond.ToCurrencyString();
        }
    }
}