using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using DG.Tweening;
using Utils.UI;
using Utils.Strings;

public class SuspensionPanel : MonoBehaviour
{
    public RectTransform frontSuspensionSliderRect;
    public RectTransform rearSuspensionSliderRect;
    public RectTransform frontOffsetSliderRect;
    public RectTransform rearOffsetSliderRect;
    public RectTransform frontCamberSliderRect;
    public RectTransform rearCamberSliderRect;

    public RectTransform backButtonRect;
    public RectTransform applyButtonRect;
    public RectTransform costPanelRect;
    public RectTransform costTextRect;
    public RectTransform costIconRect;

    #region Components
    private CustomSlider frontSuspensionSlider;
    private CustomSlider rearSuspensionSlider;
    private CustomSlider frontOffsetSlider;
    private CustomSlider rearOffsetSlider;
    private CustomSlider frontCamberSlider;
    private CustomSlider rearCamberSlider;
    private Button backButton;
    private Button applyButton;
    private Text costText;
    private Image costIcon;
    #endregion

    private UnityAction onComplete;
    private int cost;
    private Axles axles;

    private ShowroomCameraManager.CameraPoint cameraPoint;

    public SuspensionPanel Construct(UnityAction onComplete = null)
    {
        this.onComplete = onComplete;

        CarData car = ShowroomManager.Instance.current.data;
        axles = new Axles(car.axles);

        frontSuspensionSlider = frontSuspensionSliderRect.GetComponent<CustomSlider>().Construct(axles.front.suspensionDistance, OnFrontSuspensionChanged);
        rearSuspensionSlider = rearSuspensionSliderRect.GetComponent<CustomSlider>().Construct(axles.rear.suspensionDistance, OnRearSuspensionChanged);
        frontOffsetSlider = frontOffsetSliderRect.GetComponent<CustomSlider>().Construct(axles.front.wheelOffset, OnFrontWheelOffsetChanged); ;
        rearOffsetSlider = rearOffsetSliderRect.GetComponent<CustomSlider>().Construct(axles.rear.wheelOffset, OnRearWheelOffsetChanged);
        frontCamberSlider = frontCamberSliderRect.GetComponent<CustomSlider>().Construct(axles.front.wheelCamber, OnFrontWheelCamberChanged);
        rearCamberSlider = rearCamberSliderRect.GetComponent<CustomSlider>().Construct(axles.rear.wheelCamber, OnRearWheelCamberChanged);

        backButton = backButtonRect.GetButton(OnPressedBackButton);
        applyButton = applyButtonRect.GetButton(OnPressedApplyButton);
        costText = costTextRect.GetComponent<Text>();
        costIcon = costIconRect.GetComponent<Image>();

        costText.text = "0";
        costIcon.color = ShowroomManager.Instance.current.data.costs.body.type == 0 ? GameManager.Instance.statics.dollarColor : GameManager.Instance.statics.diamondColor;
        costIcon.sprite = ShowroomManager.Instance.current.data.costs.body.type == 0 ? GameManager.Instance.statics.dollarSprite : GameManager.Instance.statics.diamondSprite;

        Appear();

        cameraPoint = ShowroomCameraManager.Instance.GetCurrentPoint();
        ShowroomCameraManager.Instance.Focus("suspension", GameManager.Instance.statics.cameraFocusDuration);
        ShowroomManager.Instance.current.showroomMode.steer = false;
        GameManager.Instance.analyticManager.Showed("customize-suspension-panel");
        return this;
    }

    private void Appear(float duration = 0.25f)
    {
        backButtonRect.anchoredPosition += new Vector2(0f, -1000f);
        applyButtonRect.anchoredPosition += new Vector2(0f, -1000f);
        costPanelRect.anchoredPosition += new Vector2(0f, -1000f);

        frontSuspensionSliderRect.anchoredPosition += new Vector2(-1000f, 0f);
        rearSuspensionSliderRect.anchoredPosition += new Vector2(1000f, 0f);
        frontOffsetSliderRect.anchoredPosition += new Vector2(0f, -1000f);
        rearOffsetSliderRect.anchoredPosition += new Vector2(0f, -1000f);
        frontCamberSliderRect.anchoredPosition += new Vector2(0f, -1000f);
        rearCamberSliderRect.anchoredPosition += new Vector2(0f, -1000f);

        backButtonRect.DOAnchorPosY(backButtonRect.anchoredPosition.y + 1000f, duration);
        applyButtonRect.DOAnchorPosY(applyButtonRect.anchoredPosition.y + 1000f, duration);
        costPanelRect.DOAnchorPosY(costPanelRect.anchoredPosition.y + 1000f, duration);

        frontSuspensionSliderRect.DOAnchorPosX(frontSuspensionSliderRect.anchoredPosition.x + 1000f, duration);
        rearSuspensionSliderRect.DOAnchorPosX(rearSuspensionSliderRect.anchoredPosition.x - 1000f, duration);
        frontCamberSliderRect.DOAnchorPosY(frontCamberSliderRect.anchoredPosition.y + 1000f, duration);
        rearCamberSliderRect.DOAnchorPosY(rearCamberSliderRect.anchoredPosition.y + 1000f, duration);
        frontOffsetSliderRect.DOAnchorPosY(frontOffsetSliderRect.anchoredPosition.y + 1000f, duration);
        rearOffsetSliderRect.DOAnchorPosY(rearOffsetSliderRect.anchoredPosition.y + 1000f, duration);
    }

