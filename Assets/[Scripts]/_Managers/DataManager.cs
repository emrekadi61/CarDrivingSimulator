using UnityEngine;

public class DataManager : ManagerBase
{
    [HideInInspector] public User user;
    [HideInInspector] public Cars cars;
    [HideInInspector] public IAPObjects iap;
    [HideInInspector] public Audios sounds;
    [HideInInspector] public DailyRewards dailyRewards;
    [HideInInspector] public RatePanelModel ratePanel;
    [HideInInspector] public HintModel hints;

    private void Awake() => base.onConstructed.AddListener(GetDatas);
    private void Start() => GameManager.Instance?.currencyManager.onCurrencyChanged.AddListener(OnCurrencyChanged);

    private void GetDatas()
    {
        user = new User().Deserialize();
        cars = new Cars().Deserialize();
        iap = new IAPObjects().Deserialize();
        sounds = new Audios().Deserialize();
        dailyRewards = new DailyRewards().Deserialize();
        ratePanel = new RatePanelModel().Deserialize();
        hints = new HintModel().Deserialize();

        if (user.gameData.cars == null || user.gameData.cars.Count <= 0)
        {
            user.gameData.cars.Add(cars.cars.Find(c => c.id == "00"));
            SaveUser();
        }
    }

    public void SaveUser() => user.Save();

    public void SaveCar(CarData data)
    {
        int index = user.gameData.cars.FindIndex(c => c.id == data.id);
        if (index == -1)
        {
            Debug.Log("An Error Occured!");
            return;
        }

        user.gameData.cars[index] = data;
        SaveUser();
    }

    public void DeleteAllData()
    {
        Utils.Data.DataUtils.DeleteAllData(() =>
        {
            StopAllCoroutines();
            DestroyImmediate(GameManager.Instance.gameObject);
            UnityEngine.SceneManagement.SceneManager.LoadScene(0);
        });
    }

    private void OnCurrencyChanged(Currency type, int amount)
    {
        switch (type)
        {
            case Currency.Dollar:
                user.gameData.dollar = amount;
                break;
            case Currency.Diamond:
                user.gameData.diamond = amount;
                break;
        }
        SaveUser();
    }
}