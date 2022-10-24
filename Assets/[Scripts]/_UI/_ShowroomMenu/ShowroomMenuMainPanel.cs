using UnityEngine;
using UnityEngine.UI;
using Utils.UI;
using DG.Tweening;

public class ShowroomMenuMainPanel : MonoBehaviour
{
    #region Variables
    [Header("GIZMOS"), Space(5)]
    public RectTransform carFeaturesRect;
    public RectTransform userStatisticsRect;
    public RectTransform doorButtonRect;

    [Header("ARROWS"), Space(5)]
    public RectTransform nextButtonRect;
    public RectTransform prevButtonRect;

    [Header("TOP"), Space(5)]
    public RectTransform settingsButtonRect;

    [Header("SOCIAL"), Space(5)]
    public RectTransform instagramButtonRect;
    public RectTransform facebookButtonRect;
    public RectTransform twitterButtonRect;
    public RectTransform websiteButtonRect;
    public RectTransform rateButtonRect;

    [Header("USER"), Space(5)]
    public RectTransform avatarRect;
    public RectTransform usernameTextRect;

    [Header("BOTTOM"), Space(5)]
    public RectTransform bottomButtonContainerRect;
    public RectTransform customizeButtonRect;
    public RectTransform playButtonRect;
    public RectTransform upgradeButtonRect;
    public RectTransform rewardButtonRect;

    [Header("LOCK PANEL"), Space(5)]
    public RectTransform lockPanelRect;

    [Header("LOCK PANEL"), Space(5)]
    public RectTransform policeChaseSelectorRect;

    #region Components
    private Car current;
    private CarFeaturesPanel carFeatures;
    private UserStatisticsPanel userStatistics;
    private Button nextButton;
    private Button prevButton;
    private Button settingsButton;
    private Button instagramButton;
    private Button facebookButton;
    private Button twitterButton;
    private Button websiteButton;
    private Button rateButton;
    private Button addMoneyButton;
    private Image avatar;
    private Text usernameText;
    private Button customizeButton;
    private Button playButton;
    private Button upgradeButton;
    private LockPanel lockPanel;
    private Button doorButton;
    private Selector policeChaseSelector;
    private PromoteButton rewardButton;
    #endregion
    #endregion

    private void Awake()
    {
        carFeatures = carFeaturesRect.GetComponent<CarFeaturesPanel>().Construct(ShowroomManager.Instance.current);
        userStatistics = userStatisticsRect.GetComponent<UserStatisticsPanel>().Construct();

        nextButton = nextButtonRect.GetButton(OnPressedNextButton);
        prevButton = prevButtonRect.GetButton(OnPressedPrevButton);

        settingsButton = settingsButtonRect.GetButton(OnPressedSettingsButton);

        instagramButton = instagramButtonRect.GetButton(OnPressedInstagramButton);
        facebookButton = facebookButtonRect.GetButton(OnPressedFacebookButton);
        twitterButton = twitterButtonRect.GetButton(OnPressedTwitterButton);
        websiteButton = websiteButtonRect.GetButton(OnPressedWebsiteButton);
        rateButton = rateButtonRect.GetButton(OnPressedRateButton);

        avatar = avatarRect.gameObject.GetComponent<Image>();
        usernameText = usernameTextRect.gameObject.GetComponent<Text>();

        customizeButton = customizeButtonRect.GetButton(OnPressedCustomizeButton);
        playButton = playButtonRect.GetButton(OnPressedPlayButton);
        upgradeButton = upgradeButtonRect.GetButton(OnPressedUpgradeButton);

        lockPanel = lockPanelRect.GetComponent<LockPanel>();

        doorButton = doorButtonRect.GetButton(OnPressedDoorButton);

        policeChaseSelector = policeChaseSelectorRect.GetComponent<Selector>().Construct(GameManager.Instance.statics.policeChase ? 1 : 0, OnPoliceChaseSelectorChanged);

        ShowroomManager.Instance.onCarChanged.AddListener(OnCarChanged);
        ShowroomManager.Instance.onCarBought.AddListener(OnCarBought);

        if (!string.IsNullOrEmpty(GameManager.Instance.sdkManager.gpgManager.username)) usernameText.text = GameManager.Instance.sdkManager.gpgManager.username;

        ConstructPromoteButton();

        GameManager.Instance.analyticManager.Showed("showroom-main-panel");
    }

    private void ConstructPromoteButton()
    {
        rewardButton = rewardButtonRect.GetComponent<PromoteButton>();
        Price p = new Price();
        p.type = Random.Range(0f, 1f) < 0.15f ? 1 : 0;
        p.amount = p.type == 0 ? (Random.Range(1, 10) * 500) : Random.Range(1, 2);
        rewardButton.Construct(false, p, OnPressedGetReward);
    }

    private void OnPressedGetReward(Price price)
    {
        GameManager.Instance.adManager.ShowRewarded(b =>
        {
            if (b)
            {
                GameManager.Instance.currencyManager.Earn(price);
                GameManager.Instance.analyticManager.Clicked("showroom-main-panel", "reward-button");
                rewardButtonRect.gameObject.SetActive(false);
            }
        });
    }

    private void OnCarChanged(Car car)
    {
        current = car;

        CarData cd = GameManager.Instance.dataManager.user.gameData.cars.Find(c => c.id == current.data.id);
        if (cd == null)
        {
            lockPanel
            .Active
            (
                true,
                () => GameManager.Instance.currencyManager.Pay(car.data.price, b =>
                {
                    if (b) ShowroomManager.Instance.onCarBought.Invoke(car.data);
                }),
                car.data.price.amount,
                car.data.price.type == 0 ? Currency.Dollar : Currency.Diamond
            );
        }
        else
        {
            lockPanel.Active(false);
        }
        bottomButtonContainerRect.gameObject.SetActive(cd != null);
    }

