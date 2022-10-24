using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using DG.Tweening;
using Utils.UI;

public class UpgradePanel : MonoBehaviour
{
    public RectTransform panel;
    public RectTransform closeButtonRect;

    private UpgradeButton engineButton;
    private UpgradeButton transmissionButton;
    private UpgradeButton steeringButton;
    private UpgradeButton brakesButton;

    private Button closeButton;

    private readonly int maxLevel = 9;
    private CarData car;

    private UnityAction<bool> onComplete;
    private ShowroomCameraManager.CameraPoint cameraPoint;

    public UpgradePanel Construct(UnityAction<bool> onComplete = null)
    {
        car = ShowroomManager.Instance.current.data;
        Price price;

        price = new Price(car.costs.engine); price.amount *= (car.upgrades.engine + 1);
        engineButton = transform
                                .Find("panel/engine-button")
                                .GetComponent<UpgradeButton>()
                                .Construct(car.upgrades.engine, maxLevel, price, OnPressedEngineButton);

        price = new Price(car.costs.transmission); price.amount *= (car.upgrades.transmission + 1);
        transmissionButton = transform
                                    .Find("panel/transmission-button")
                                    .GetComponent<UpgradeButton>()
                                    .Construct(car.upgrades.transmission, maxLevel, price, OnPressedTransmissionButton);

        price = new Price(car.costs.steering); price.amount *= (car.upgrades.steering + 1);
        steeringButton = transform
                                .Find("panel/steering-button")
                                .GetComponent<UpgradeButton>()
                                .Construct(car.upgrades.steering, maxLevel, price, OnPressedSteeringButton);

        price = new Price(car.costs.brakes); price.amount *= (car.upgrades.brakes + 1);
        brakesButton = transform
                                .Find("panel/brakes-button")
                                .GetComponent<UpgradeButton>()
                                .Construct(car.upgrades.brakes, maxLevel, price, OnPressedBrakesButton);

        closeButton = closeButtonRect.GetButton(OnPressedCloseButton);

        panel.anchoredPosition += new Vector2(0f, -1000f);

        this.onComplete = onComplete;
        this.onComplete?.Invoke(true);

        cameraPoint = ShowroomCameraManager.Instance.GetCurrentPoint();
        ShowroomCameraManager.Instance.Focus("body-paint", GameManager.Instance.statics.cameraFocusDuration, () => ShowroomCameraManager.Instance.orbitable = false);
        GameManager.Instance.analyticManager.Showed("upgrade-panel");
        return this;
    }

    private void Start()
    {
        Appear();
    }

    private void OnPressedCloseButton()
    {
        closeButton?.onClick.RemoveAllListeners();
        Disappear();
        GameManager.Instance.analyticManager.Clicked("upgrade-panel", "back-button");
    }

    private void OnPressedEngineButton(Price price, bool video)
    {
        if (!video)
        {
            GameManager.Instance.currencyManager.PayWithoutConfirmation(price, b =>
            {
                if (b)
                {
                    car.upgrades.engine++;
                    GameManager.Instance.dataManager.SaveCar(car);
                    GameManager.Instance.analyticManager.Clicked("upgrade-panel", "engine-button");
                }
            });
            SetButtons();
        }
        else
        {
            GameManager.Instance.adManager.ShowRewarded(b =>
            {
                if (b)
                {
                    car.upgrades.engine++;
                    GameManager.Instance.dataManager.SaveCar(ShowroomManager.Instance.current.data);
                    SetButtons();
                    GameManager.Instance.analyticManager.Clicked("upgrade-panel", "engine-button");
                }
            });
        }
    }

    private void OnPressedTransmissionButton(Price price, bool video)
    {
        if (!video)
        {
            GameManager.Instance.currencyManager.PayWithoutConfirmation(price, b =>
            {
                if (b)
                {
                    car.upgrades.transmission++;
                    GameManager.Instance.dataManager.SaveCar(car);
                    SetButtons();
                    GameManager.Instance.analyticManager.Clicked("upgrade-panel", "transmission-button");
                }
            });
        }
        else
        {
            GameManager.Instance.adManager.ShowRewarded(b =>
            {
                if (b)
                {
                    car.upgrades.transmission++;
                    GameManager.Instance.dataManager.SaveCar(car);
                    SetButtons();
                    GameManager.Instance.analyticManager.Clicked("upgrade-panel", "transmission-button");
                }
            });
        }
    }

