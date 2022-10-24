using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using DG.Tweening;
using Utils.UI;
using Utils.Strings;

public class RimPanel : MonoBehaviour
{
    public RectTransform tintSliderRect;
    public RectTransform pickerRect;
    public RectTransform backButtonRect;
    public RectTransform applyButtonRect;
    public RectTransform costPanelRect;
    public RectTransform costTextRect;
    public RectTransform costIconRect;
    public RimSelector rimSelector;

    #region Components
    private CustomSlider tintSlider;
    private ColorPicker picker;
    private Button backButton;
    private Button applyButton;
    private Text costText;
    private Image costIcon;
    #endregion

    private UnityAction onComplete;
    private int cost;
    private int rimIndex;
    private int rimCount = 10;
    private float tint;
    private Color color;

    private ShowroomCameraManager.CameraPoint cameraPoint;

    public RimPanel Construct(UnityAction onComplete = null)
    {
        this.onComplete = onComplete;

        tint = ShowroomManager.Instance.current.data.visual.rim.tint;
        color = ShowroomManager.Instance.current.data.visual.rim.color.GetColor();

        tintSlider = tintSliderRect.GetComponent<CustomSlider>().Construct(tint, OnTintChanged);
        picker = pickerRect.GetComponent<ColorPicker>().Construct(OnPickerChanged, color);
        backButton = backButtonRect.GetButton(OnPressedBackButton);
        applyButton = applyButtonRect.GetButton(OnPressedApplyButton);
        costText = costTextRect.GetComponent<Text>();
        costIcon = costIconRect.GetComponent<Image>();

        rimIndex = ShowroomManager.Instance.current.data.rimIndex;
        rimSelector.Construct(OnPressedPrevRim, OnPressedNextRim, rimIndex, rimCount);

        costText.text = "0";
        costIcon.color = ShowroomManager.Instance.current.data.costs.rim.type == 0 ? GameManager.Instance.statics.dollarColor : GameManager.Instance.statics.diamondColor;
        costIcon.sprite = ShowroomManager.Instance.current.data.costs.rim.type == 0 ? GameManager.Instance.statics.dollarSprite : GameManager.Instance.statics.diamondSprite;

        Appear();

        cameraPoint = ShowroomCameraManager.Instance.GetCurrentPoint();
        ShowroomCameraManager.Instance.Focus("rim", GameManager.Instance.statics.cameraFocusDuration);
        GameManager.Instance.analyticManager.Showed("customize-rim-panel");
        return this;
    }

    private void Appear(float duration = 0.25f)
    {
        tintSliderRect.anchoredPosition += new Vector2(-1000f, 0f);
        pickerRect.anchoredPosition += new Vector2(1000f, 0f);
        backButtonRect.anchoredPosition += new Vector2(0f, -1000f);
        applyButtonRect.anchoredPosition += new Vector2(0f, -1000f);
        costPanelRect.anchoredPosition += new Vector2(0f, -1000f);
        rimSelector.rect.anchoredPosition += new Vector2(0f, -1000f);

        tintSliderRect.DOAnchorPosX(tintSliderRect.anchoredPosition.x + 1000f, duration);
        pickerRect.DOAnchorPosX(pickerRect.anchoredPosition.x - 1000f, duration);
        backButtonRect.DOAnchorPosY(backButtonRect.anchoredPosition.y + 1000f, duration);
        applyButtonRect.DOAnchorPosY(applyButtonRect.anchoredPosition.y + 1000f, duration);
        costPanelRect.DOAnchorPosY(costPanelRect.anchoredPosition.y + 1000f, duration);
        rimSelector.rect.DOAnchorPosY(rimSelector.rect.anchoredPosition.y + 1000f, duration);
    }

    private void Disappear(float duration = 0.25f)
    {
        tintSliderRect.DOComplete();
        pickerRect.DOComplete();
        backButtonRect.DOComplete();
        applyButtonRect.DOComplete();
        costPanelRect.DOComplete();
        rimSelector.rect.DOComplete();

        tintSliderRect.DOAnchorPosX(tintSliderRect.anchoredPosition.x - 1000f, duration);
        pickerRect.DOAnchorPosX(pickerRect.anchoredPosition.x + 1000f, duration);
        backButtonRect.DOAnchorPosY(backButtonRect.anchoredPosition.y - 1000f, duration);
        applyButtonRect.DOAnchorPosY(applyButtonRect.anchoredPosition.y - 1000f, duration);
        costPanelRect.DOAnchorPosY(costPanelRect.anchoredPosition.y - 1000f, duration);
        rimSelector.rect.DOAnchorPosY(rimSelector.rect.anchoredPosition.y - 1000f, duration).OnComplete(() => Destroy(gameObject));
    }