    private void Disappear(float duration = 0.25f)
    {
        backButtonRect.DOComplete();
        applyButtonRect.DOComplete();
        costPanelRect.DOComplete();
        frontSuspensionSliderRect.DOComplete();
        rearSuspensionSliderRect.DOComplete();
        frontOffsetSliderRect.DOComplete();
        rearOffsetSliderRect.DOComplete();
        frontCamberSliderRect.DOComplete();
        rearCamberSliderRect.DOComplete();

        backButtonRect.DOAnchorPosY(backButtonRect.anchoredPosition.y - 1000f, duration);
        applyButtonRect.DOAnchorPosY(applyButtonRect.anchoredPosition.y - 1000f, duration);
        costPanelRect.DOAnchorPosY(costPanelRect.anchoredPosition.y - 1000f, duration);

        frontSuspensionSliderRect.DOAnchorPosX(frontSuspensionSliderRect.anchoredPosition.x - 1000f, duration);
        rearSuspensionSliderRect.DOAnchorPosX(rearSuspensionSliderRect.anchoredPosition.x + 1000f, duration);
        frontCamberSliderRect.DOAnchorPosY(frontCamberSliderRect.anchoredPosition.y - 1000f, duration);
        rearCamberSliderRect.DOAnchorPosY(rearCamberSliderRect.anchoredPosition.y - 1000f, duration);
        frontOffsetSliderRect.DOAnchorPosY(frontOffsetSliderRect.anchoredPosition.y - 1000f, duration);
        rearOffsetSliderRect.DOAnchorPosY(rearOffsetSliderRect.anchoredPosition.y - 1000f, duration).OnComplete(() => Destroy(gameObject));
    }

    private void OnPressedBackButton()
    {
        onComplete?.Invoke();
        ShowroomManager.Instance.current.onAxlesChanged.Invoke(ShowroomManager.Instance.current.data.axles);
        ShowroomCameraManager.Instance.Focus(cameraPoint, GameManager.Instance.statics.cameraFocusDuration);
        ShowroomManager.Instance.current.showroomMode.steer = true;
        Disappear();
        GameManager.Instance.audioManager.Play("tap");
        GameManager.Instance.analyticManager.Clicked("customize-suspension-panel", "back-button");
    }

    private void OnPressedApplyButton()
    {
        GameManager
        .Instance
        .currencyManager
        .Pay(ShowroomManager.Instance.current.data.costs.suspension, b =>
        {
            if (b)
            {
                ShowroomManager.Instance.current.data.axles = axles;
                GameManager.Instance.dataManager.SaveUser();
                OnPressedBackButton();
                GameManager.Instance.analyticManager.Clicked("customize-suspension-panel", "apply-button");
            }
        });
        GameManager.Instance.audioManager.Play("tap");
    }

    #region SliderSubs
    private void OnFrontSuspensionChanged(float val)
    {
        axles.front.suspensionDistance = val;
        OnValueChanged();
    }

    private void OnRearSuspensionChanged(float val)
    {
        axles.rear.suspensionDistance = val;
        OnValueChanged();
    }

    private void OnFrontWheelOffsetChanged(float val)
    {
        axles.front.wheelOffset = val;
        OnValueChanged();
    }

    private void OnRearWheelOffsetChanged(float val)
    {
        axles.rear.wheelOffset = val;
        OnValueChanged();
    }

    private void OnFrontWheelCamberChanged(float val)
    {
        axles.front.wheelCamber = val;
        OnValueChanged();
    }

    private void OnRearWheelCamberChanged(float val)
    {
        axles.rear.wheelCamber = val;
        OnValueChanged();
    }
    #endregion

    private void OnValueChanged()
    {
        cost = ShowroomManager.Instance.current.data.costs.suspension.amount;
        costText.text = cost.ToCurrencyString();

        ShowroomManager.Instance.current.onAxlesChanged.Invoke(axles);
    }

    private void OnDestroy()
    {
        backButton?.onClick.RemoveAllListeners();
        applyButton?.onClick.RemoveAllListeners();
        onComplete = null;
    }

    public static EKCanvas Get(UnityAction onComplete = null)
    {
        GameObject prefab = Resources.Load<GameObject>("_ui/_showroom/_customize-panel/suspension-panel");
        SuspensionPanel bp = Instantiate(prefab).GetComponent<SuspensionPanel>().Construct(onComplete);
        return bp.gameObject.AddComponent<EKCanvas>().Construct(null);
    }
}