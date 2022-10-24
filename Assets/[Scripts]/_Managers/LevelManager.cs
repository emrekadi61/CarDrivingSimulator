using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Utils.Strings;

public class LevelManager : Singleton<LevelManager>
{
    public Transform minimapRoads;
    public Transform rewardPointsContainer;
    [HideInInspector] public RCC_Camera rccCamera;
    [HideInInspector] public CameraTiltController cameraTilt;
    [HideInInspector] public MiniMapCamera miniMapCamera;
    [HideInInspector] public User.UserStatistics statistics;
    [HideInInspector] public Car car;
    [HideInInspector] public OnMoneyEarn onMoneyEarn = new OnMoneyEarn();
    [HideInInspector] public bool respawning;
    [HideInInspector] public UnityEvent onGamePaused = new UnityEvent();
    [HideInInspector] public int earnedDollar = 0;
    [HideInInspector] public int earnedDiamond = 0;
    [HideInInspector] public AchievementChecker achievementChecker;

    #region PlayCount
    [HideInInspector] public int playCount;
    private readonly string PLAY_COUNT_DATA = "level-manager-play-count";
    #endregion

    private void Start()
    {
        playCount = PlayerPrefs.GetInt(PLAY_COUNT_DATA, 0);
        playCount++;
        PlayerPrefs.SetInt(PLAY_COUNT_DATA, playCount);

        string carID = GameManager.Instance.dataManager.cars.cars[GameManager.Instance.dataManager.user.gameData.carIndex].id;
        CarData carData = GameManager.Instance.dataManager.user.gameData.cars.Find(c => c.id == carID);

        GameObject prefab = Resources.Load<GameObject>("_cars/" + carData.prefabName);
        car = Instantiate(prefab).GetComponent<Car>().Construct(carData);
        car.transform.position = transform.position;
        car.transform.rotation = transform.rotation;

        rccCamera = Instantiate(Resources.Load<GameObject>("_prefabs/RCCCamera")).GetComponent<RCC_Camera>();
        rccCamera.transform.position = car.transform.position + (car.transform.forward * -10f);
        rccCamera.transform.rotation = car.transform.rotation;
        rccCamera.SetTarget(car.gameObject);
        cameraTilt = rccCamera.gameObject.AddComponent<CameraTiltController>().Construct(rccCamera, car.rcc);
        cameraTilt.enabled = false;

        UIManager.Instance.Push(GameMainPanel.Get(car.rcc));
        UIManager.Instance.Push(RatePanel.Get());

        SetMiniMap(GameManager.Instance.statics.miniMapMode);

        GameManager.Instance.audioManager?.SetVolume("theme", GameManager.Instance.dataManager.user.gameData.settings.musicLevel * 0.5f);

        if (!achievementChecker) achievementChecker = gameObject.AddComponent<AchievementChecker>();

        SetRewardPoints();
        onMoneyEarn.AddListener(OnMoneyEarned);
        onGamePaused.AddListener(OnGamePaused);
    }

    private void SetMiniMap(int miniMapMode)
    {
        if (miniMapMode == -1)
        {
            minimapRoads.gameObject.SetActive(false);
            return;
        }

        minimapRoads.gameObject.SetActive(true);
        minimapRoads.localPosition = Vector3.zero;
        miniMapCamera = Instantiate(Resources.Load<GameObject>("_prefabs/minimap-camera")).GetComponent<MiniMapCamera>().Construct(car.rcc, miniMapMode);

        Camera mmc = miniMapCamera.GetComponent<Camera>();
        Color c = mmc.backgroundColor; c.a = 0f; mmc.backgroundColor = c;
    }

    private void OnMoneyEarned(Price price)
    {
        earnedDollar += price.type == 0 ? price.amount : 0;
        earnedDiamond += price.type == 1 ? price.amount : 0;
        UIManager.Instance.Push(CurrencyEarnPanel.Get(price));
        GameManager.Instance.currencyManager.Earn(price);
    }

    private void FixedUpdate()
    {
        if (!car) return;
        if (car.transform.position.y <= transform.position.y - 30f)
        {
            respawning = true;
            Vector3 p = car.transform.position;
            p.y = transform.position.y + 3f;
            Quaternion r = car.transform.rotation;
            RCC.Transport(car.rcc, p, r);
        }
    }

    private void SetRewardPoints()
    {
        int totalCount = 5;
        List<int> arr = new List<int>();

        do
        {
            int v;
            do
            {
                v = Random.Range(0, rewardPointsContainer.childCount);
            }
            while (arr.Contains(v));
            arr.Add(v);
        }
        while (arr.Count < totalCount);

        for (int i = 0; i < arr.Count; i++)
        {
            Transform t;

            if (Random.Range(0f, 1f) < 0.2f) t = Instantiate(Resources.Load<GameObject>("_prefabs/ingame-rew-diamond")).transform;
            else t = Instantiate(Resources.Load<GameObject>("_prefabs/ingame-rew-dollar")).transform;

            t.gameObject.AddComponent<InGameRewards>();
            t.position = rewardPointsContainer.GetChild(arr[i]).position;
            t.rotation = rewardPointsContainer.GetChild(arr[i]).rotation;
        }

        arr.Clear();
    }

    private void OnGamePaused()
    {
        GameManager.Instance?.dataManager?.SaveUser();
        GameManager.Instance.sdkManager?.gpgManager?.ReportScore(GameManager.Instance.statics.gpgTotalExperienceScoreboardID, GameManager.Instance.dataManager.user.gameData.statistics.experience);
        GameManager.Instance.sdkManager?.gpgManager?.ReportScore(GameManager.Instance.statics.gpgTravelersScoreboardID, GameManager.Instance.dataManager.user.gameData.statistics.trip);
        GameManager.Instance.sdkManager?.gpgManager?.ReportScore(GameManager.Instance.statics.gpgDriftersScoreboardID, GameManager.Instance.dataManager.user.gameData.statistics.drift);
        GameManager.Instance.sdkManager?.gpgManager?.ReportScore(GameManager.Instance.statics.gpgFastDriversScoreboardID, GameManager.Instance.dataManager.user.gameData.statistics.highSpeedTrip);
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        onMoneyEarn.RemoveAllListeners();
        GameManager.Instance?.dataManager?.SaveUser();
    }

    public class OnMoneyEarn : UnityEvent<Price> { }
}