    private void OnCarBought(CarData data)
    {
        lockPanel.Active(false);
        bottomButtonContainerRect.gameObject.SetActive(true);
    }

    private void OnPressedNextButton()
    {
        ShowroomManager.Instance.onNextCar.Invoke();
        GameManager.Instance.audioManager.Play("tap");

        GameManager.Instance.analyticManager.Clicked("showroom-main-panel", "next-button");
    }

    private void OnPressedPrevButton()
    {
        ShowroomManager.Instance.onPrevCar.Invoke();
        GameManager.Instance.audioManager.Play("tap");

        GameManager.Instance.analyticManager.Clicked("showroom-main-panel", "prev-button");
    }

    private void OnPressedSettingsButton()
    {
        UIManager.Instance.Push(SettingsPanel.Get());
        GameManager.Instance.audioManager.Play("tap");

        GameManager.Instance.analyticManager.Clicked("showroom-main-panel", "settings-button");
    }

    private void OnPressedInstagramButton()
    {
        Application.OpenURL(GameManager.Instance.statics.instagramURL);
        GameManager.Instance.audioManager.Play("tap");

        GameManager.Instance.analyticManager.Clicked("showroom-main-panel", "instagram-button");
    }

    private void OnPressedFacebookButton()
    {
        Application.OpenURL(GameManager.Instance.statics.facebookURL);
        GameManager.Instance.audioManager.Play("tap");

        GameManager.Instance.analyticManager.Clicked("showroom-main-panel", "facebook-button");
    }

    private void OnPressedTwitterButton()
    {
        Application.OpenURL(GameManager.Instance.statics.twitterURL);
        GameManager.Instance.audioManager.Play("tap");

        GameManager.Instance.analyticManager.Clicked("showroom-main-panel", "twitter-button");
    }

    private void OnPressedWebsiteButton()
    {
        Application.OpenURL(GameManager.Instance.statics.websiteURL);
        GameManager.Instance.audioManager.Play("tap");

        GameManager.Instance.analyticManager.Clicked("showroom-main-panel", "website-button");
    }

    private void OnPressedRateButton()
    {
        Application.OpenURL(GameManager.Instance.statics.rateURL);
        GameManager.Instance.audioManager.Play("tap");

        GameManager.Instance.analyticManager.Clicked("showroom-main-panel", "rate-button");
    }

    private void OnPressedCustomizeButton()
    {
        UIManager.Instance.Push(CustomizePanel.Get(HideMenu));
        GameManager.Instance.audioManager.Play("tap");

        GameManager.Instance.analyticManager.Clicked("showroom-main-panel", "customize-button");
    }

    private void OnPressedUpgradeButton()
    {
        UIManager.Instance.Push(UpgradePanel.Get(HideMenu));
        GameManager.Instance.audioManager.Play("tap");

        GameManager.Instance.analyticManager.Clicked("showroom-main-panel", "upgrade-button");
    }

    private void OnPressedDoorButton()
    {
        ShowroomManager.Instance.onPressedDoorButton?.Invoke();
        GameManager.Instance.audioManager.Play("tap");

        GameManager.Instance.analyticManager.Clicked("showroom-main-panel", "door-button");
    }

    private void OnPressedPlayButton()
    {
        ShowroomManager.Instance.current.showroomMode.Run();
        GameManager.Instance.audioManager.Play("tap");

        GameManager.Instance.analyticManager.Clicked("showroom-main-panel", "play-button");
    }

    private void HideMenu(bool active)
    {
        nextButtonRect.DOComplete();
        prevButtonRect.DOComplete();
        bottomButtonContainerRect.DOComplete();
        policeChaseSelectorRect.DOComplete();
        userStatisticsRect.DOComplete();
        carFeaturesRect.DOComplete();

        float duration = 0.25f;
        nextButtonRect.DOAnchorPosX(nextButtonRect.anchoredPosition.x + 1000f * (active ? 1f : -1f), duration);
        prevButtonRect.DOAnchorPosX(prevButtonRect.anchoredPosition.x - 1000f * (active ? 1f : -1f), duration);
        bottomButtonContainerRect.DOAnchorPosY(bottomButtonContainerRect.anchoredPosition.y - 1000f * (active ? 1f : -1f), duration);
        policeChaseSelectorRect.DOAnchorPosY(policeChaseSelectorRect.anchoredPosition.y + 1000f * (active ? 1f : -1f), duration);
        userStatisticsRect.DOAnchorPosX(userStatisticsRect.anchoredPosition.x - 1000f * (active ? 1f : -1f), duration);
        carFeaturesRect.DOAnchorPosX(carFeaturesRect.anchoredPosition.x + 1000f * (active ? 1f : -1f), duration);
    }

    private void OnPoliceChaseSelectorChanged(int index)
    {
        GameManager.Instance.statics.policeChase = index == 1;
        GameManager.Instance.statics.SetPoliceChase(index == 1);

        GameManager.Instance.analyticManager.Clicked("showroom-main-panel", "police-chase-" + (index == 1 ? "on" : "off") + "-button");
    }

    private void OnDestroy()
    {
        nextButton?.onClick.RemoveAllListeners();
        prevButton?.onClick.RemoveAllListeners();

        instagramButton?.onClick.RemoveAllListeners();
        facebookButton?.onClick.RemoveAllListeners();
        twitterButton?.onClick.RemoveAllListeners();
        websiteButton?.onClick.RemoveAllListeners();
        rateButton?.onClick.RemoveAllListeners();

        customizeButton?.onClick.RemoveAllListeners();
        upgradeButton?.onClick.RemoveAllListeners();
        playButton?.onClick.RemoveAllListeners();
    }
}