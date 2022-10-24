using UnityEngine;
using UnityEngine.UI;
using Utils.UI;

public class GameMainPanel : MonoBehaviour
{
    [Header("BUTTON'S"), Space(5)]
    public RectTransform steeringWheelRect;
    public RectTransform joystickRect;
    public RectTransform leftButtonRect;
    public RectTransform rightButtonRect;
    public RectTransform gasButtonRect;
    public RectTransform brakeButtonRightRect;
    public RectTransform brakeButtonLeftRect;
    public RectTransform handbrakeButtonRect;
    public RectTransform pauseButtonRect;
    public RectTransform changeCameraButtonRect;
    public RectTransform respawnButtonRect;
    public RectTransform repairButtonRect;
    public RectTransform mirrorButtonRect;
    public RectTransform driveModeRect;

    #region Components
    private EKDashboard dashboard;
    private RCC_CarControllerV3 rcc;
    private RCC_MobileButtons rccButtons;
    private RCC_UIController gasButtonController;
    private RCC_UIController brakeButtonController;
    private RCC_UIController handbrakeButtonController;
    private RCC_UIController leftButtonController;
    private RCC_UIController rightButtonController;
    private RCC_UISteeringWheelController steeringWheelController;
    private RCC_UIJoystick joystickController;
    private RectTransform brakeButtonRect;
    private Button pauseButton;
    private Button changeCameraButton;
    private Button respawnButton;
    private Button repairButton;
    private MirrorButton mirrorButton;
    private DriveMode driveMode;
    #endregion

    private float handbrakeRot;

    public GameMainPanel Construct(RCC_CarControllerV3 rcc)
    {
        this.rcc = rcc;
        if (this.rcc == null) this.rcc = LevelManager.Instance.car.rcc;

        RCC_Settings.Instance.mobileControllerEnabled = GameManager.Instance.onMobileDevice;
        switch (GameManager.Instance.dataManager.user.gameData.settings.controllerType)
        {
            case 0: RCC.SetMobileController(RCC_Settings.MobileController.SteeringWheel); break;
            case 1: RCC.SetMobileController(RCC_Settings.MobileController.Joystick); break;
            case 2: RCC.SetMobileController(RCC_Settings.MobileController.TouchScreen); break;
            case 3: RCC.SetMobileController(RCC_Settings.MobileController.Gyro); break;
        }

        if (rccButtons != null) Destroy(rccButtons);
        rccButtons = gameObject.AddComponent<RCC_MobileButtons>();

        if (!dashboard) dashboard = GetComponentInChildren<EKDashboard>().Construct(this.rcc);
        if (!pauseButton) pauseButton = pauseButtonRect.GetButton(OnPressedPauseButton);
        if (!changeCameraButton) changeCameraButton = changeCameraButtonRect.GetButton(OnPressedChangeCameraButton);
        if (!respawnButton) respawnButton = respawnButtonRect.GetButton(OnPressedRespawnButton);
        if (!repairButton) repairButton = repairButtonRect.GetButton(OnPressedRepairButton);
        if (!mirrorButton) mirrorButton = mirrorButtonRect.gameObject.AddComponent<MirrorButton>().Construct(OnPressedMirrorButton, OnReleasedMirrorButton);

        if (driveMode == null)
        {
            driveMode = new DriveMode();
            driveMode.button = driveModeRect.GetButton(OnPressedDriveModeButton);
            driveMode.image = driveModeRect.GetComponentInChildren<Image>();
            driveMode.text = driveModeRect.GetComponentInChildren<Text>();
            LevelManager.Instance.car.carMode.onModeChanged.AddListener(SetDriveModeUI);
            SetDriveModeUI(LevelManager.Instance.car.carMode.mode);
        }

        Construct();

        GameManager.Instance.analyticManager.Showed("game-main-panel");
        UIManager.Instance.Push(TutorialDriftMode.Get(driveModeRect));

        return this;
    }

