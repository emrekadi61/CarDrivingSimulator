using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class ShowroomManager : Singleton<ShowroomManager>
{
    [HideInInspector] public Car current;
    [HideInInspector] public UnityEvent onNextCar = new UnityEvent();
    [HideInInspector] public UnityEvent onPrevCar = new UnityEvent();
    [HideInInspector] public OnCarChanged onCarChanged = new OnCarChanged();
    [HideInInspector] public OnCarBought onCarBought = new OnCarBought();

    [HideInInspector] public UnityEvent onPressedDoorButton = new UnityEvent();

    private Transform carPoint;
    private int carIndex;

    private void Start()
    {
        carPoint = transform.Find("car-point");
        carIndex = GameManager.Instance.dataManager.user.gameData.carIndex;

        if (carIndex >= GameManager.Instance.dataManager.cars.cars.Count)
            carIndex = GameManager.Instance.dataManager.cars.cars.FindIndex(c => c.id == GameManager.Instance.dataManager.user.gameData.cars.Last().id);

        SpawnCar(GameManager.Instance.dataManager.cars.cars[carIndex]);
        onNextCar.AddListener(NextCar);
        onPrevCar.AddListener(PrevCar);
        onCarBought.AddListener(OnCarBoughted);

        GameManager.Instance.audioManager?.SetVolume("theme", GameManager.Instance.dataManager.user.gameData.settings.musicLevel);
        GameManager.Instance.sdkManager?.gpgManager.ReportScore(GameManager.Instance.statics.gpgTotalExperienceScoreboardID, GameManager.Instance.dataManager.user.gameData.statistics.experience);
    }

    private void SpawnCar(CarData carData)
    {
        if (current) Destroy(current.gameObject); current = null;

        string prefabName = GameManager.Instance.dataManager.cars.cars[carIndex].prefabName;
        current = Instantiate(Resources.Load<GameObject>("_cars/" + prefabName)).GetComponent<Car>();

        CarData cd = GameManager.Instance.dataManager.user.gameData.cars.Find(c => c.id == carData.id);
        if (cd != null)
        {
            carData = cd;
            GameManager.Instance.dataManager.user.gameData.carIndex = carIndex;
            GameManager.Instance.dataManager.SaveUser();
        }

        current.transform.position = carPoint.position;
        current.transform.rotation = carPoint.rotation;

        current.Construct(carData);

        onCarChanged.Invoke(current);
    }

    private void NextCar()
    {
        carIndex++;
        if (carIndex >= GameManager.Instance.dataManager.cars.cars.Count) carIndex = 0;
        SpawnCar(GameManager.Instance.dataManager.cars.cars[carIndex]);
    }

    private void PrevCar()
    {
        carIndex--;
        if (carIndex < 0) carIndex = GameManager.Instance.dataManager.cars.cars.Count - 1;
        SpawnCar(GameManager.Instance.dataManager.cars.cars[carIndex]);
    }

    private void OnCarBoughted(CarData data)
    {
        GameManager.Instance.dataManager.user.gameData.cars.Add(data);
        GameManager.Instance.dataManager.SaveUser();

        GameManager.Instance.dataManager.user.gameData.carIndex = carIndex;
        GameManager.Instance.dataManager.SaveUser();
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        onNextCar?.RemoveAllListeners();
        onPrevCar?.RemoveAllListeners();
        onCarChanged?.RemoveAllListeners();
        onCarBought?.RemoveAllListeners();
        onPressedDoorButton?.RemoveAllListeners();
    }

    public class OnCarChanged : UnityEvent<Car> { }
    public class OnCarBought : UnityEvent<CarData> { }
}