    private void OnPressedBackButton()
    {
        onComplete?.Invoke();
        ShowroomManager.Instance.current.paintSystem.ReturnOriginal();
        ShowroomManager.Instance.current.onWheelChanged.Invoke(-1);

        ShowroomCameraManager.Instance.Focus(cameraPoint, GameManager.Instance.statics.cameraFocusDuration);
        Disappear();
        GameManager.Instance.audioManager.Play("tap");
        GameManager.Instance.analyticManager.Clicked("customize-rim-panel", "back-button");
    }

    private void OnPressedApplyButton()
    {
        GameManager
        .Instance
        .currencyManager
        .Pay(ShowroomManager.Instance.current.data.costs.rim, b =>
        {
            if (b)
            {
                ShowroomManager.Instance.current.paintSystem.Save();
                ShowroomManager.Instance.current.data.rimIndex = rimIndex;
                GameManager.Instance.dataManager.SaveUser();
                OnPressedBackButton();
                GameManager.Instance.analyticManager.Clicked("customize-rim-panel", "apply-button");
            }
        });
        GameManager.Instance.audioManager.Play("tap");
    }

    private void OnPickerChanged(Color color)
    {
        cost = ShowroomManager.Instance.current.data.costs.rim.amount;
        costText.text = cost.ToCurrencyString();

        this.color = color;
        OnColorChanged();
    }

    private void OnTintChanged(float val)
    {
        cost = ShowroomManager.Instance.current.data.costs.rim.amount;
        costText.text = cost.ToCurrencyString();

        this.tint = val;
        OnColorChanged();
    }

    private void OnColorChanged()
    {
        ShowroomManager.Instance.current.paintSystem.rimPaint.SetColor(color, tint);
    }

    private void OnPressedNextRim()
    {
        rimIndex++;
        if (rimIndex >= rimCount) rimIndex = 0;
        rimSelector.SetText(rimIndex, rimCount);
        OnRimChanged();
        GameManager.Instance.analyticManager.Clicked("customize-rim-panel", "next-button");
    }

    private void OnPressedPrevRim()
    {
        rimIndex--;
        if (rimIndex < 0) rimIndex = rimCount - 1;
        rimSelector.SetText(rimIndex, rimCount);
        OnRimChanged();
        GameManager.Instance.analyticManager.Clicked("customize-rim-panel", "prev-button");
    }

    private void OnRimChanged()
    {
        ShowroomManager.Instance.current.onWheelChanged.Invoke(rimIndex);
        cost = ShowroomManager.Instance.current.data.costs.rim.amount;
        costText.text = cost.ToCurrencyString();
    }

    private void OnDestroy()
    {
        backButton?.onClick.RemoveAllListeners();
        applyButton?.onClick.RemoveAllListeners();
        onComplete = null;
    }

    public static EKCanvas Get(UnityAction onComplete = null)
    {
        GameObject prefab = Resources.Load<GameObject>("_ui/_showroom/_customize-panel/rim-panel");
        RimPanel bp = Instantiate(prefab).GetComponent<RimPanel>().Construct(onComplete);
        return bp.gameObject.AddComponent<EKCanvas>().Construct(null);
    }

    [System.Serializable]
    public class RimSelector
    {
        public RectTransform rect;
        public RectTransform prevButtonRect;
        public RectTransform nextButtonRect;
        public RectTransform statusTextRect;

        [HideInInspector] public Button prevButton;
        [HideInInspector] public Button nextButton;
        [HideInInspector] public Text statusText;

        public void Construct(UnityAction onPressedPrev, UnityAction onPressedNext, int rimIndex, int rimCount)
        {
            prevButton = prevButtonRect.GetButton(onPressedPrev);
            nextButton = nextButtonRect.GetButton(onPressedNext);
            statusText = statusTextRect.GetComponent<Text>();
            SetText(rimIndex, rimCount);
        }

        public void SetText(int rimIndex, int rimCount)
        {
            statusText.text = (rimIndex + 1) + " / " + rimCount;
        }
    }
}