using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class Car : MonoBehaviour
{
    [HideInInspector] public CarData data;
    [HideInInspector] public RCC_CarControllerV3 rcc;
    [HideInInspector] public CarLightSystem lightSystem;
    [HideInInspector] public CarPaintSystemV2 paintSystem;
    [HideInInspector] public OpenablePart[] openableParts;
    [HideInInspector] public CarShowroomMode showroomMode;
    [HideInInspector] public CarModeSetter carMode;
    [HideInInspector] public CarCheckerController checkerController;

    [HideInInspector] public UnityEvent onPlateChanged = new UnityEvent();
    [HideInInspector] public UnityEvent onUpgraded = new UnityEvent();
    [HideInInspector] public OnAxlesChanged onAxlesChanged = new OnAxlesChanged();
    [HideInInspector] public OnWheelChanged onWheelChanged = new OnWheelChanged();

    public Transform[] licencePoints;
    private CarLicencePlate[] licencePlates;

    public Car Construct(CarData data)
    {
        this.data = data;

        rcc = GetComponent<RCC_CarControllerV3>();
        lightSystem = GetComponent<CarLightSystem>().Construct(this);
        paintSystem = GetComponent<CarPaintSystemV2>().Construct(this);
        openableParts = GetComponentsInChildren<OpenablePart>();
        carMode = gameObject.AddComponent<CarModeSetter>().Construct(this);

        if (GameManager.Instance && GameManager.Instance.currentScene.Contains("showroom"))
        {
            showroomMode = gameObject.AddComponent<CarShowroomMode>().Construct(this);
            ShowroomManager.Instance.onPressedDoorButton.AddListener(SetOpenableParts);
            onAxlesChanged.AddListener(OnAxleChanged);
            onWheelChanged.AddListener(OnWheelsChanged);
            onPlateChanged.AddListener(SetLicencePlate);
            onUpgraded.AddListener(SetCarSpecifications);
        }

        if (GameManager.Instance && GameManager.Instance.currentScene.Contains("game"))
        {
            checkerController = gameObject.AddComponent<CarCheckerController>().Construct(this);
        }

        ApplyDataToCars();
        SetSoundVolumes();
        return this;
    }

    public void SetSoundVolumes()
    {
        RCC_Settings.Instance.maxCrashSoundVolume = GameManager.Instance.statics.maxCrashSound * GameManager.Instance.dataManager.user.gameData.settings.fxLevel;
        RCC_Settings.Instance.maxBrakeSoundVolume = GameManager.Instance.statics.maxBrakeSound * GameManager.Instance.dataManager.user.gameData.settings.fxLevel;
        RCC_Settings.Instance.maxWindSoundVolume = GameManager.Instance.statics.maxWindSound * GameManager.Instance.dataManager.user.gameData.settings.fxLevel;
        rcc.maxEngineSoundVolume = GameManager.Instance.statics.maxEngineSound * GameManager.Instance.dataManager.user.gameData.settings.fxLevel;
    }

    public void SetHood()
    {
        OpenablePart op = openableParts.ToList().Find(c => c.gameObject.name.Contains("hood"));
        op?.Set();
    }

    public void SetOpenableParts()
    {
        for (int i = 0; i < openableParts.Length; i++) openableParts[i].Set();
    }

    public void SetOpenableParts(float duration = 1f)
    {
        for (int i = 0; i < openableParts.Length; i++) openableParts[i].Set(duration);
    }

    private void ApplyDataToCars()
    {
        SetRims();
        SetAxles();
        SetCarSpecifications();
        SetPaints();
        SetLicencePlate();
    }

    private void SetRims(int rimIndex = -1)
    {
        if (rimIndex == -1) rimIndex = data.rimIndex;

        Transform tr;
        Vector3 euler;
        GameObject prefab = Resources.Load<GameObject>("_cars/_wheels/wheel-" + rimIndex);

        euler = rcc.FrontLeftWheelTransform.GetChild(0).localEulerAngles;
        DestroyImmediate(rcc.FrontLeftWheelTransform.GetChild(0).gameObject);
        tr = Instantiate(prefab, rcc.FrontLeftWheelTransform).transform;
        tr.localPosition = Vector3.zero;
        tr.localEulerAngles = euler;
        tr.localScale = Vector3.one;

        euler = rcc.FrontRightWheelTransform.GetChild(0).localEulerAngles;
        DestroyImmediate(rcc.FrontRightWheelTransform.GetChild(0).gameObject);
        tr = Instantiate(prefab, rcc.FrontRightWheelTransform).transform;
        tr.localPosition = Vector3.zero;
        tr.localEulerAngles = euler;
        tr.localScale = Vector3.one;

        euler = rcc.RearLeftWheelTransform.GetChild(0).localEulerAngles;
        DestroyImmediate(rcc.RearLeftWheelTransform.GetChild(0).gameObject);
        tr = Instantiate(prefab, rcc.RearLeftWheelTransform).transform;
        tr.localPosition = Vector3.zero;
        tr.localEulerAngles = euler;
        tr.localScale = Vector3.one;

        euler = rcc.RearRightWheelTransform.GetChild(0).localEulerAngles;
        DestroyImmediate(rcc.RearRightWheelTransform.GetChild(0).gameObject);
        tr = Instantiate(prefab, rcc.RearRightWheelTransform).transform;
        tr.localPosition = Vector3.zero;
        tr.localEulerAngles = euler;
        tr.localScale = Vector3.one;

        paintSystem.rimPaint.FindParts(gameObject);
        paintSystem.rimPaint.SetColor(data.visual.rim.color.GetColor(), data.visual.rim.tint);
    }

    private void SetAxles(Axles income = null)
    {
        Axles axles = income ?? data.axles;

        rcc.FrontLeftWheelCollider.wheelCollider.suspensionDistance = rcc.FrontRightWheelCollider.wheelCollider.suspensionDistance = axles.front.suspensionDistance;
        rcc.RearLeftWheelCollider.wheelCollider.suspensionDistance = rcc.RearRightWheelCollider.wheelCollider.suspensionDistance = axles.rear.suspensionDistance;

        rcc.FrontLeftWheelCollider.wheelOffset = rcc.FrontRightWheelCollider.wheelOffset = axles.front.wheelOffset;
        rcc.RearLeftWheelCollider.wheelOffset = rcc.RearRightWheelCollider.wheelOffset = axles.rear.wheelOffset;

        rcc.FrontLeftWheelCollider.camber = rcc.FrontRightWheelCollider.camber = axles.front.wheelCamber;
        rcc.RearLeftWheelCollider.camber = rcc.RearRightWheelCollider.camber = axles.rear.wheelCamber;
    }

    private void SetCarSpecifications()
    {
        rcc.maxEngineTorque = data.specifications.engineTorque + (data.upgrades.engine * data.upgradeIncrements.engine);
        rcc.gearShiftingDelay = data.specifications.transmission - (data.upgrades.transmission * data.upgradeIncrements.transmission);
        rcc.brakeTorque = data.specifications.brakeTorque + (data.upgrades.brakes * data.upgradeIncrements.brakes);
    }

    private void SetPaints()
    {
        paintSystem.bodyPaint.SetColor(data.visual.body.color.GetColor(), data.visual.body.tint);
        paintSystem.bodyDetailPaint.SetColor(data.visual.bodyDetail.color.GetColor(), data.visual.bodyDetail.tint);
        paintSystem.rimPaint.SetColor(data.visual.rim.color.GetColor(), data.visual.rim.tint);
        paintSystem.windowPaint.SetColor(true, data.visual.window.color.GetColor(), data.visual.window.tint);
    }

    private void OnAxleChanged(Axles axles)
    {
        SetAxles(axles);
    }

    private void OnWheelsChanged(int index)
    {
        SetRims(index);
    }

    private void SetLicencePlate()
    {
        string plate = GameManager.Instance.dataManager.user.gameData.plate.text;
        if (string.IsNullOrEmpty(plate))
        {
            if (licencePlates != null && licencePlates.Length >= 0)
            {
                for (int i = 0; i < licencePlates.Length; i++) Destroy(licencePlates[i].gameObject);
            }
            return;
        }
        if (licencePlates == null || licencePlates.Length <= 0)
        {
            licencePlates = new CarLicencePlate[licencePoints.Length];
            for (int i = 0; i < licencePoints.Length; i++)
            {
                CarLicencePlate clp = Instantiate(Resources.Load<GameObject>("_prefabs/licence-plate"), licencePoints[i]).GetComponent<CarLicencePlate>();
                clp.transform.localPosition = Vector3.zero;
                clp.transform.localEulerAngles = Vector3.zero;
                clp.transform.localScale = Vector3.one;
                clp.SetPlate(plate, GameManager.Instance.dataManager.user.gameData.plate.baseColor.GetColor(), GameManager.Instance.dataManager.user.gameData.plate.textColor.GetColor());

                licencePlates[i] = clp;
            }
            return;
        }

        for (int i = 0; i < licencePlates.Length; i++)
            licencePlates[i].SetPlate(plate, GameManager.Instance.dataManager.user.gameData.plate.baseColor.GetColor(), GameManager.Instance.dataManager.user.gameData.plate.textColor.GetColor());
    }

    private void OnDestroy()
    {
        onPlateChanged?.RemoveAllListeners();
        onUpgraded?.RemoveAllListeners();
        onWheelChanged?.RemoveAllListeners();
    }

    public class OnAxlesChanged : UnityEvent<Axles> { }
    public class OnWheelChanged : UnityEvent<int> { }
}