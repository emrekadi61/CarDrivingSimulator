using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using DG.Tweening;
using Utils.UI;
using Utils.Strings;

public class WindowPanel : MonoBehaviour
{
    public RectTransform tintSliderRect;
    public RectTransform pickerRect;
    public RectTransform backButtonRect;
    public RectTransform applyButtonRect;
    public RectTransform costPanelRect;
    public RectTransform costTextRect;
    public RectTransform costIconRect;

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
    private float tint;
    private Color color;

    private ShowroomCameraManager.CameraPoint cameraPoint;

    public WindowPanel Construct(UnityAction onComplete = null)
    {
        this.onComplete = onComplete;

        tint = ShowroomManager.Instance.current.data.visual.window.tint;
        color = ShowroomManager.Instance.current.data.visual.window.color.GetColor();

        tintSlider = tintSliderRect.GetComponent<CustomSlider>().Construct(tint, OnTintChanged);
        picker = pickerRect.GetComponent<ColorPicker>().Construct(OnPickerChanged, color);
        backButton = backButtonRect.GetButton(OnPressedBackButton);
        applyButton = applyButtonRect.GetButton(OnPressedApplyButton);
        costText = costTextRect.GetComponent<Text>();
        costIcon = costIconRect.GetComponent<Image>();

        costText.text = "0";
        costIcon.color = ShowroomManager.Instance.current.data.costs.window.type == 0 ? GameManager.Instance.statics.dollarColor : GameManager.Instance.statics.diamondColor;
        costIcon.sprite = ShowroomManager.Instance.current.data.costs.window.type == 0 ? GameManager.Instance.statics.dollarSprite : GameManager.Instance.statics.diamondSprite;

        Appear();

        cameraPoint = ShowroomCameraManager.Instance.GetCurrentPoint();
        ShowroomCameraManager.Instance.Focus("windows", GameManager.Instance.statics.cameraFocusDuration, () => ShowroomCameraManager.Instance.orbitable = false);
        GameManager.Instance.analyticManager.Showed("customize-window-panel");
        return this;
    }

    private void Appear(float duration = 0.25f)
    {
        tintSliderRect.anchoredPosition += new Vector2(-1000f, 0f);
        pickerRect.anchoredPosition += new Vector2(1000f, 0f);
        backButtonRect.anchoredPosition += new Vector2(0f, -1000f);
        applyButtonRect.anchoredPosition += new Vector2(0f, -1000f);
        costPanelRect.anchoredPosition += new Vector2(0f, -1000f);

        tintSliderRect.DOAnchorPosX(tintSliderRect.anchoredPosition.x + 1000f, duration);
        pickerRect.DOAnchorPosX(pickerRect.anchoredPosition.x - 1000f, duration);
        backButtonRect.DOAnchorPosY(backButtonRect.anchoredPosition.y + 1000f, duration);
        applyButtonRect.DOAnchorPosY(applyButtonRect.anchoredPosition.y + 1000f, duration);
        costPanelRect.DOAnchorPosY(costPanelRect.anchoredPosition.y + 1000f, duration);
    }

    private void Disappear(float duration = 0.25f)
    {
        tintSliderRect.DOComplete();
        pickerRect.DOComplete();
        backButtonRect.DOComplete();
        applyButtonRect.DOComplete();
        costPanelRect.DOComplete();

        tintSliderRect.DOAnchorPosX(tintSliderRect.anchoredPosition.x - 1000f, duration);
        pickerRect.DOAnchorPosX(pickerRect.anchoredPosition.x + 1000f, duration);
        backButtonRect.DOAnchorPosY(backButtonRect.anchoredPosition.y - 1000f, duration);
        applyButtonRect.DOAnchorPosY(applyButtonRect.anchoredPosition.y - 1000f, duration);
        costPanelRect.DOAnchorPosY(costPanelRect.anchoredPosition.y - 1000f, duration).OnComplete(() => Destroy(gameObject));
    }

    private void OnPressedBackButton()
    {
        onComplete?.Invoke();
        ShowroomManager.Instance.current.paintSystem.ReturnOriginal();
        ShowroomCameraManager.Instance.orbitable = true;
        ShowroomCameraManager.Instance.Focus(cameraPoint, GameManager.Instance.statics.cameraFocusDuration);
        Disappear();
        GameManager.Instance.audioManager.Play("tap");
        GameManager.Instance.analyticManager.Clicked("customize-window-panel", "back-button");
    }

    private void OnPressedApplyButton()
    {
        GameManager
        .Instance
        .currencyManager
        .Pay(ShowroomManager.Instance.current.data.costs.window, b =>
        {
            if (b)
            {
                ShowroomManager.Instance.current.paintSystem.Save();
                GameManager.Instance.dataManager.SaveUser();
                OnPressedBackButton();
                GameManager.Instance.analyticManager.Clicked("customize-window-panel", "apply-button");
            }
        });
        GameManager.Instance.audioManager.Play("tap");
    }

    private void OnPickerChanged(Color color)
    {
        cost = ShowroomManager.Instance.current.data.costs.window.amount;
        costText.text = cost.ToCurrencyString();

        this.color = color;
        OnColorChanged();
    }

    private void OnTintChanged(float val)
    {
        cost = ShowroomManager.Instance.current.data.costs.window.amount;
        costText.text = cost.ToCurrencyString();

        this.tint = val;
        OnColorChanged();
    }

    private void OnColorChanged()
    {
        ShowroomManager.Instance.current.paintSystem.windowPaint.SetColor(true, color, tint);
    }

    private void OnDestroy()
    {
        backButton?.onClick.RemoveAllListeners();
        applyButton?.onClick.RemoveAllListeners();
        onComplete = null;
    }

    public static EKCanvas Get(UnityAction onComplete = null)
    {
        GameObject prefab = Resources.Load<GameObject>("_ui/_showroom/_customize-panel/window-panel");
        WindowPanel bp = Instantiate(prefab).GetComponent<WindowPanel>().Construct(onComplete);
        return bp.gameObject.AddComponent<EKCanvas>().Construct(null);
    }
}