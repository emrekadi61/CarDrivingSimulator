using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using DG.Tweening;
using Utils.UI;
using Utils.Strings;

public class BodyPanel : MonoBehaviour
{
    public RectTransform tintSliderRect;
    public RectTransform pickerRect;
    public RectTransform backButtonRect;
    public RectTransform applyButtonRect;
    public RectTransform costPanelRect;
    public RectTransform costTextRect;
    public RectTransform costIconRect;
    public RectTransform partSelectorRect;

    #region Components
    private CustomSlider tintSlider;
    private ColorPicker picker;
    private Button backButton;
    private Button applyButton;
    private Text costText;
    private Image costIcon;
    private PartSelector partSelector;
    #endregion

    private UnityAction onComplete;
    private int cost;
    private float tint;
    private Color color;

    private ShowroomCameraManager.CameraPoint cameraPoint;

    public BodyPanel Construct(UnityAction onComplete = null)
    {
        this.onComplete = onComplete;

        tint = ShowroomManager.Instance.current.data.visual.body.tint;
        color = ShowroomManager.Instance.current.data.visual.body.color.GetColor();

        tintSlider = tintSliderRect.GetComponent<CustomSlider>().Construct(tint, OnTintChanged);
        picker = pickerRect.GetComponent<ColorPicker>().Construct(OnPickerChanged, color);
        backButton = backButtonRect.GetButton(OnPressedBackButton);
        applyButton = applyButtonRect.GetButton(OnPressedApplyButton);
        costText = costTextRect.GetComponent<Text>();
        costIcon = costIconRect.GetComponent<Image>();
        partSelector = partSelectorRect.GetComponent<PartSelector>();

        costText.text = "0";
        costIcon.color = ShowroomManager.Instance.current.data.costs.body.type == 0 ? GameManager.Instance.statics.dollarColor : GameManager.Instance.statics.diamondColor;
        costIcon.sprite = ShowroomManager.Instance.current.data.costs.body.type == 0 ? GameManager.Instance.statics.dollarSprite : GameManager.Instance.statics.diamondSprite;

        if (ShowroomManager.Instance.current.paintSystem.bodyDetailPaint.material != null)
        {
            partSelector.gameObject.SetActive(true);
            partSelector.onPartChanged?.AddListener(OnPartChanged);
        }
        else
        {
            partSelector.gameObject.SetActive(false);
        }

        Appear();

        cameraPoint = ShowroomCameraManager.Instance.GetCurrentPoint();
        ShowroomCameraManager.Instance.Focus("body-paint", GameManager.Instance.statics.cameraFocusDuration, () => ShowroomCameraManager.Instance.orbitable = false);

        GameManager.Instance.analyticManager.Showed("customize-body-panel");

        return this;
    }

    private void Appear(float duration = 0.25f)
    {
        tintSliderRect.anchoredPosition += new Vector2(-1000f, 0f);
        pickerRect.anchoredPosition += new Vector2(1000f, 0f);
        backButtonRect.anchoredPosition += new Vector2(0f, -1000f);
        applyButtonRect.anchoredPosition += new Vector2(0f, -1000f);
        costPanelRect.anchoredPosition += new Vector2(0f, -1000f);
        partSelectorRect.anchoredPosition += new Vector2(0f, 1000f);

        tintSliderRect.DOAnchorPosX(tintSliderRect.anchoredPosition.x + 1000f, duration);
        pickerRect.DOAnchorPosX(pickerRect.anchoredPosition.x - 1000f, duration);
        backButtonRect.DOAnchorPosY(backButtonRect.anchoredPosition.y + 1000f, duration);
        applyButtonRect.DOAnchorPosY(applyButtonRect.anchoredPosition.y + 1000f, duration);
        costPanelRect.DOAnchorPosY(costPanelRect.anchoredPosition.y + 1000f, duration);
        partSelectorRect.DOAnchorPosY(partSelectorRect.anchoredPosition.y - 1000f, duration);
    }

