using UnityEngine;

public class LevelManagerTest : Singleton<LevelManager>
{
    [HideInInspector] public RCC_Camera rccCamera;
    [HideInInspector] public CameraTiltController cameraTilt;
    [HideInInspector] public MiniMapCamera miniMapCamera;
    [HideInInspector] public User.UserStatistics statistics;
    [HideInInspector] public Car car;
    [HideInInspector] public bool respawning;
    [HideInInspector] public int earnedDollar = 0;
    [HideInInspector] public int earnedDiamond = 0;
    [HideInInspector] public AchievementChecker achievementChecker;

    private void Start()
    {
        string carID = GameManagerTest.Instance.dataManager.cars.cars[GameManagerTest.Instance.dataManager.user.gameData.carIndex].id;
        CarData carData = GameManagerTest.Instance.dataManager.user.gameData.cars.Find(c => c.id == carID);

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

        // UIManager.Instance.Push(GameMainPanel.Get(car.rcc));
        // UIManager.Instance.Push(RatePanel.Get());

        GameManagerTest.Instance.audioManager?.SetVolume("theme", GameManagerTest.Instance.dataManager.user.gameData.settings.musicLevel * 0.5f);

        if (!achievementChecker) achievementChecker = gameObject.AddComponent<AchievementChecker>();
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
}