    private void Construct()
    {
        gasButtonController = gasButtonRect.GetComponent<RCC_UIController>();
        rccButtons.gasButton = gasButtonController;

        handbrakeButtonController = handbrakeButtonRect.GetComponent<RCC_UIController>();
        rccButtons.handbrakeButton = handbrakeButtonController;

        SetMiniMap(GameManager.Instance.statics.miniMapMode);

        LevelManager.Instance.cameraTilt.enabled = false;
        switch (GameManager.Instance.dataManager.user.gameData.settings.controllerType)
        {
            case 0: // steering wheel
                steeringWheelRect.gameObject.SetActive(true);
                joystickRect.gameObject.SetActive(false);
                leftButtonRect.gameObject.SetActive(false);
                rightButtonRect.gameObject.SetActive(false);

                steeringWheelController = steeringWheelRect.GetComponent<RCC_UISteeringWheelController>();
                rccButtons.steeringWheel = steeringWheelController;

                brakeButtonLeftRect.gameObject.SetActive(false);
                brakeButtonRightRect.gameObject.SetActive(true);
                brakeButtonRect = brakeButtonRightRect;
                brakeButtonController = brakeButtonRect.GetComponent<RCC_UIController>();
                rccButtons.brakeButton = brakeButtonController;
                break;
            case 1: // joystick
                steeringWheelRect.gameObject.SetActive(false);
                joystickRect.gameObject.SetActive(true);
                leftButtonRect.gameObject.SetActive(false);
                rightButtonRect.gameObject.SetActive(false);

                joystickController = joystickRect.GetComponent<RCC_UIJoystick>();
                rccButtons.joystick = joystickController;

                brakeButtonLeftRect.gameObject.SetActive(false);
                brakeButtonRightRect.gameObject.SetActive(true);
                brakeButtonRect = brakeButtonRightRect;
                brakeButtonController = brakeButtonRect.GetComponent<RCC_UIController>();
                rccButtons.brakeButton = brakeButtonController;
                break;
            case 2: // buttons
                steeringWheelRect.gameObject.SetActive(false);
                joystickRect.gameObject.SetActive(false);
                leftButtonRect.gameObject.SetActive(true);
                rightButtonRect.gameObject.SetActive(true);

                leftButtonController = leftButtonRect.GetComponent<RCC_UIController>();
                rightButtonController = rightButtonRect.GetComponent<RCC_UIController>();

                rccButtons.leftButton = leftButtonController;
                rccButtons.rightButton = rightButtonController;

                rccButtons.leftButton = leftButtonController;
                rccButtons.rightButton = rightButtonController;

                brakeButtonLeftRect.gameObject.SetActive(false);
                brakeButtonRightRect.gameObject.SetActive(true);
                brakeButtonRect = brakeButtonRightRect;
                brakeButtonController = brakeButtonRect.GetComponent<RCC_UIController>();
                rccButtons.brakeButton = brakeButtonController;
                break;
            case 3: //tilt
                steeringWheelRect.gameObject.SetActive(false);
                joystickRect.gameObject.SetActive(false);
                leftButtonRect.gameObject.SetActive(false);
                rightButtonRect.gameObject.SetActive(false);

                brakeButtonLeftRect.gameObject.SetActive(true);
                brakeButtonRightRect.gameObject.SetActive(false);
                brakeButtonRect = brakeButtonLeftRect;
                brakeButtonController = brakeButtonRect.GetComponent<RCC_UIController>();
                rccButtons.brakeButton = brakeButtonController;
                LevelManager.Instance.cameraTilt.enabled = true;
                break;
        }
        handbrakeRot = handbrakeButtonRect.GetChild(0).localEulerAngles.z;

        if (!GameManager.Instance.onMobileDevice)
        {
            if (steeringWheelController) steeringWheelController.enabled = false;
            if (joystickController) joystickController.enabled = false;
        }
    }