    private void Disappear(float duration = 0.25f)
    {
        tintSliderRect.DOComplete();
        pickerRect.DOComplete();
        backButtonRect.DOComplete();
        applyButtonRect.DOComplete();
        costPanelRect.DOComplete();
        partSelectorRect.DOComplete();

        tintSliderRect.DOAnchorPosX(tintSliderRect.anchoredPosition.x - 1000f, duration);
        pickerRect.DOAnchorPosX(pickerRect.anchoredPosition.x + 1000f, duration);
        backButtonRect.DOAnchorPosY(backButtonRect.anchoredPosition.y - 1000f, duration);
        applyButtonRect.DOAnchorPosY(applyButtonRect.anchoredPosition.y - 1000f, duration);
        costPanelRect.DOAnchorPosY(costPanelRect.anchoredPosition.y - 1000f, duration);
        partSelectorRect.DOAnchorPosY(partSelectorRect.anchoredPosition.y + 1000f, duration).OnComplete(() => Destroy(gameObject));
    }

    private void OnPressedBackButton()
    {
        onComplete?.Invoke();
        ShowroomManager.Instance.current.paintSystem.ReturnOriginal();
        ShowroomCameraManager.Instance.orbitable = true;
        ShowroomCameraManager.Instance.Focus(cameraPoint, GameManager.Instance.statics.cameraFocusDuration);
        cameraPoint = null;
        Disappear();
        GameManager.Instance.audioManager.Play("tap");
        GameManager.Instance.analyticManager.Clicked("customize-body-panel", "back-button");
    }

    private void OnPressedApplyButton()
    {
        GameManager
        .Instance
        .currencyManager
        .Pay(ShowroomManager.Instance.current.data.costs.body, b =>
        {
            if (b)
            {
                ShowroomManager.Instance.current.paintSystem.Save();
                GameManager.Instance.dataManager.SaveUser();
                OnPressedBackButton();
                GameManager.Instance.analyticManager.Clicked("customize-body-panel", "apply-button");
            }
        });
        GameManager.Instance.audioManager.Play("tap");
    }

    private void OnPickerChanged(Color color)
    {
        cost = ShowroomManager.Instance.current.data.costs.body.amount;
        costText.text = cost.ToCurrencyString();
        this.color = color;
        SetCarColor();
    }

    private void OnTintChanged(float val)
    {
        cost = ShowroomManager.Instance.current.data.costs.body.amount;
        costText.text = cost.ToCurrencyString();
        this.tint = val;
        SetCarColor();
    }

    private void SetCarColor()
    {
        if (partSelector.selection == 0) ShowroomManager.Instance.current.paintSystem.bodyPaint.SetColor(color, tint);
        else ShowroomManager.Instance.current.paintSystem.bodyDetailPaint.SetColor(color, tint);
    }

    private void OnDestroy()
    {
        backButton?.onClick.RemoveAllListeners();
        applyButton?.onClick.RemoveAllListeners();
        onComplete = null;
    }

    private void OnPartChanged(int index)
    {
        color = index == 0 ? ShowroomManager.Instance.current.paintSystem.bodyPaint.color : ShowroomManager.Instance.current.paintSystem.bodyDetailPaint.color;
        tint = index == 0 ? ShowroomManager.Instance.current.paintSystem.bodyPaint.tint : ShowroomManager.Instance.current.paintSystem.bodyDetailPaint.tint;

        tintSlider.SetValue(tint);
    }

    public static EKCanvas Get(UnityAction onComplete = null)
    {
        GameObject prefab = Resources.Load<GameObject>("_ui/_showroom/_customize-panel/body-panel");
        BodyPanel bp = Instantiate(prefab).GetComponent<BodyPanel>().Construct(onComplete);
        return bp.gameObject.AddComponent<EKCanvas>().Construct(null);
    }
}