    private void OnPressedSteeringButton(Price price, bool video)
    {
        if (!video)
        {
            GameManager.Instance.currencyManager.PayWithoutConfirmation(price, b =>
            {
                if (b)
                {
                    car.upgrades.steering++;
                    GameManager.Instance.dataManager.SaveCar(car);
                    SetButtons();
                    GameManager.Instance.analyticManager.Clicked("upgrade-panel", "steering-button");
                }
            });
        }
        else
        {
            GameManager.Instance.adManager.ShowRewarded(b =>
            {
                if (b)
                {
                    car.upgrades.steering++;
                    GameManager.Instance.dataManager.SaveCar(car);
                    SetButtons();
                    GameManager.Instance.analyticManager.Clicked("upgrade-panel", "steering-button");
                }
            });
        }
    }

    private void OnPressedBrakesButton(Price price, bool video)
    {
        if (!video)
        {
            GameManager.Instance.currencyManager.PayWithoutConfirmation(price, b =>
            {
                if (b)
                {
                    car.upgrades.brakes++;
                    GameManager.Instance.dataManager.SaveCar(car);
                    SetButtons();
                    GameManager.Instance.analyticManager.Clicked("upgrade-panel", "brakes-button");
                }
            });
        }
        else
        {
            GameManager.Instance.adManager.ShowRewarded(b =>
            {
                if (b)
                {
                    car.upgrades.brakes++;
                    GameManager.Instance.dataManager.SaveCar(car);
                    SetButtons();
                    GameManager.Instance.analyticManager.Clicked("upgrade-panel", "brakes-button");
                }
            });
        }
    }

    private void SetButtons()
    {
        Price newPrice;

        newPrice = new Price(car.costs.engine); newPrice.amount *= (car.upgrades.engine + 1);
        engineButton.Set(car.upgrades.engine, maxLevel, newPrice);

        newPrice = new Price(car.costs.transmission); newPrice.amount *= (car.upgrades.transmission + 1);
        transmissionButton.Set(car.upgrades.transmission, maxLevel, newPrice);

        newPrice = new Price(car.costs.steering); newPrice.amount *= (car.upgrades.steering + 1);
        steeringButton.Set(car.upgrades.steering, maxLevel, newPrice);

        newPrice = new Price(car.costs.brakes); newPrice.amount *= (car.upgrades.brakes + 1);
        brakesButton.Set(car.upgrades.brakes, maxLevel, newPrice);

        ShowroomManager.Instance.current.onUpgraded.Invoke();

        GameManager.Instance.audioManager.Play("repair-1");
        GameManager.Instance.audioManager.Play("repair-2");
    }

    private void Appear(float duration = 0.25f)
    {
        ShowroomManager.Instance.current.SetHood();
        panel.DOAnchorPosY(panel.anchoredPosition.y + 1000f, duration);
    }

    private void Disappear(float duration = 0.25f)
    {
        ShowroomManager.Instance.current.SetHood();
        onComplete?.Invoke(false);

        ShowroomCameraManager.Instance.orbitable = true;
        ShowroomCameraManager.Instance.Focus(cameraPoint);

        panel
        .DOAnchorPosY(panel.anchoredPosition.y - 1000f, duration)
        .OnComplete(() =>
        {
            Destroy(gameObject);
        });
    }

    private void OnDestroy()
    {
        closeButton?.onClick.RemoveAllListeners();
    }

    public static EKCanvas Get(UnityAction<bool> onComplete = null)
    {
        GameObject prefab = Resources.Load<GameObject>("_ui/_showroom/upgrade-canvas");
        UpgradePanel up = Instantiate(prefab).GetComponent<UpgradePanel>().Construct(onComplete);
        return up.gameObject.AddComponent<EKCanvas>().Construct(null);
    }
}