    private void SetMiniMap(int miniMapMode)
    {
        if (miniMapMode == -1) return;

        RectTransform r = Instantiate(Resources.Load<GameObject>("_ui/_game/minimap"), driveModeRect.parent).GetComponent<RectTransform>();
        r.anchorMin = r.anchorMax = r.pivot = new Vector2(0, 1f);
        r.anchoredPosition = new Vector2(50f, -50f);

        RectTransform dr = dashboard.GetComponent<RectTransform>();
        dr.anchorMin = dr.anchorMax = dr.pivot = new Vector2(0.5f, 0f);
        dr.anchoredPosition = new Vector2(0f, 50f);

        if (miniMapMode == 1)
        {
            r.Find("border-black").gameObject.SetActive(false);
        }

        if (driveMode != null) driveModeRect.anchoredPosition = new Vector2(50f, -385f);
    }

    private void LateUpdate()
    {
        gasButtonRect.GetChild(0).localScale =
                            new Vector3(1f, Mathf.Lerp(gasButtonRect.GetChild(0).localScale.y, gasButtonController.pressing ? 0.75f : 1f, Time.deltaTime * 10f), 1f);
        brakeButtonRect.GetChild(0).localScale =
                            new Vector3(1f, Mathf.Lerp(brakeButtonRect.GetChild(0).localScale.y, brakeButtonController.pressing ? 0.75f : 1f, Time.deltaTime * 10f), 1f);
        handbrakeButtonRect.GetChild(0).localEulerAngles =
                            new Vector3(0f, 0f, Mathf.Lerp(handbrakeButtonRect.GetChild(0).localEulerAngles.z, handbrakeButtonController.pressing ? handbrakeRot - 20f : handbrakeRot, Time.deltaTime * 20f));

        if (leftButtonRect.gameObject.activeInHierarchy)
        {
            leftButtonRect.GetChild(0).localScale = Vector3.one * Mathf.Lerp(leftButtonRect.GetChild(0).localScale.x, (leftButtonController.pressing ? 0.75f : 1f), Time.deltaTime * 10f);
            rightButtonRect.GetChild(0).localScale = Vector3.one * Mathf.Lerp(rightButtonRect.GetChild(0).localScale.x, (rightButtonController.pressing ? 0.75f : 1f), Time.deltaTime * 10f);
        }

        if (!GameManager.Instance.onMobileDevice) SimulateButtons();
    }

    private void SimulateButtons()
    {
        gasButtonRect.GetChild(0).localScale =
                    new Vector3(1f, Mathf.Lerp(gasButtonRect.GetChild(0).localScale.y, rcc.throttleInput > 0f ? 0.75f : 1f, Time.deltaTime * 10f), 1f);
        brakeButtonRect.GetChild(0).localScale =
                            new Vector3(1f, Mathf.Lerp(brakeButtonRect.GetChild(0).localScale.y, rcc.brakeInput > 0f ? 0.75f : 1f, Time.deltaTime * 10f), 1f);
        handbrakeButtonRect.GetChild(0).localEulerAngles =
                            new Vector3(0f, 0f, Mathf.Lerp(handbrakeButtonRect.GetChild(0).localEulerAngles.z, rcc.handbrakeInput > 0f ? handbrakeRot - 20f : handbrakeRot, Time.deltaTime * 20f));

        if (leftButtonRect.gameObject.activeInHierarchy)
        {
            leftButtonRect.GetChild(0).localScale = Vector3.one * Mathf.Lerp(leftButtonRect.GetChild(0).localScale.x, (rcc.steerInput < 0 ? 0.75f : 1f), Time.deltaTime * 10f);
            rightButtonRect.GetChild(0).localScale = Vector3.one * Mathf.Lerp(rightButtonRect.GetChild(0).localScale.x, (rcc.steerInput > 0 ? 0.75f : 1f), Time.deltaTime * 10f);
        }
        else if (joystickRect.gameObject.activeInHierarchy)
        {
            joystickController.handleSprite.anchoredPosition = new Vector2((joystickRect.rect.size.x / 2f) * rcc.steerInput, 0f);
        }
        else if (steeringWheelRect.gameObject.activeInHierarchy)
        {
            steeringWheelRect.localRotation = Quaternion.Euler(0, 0, rcc.steerInput * -120f);
        }
    }

    private void OnPressedPauseButton()
    {
        LevelManager.Instance?.onGamePaused?.Invoke();
        UIManager.Instance.Push(GamePauseMenu.Get(this));

        GameManager.Instance.analyticManager.Clicked("game-main-panel", "pause-button");
        
        if (GameManager.Instance.statics.pauseButtonAdsEnable) GameManager.Instance.adManager.ShowInterstitial();
    }

    private void OnPressedChangeCameraButton()
    {
        RCC.ChangeCamera();
    }

    private void OnPressedRespawnButton()
    {
        LevelManager.Instance.respawning = true;
        Vector3 p = LevelManager.Instance.car.transform.position;
        p.y = LevelManager.Instance.transform.position.y + 3f;
        Quaternion r = LevelManager.Instance.car.transform.rotation;
        r.x = 0f; r.z = 0f;
        RCC.Transport(LevelManager.Instance.car.rcc, p, r);
    }

    private void OnPressedRepairButton()
    {
        RCC.Repair(rcc);
    }

    private void OnPressedMirrorButton()
    {
        LevelManager.Instance.rccCamera.lookBack = true;
    }

    private void OnReleasedMirrorButton()
    {
        LevelManager.Instance.rccCamera.lookBack = false;
    }

    private void OnPressedDriveModeButton()
    {
        switch (LevelManager.Instance.car.carMode.mode)
        {
            case CarModeSetter.CarDriveMode.Sim: LevelManager.Instance.car.carMode.onModeChanged.Invoke(CarModeSetter.CarDriveMode.Drift); break;
            case CarModeSetter.CarDriveMode.Drift: LevelManager.Instance.car.carMode.onModeChanged.Invoke(CarModeSetter.CarDriveMode.Sim); break;
        }
    }

    private void SetDriveModeUI(CarModeSetter.CarDriveMode mode)
    {
        string st = "";
        Sprite spr = null;
        Color textColor = Color.white;
        switch (mode)
        {
            case CarModeSetter.CarDriveMode.Sim:
                st = "TRACTION";
                textColor = Color.green;
                spr = Resources.Load<Sprite>("_sprites/_game/traction");
                break;
            case CarModeSetter.CarDriveMode.Drift:
                st = "DRIFT";
                textColor = Color.red;
                spr = Resources.Load<Sprite>("_sprites/_game/drift");
                break;
        }

        driveMode.text.text = st;
        driveMode.text.color = textColor;
        driveMode.image.color = textColor;
        driveMode.image.sprite = spr;
    }

    private void OnDestroy()
    {
        pauseButton?.onClick.RemoveAllListeners();
        respawnButton?.onClick.RemoveAllListeners();
        repairButton?.onClick.RemoveAllListeners();
        changeCameraButton?.onClick.RemoveAllListeners();
    }

    private void OnApplicationPause(bool pauseStatus)
    {
        if (!pauseStatus) return;
        // LevelManager.Instance?.onGamePaused?.Invoke();
        // UIManager.Instance.Push(GamePauseMenu.Get(this, false));
    }

    public static EKCanvas Get(RCC_CarControllerV3 rcc)
    {
        GameObject prefab = Resources.Load<GameObject>("_ui/_game/main-canvas");
        GameMainPanel gmp = Instantiate(prefab).GetComponent<GameMainPanel>().Construct(rcc);
        return gmp.gameObject.AddComponent<EKCanvas>().Construct(null);
    }

    [System.Serializable]
    public class DriveMode
    {
        public Button button;
        public Image image;
        public Text text;
    }
}