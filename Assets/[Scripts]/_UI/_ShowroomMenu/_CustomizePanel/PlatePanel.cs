using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using DG.Tweening;
using Utils.UI;

public class PlatePanel : MonoBehaviour
{
    public RectTransform pickerRect;
    public RectTransform backButtonRect;
    public RectTransform applyButtonRect;
    public RectTransform plateRect;

    #region Components
    private CustomSlider tintSlider;
    private ColorPicker picker;
    private Button backButton;
    private Button applyButton;
    private InputField plate;
    private Image plateImage;
    #endregion

    private UnityAction onComplete;
    private ShowroomCameraManager.CameraPoint cameraPoint;

    public PlatePanel Construct(UnityAction onComplete = null)
    {
        this.onComplete = onComplete;

        picker = pickerRect.GetComponent<ColorPicker>().Construct(OnPickerChanged, ShowroomManager.Instance.current.data.visual.body.color.GetColor());
        backButton = backButtonRect.GetButton(OnPressedBackButton);
        applyButton = applyButtonRect.GetButton(OnPressedApplyButton);
        plate = plateRect.GetComponent<InputField>();
        plateImage = plateRect.GetComponent<Image>();
        plate.textComponent.color = GameManager.Instance.dataManager.user.gameData.plate.textColor.GetColor();
        plateImage.color = GameManager.Instance.dataManager.user.gameData.plate.baseColor.GetColor();
        plate.text = GameManager.Instance.dataManager.user.gameData.plate.text;
        plate.onValueChanged.AddListener(OnPlateChanged);

        Appear();

        cameraPoint = ShowroomCameraManager.Instance.GetCurrentPoint();
        ShowroomCameraManager.Instance.Focus("plate", GameManager.Instance.statics.cameraFocusDuration, () => ShowroomCameraManager.Instance.orbitable = false);

        GameManager.Instance.analyticManager.Showed("customize-plate-panel");
        return this;
    }

    private void Appear(float duration = 0.25f)
    {
        pickerRect.anchoredPosition += new Vector2(1000f, 0f);
        backButtonRect.anchoredPosition += new Vector2(0f, -1000f);
        applyButtonRect.anchoredPosition += new Vector2(0f, -1000f);
        plateRect.anchoredPosition += new Vector2(0f, -1000f);

        pickerRect.DOAnchorPosX(pickerRect.anchoredPosition.x - 1000f, duration);
        backButtonRect.DOAnchorPosY(backButtonRect.anchoredPosition.y + 1000f, duration);
        applyButtonRect.DOAnchorPosY(applyButtonRect.anchoredPosition.y + 1000f, duration);
        plateRect.DOAnchorPosY(plateRect.anchoredPosition.y + 1000f, duration);
    }

    private void Disappear(float duration = 0.25f)
    {
        pickerRect.DOComplete();
        backButtonRect.DOComplete();
        applyButtonRect.DOComplete();
        plateRect.DOComplete();

        pickerRect.DOAnchorPosX(pickerRect.anchoredPosition.x + 1000f, duration);
        backButtonRect.DOAnchorPosY(backButtonRect.anchoredPosition.y - 1000f, duration);
        applyButtonRect.DOAnchorPosY(applyButtonRect.anchoredPosition.y - 1000f, duration);
        plateRect.DOAnchorPosY(plateRect.anchoredPosition.y - 1000f, duration);
    }

    private void OnPressedBackButton()
    {
        onComplete?.Invoke();
        ShowroomCameraManager.Instance.orbitable = true;
        ShowroomCameraManager.Instance.Focus(cameraPoint, GameManager.Instance.statics.cameraFocusDuration);
        Disappear();
        GameManager.Instance.audioManager.Play("tap");

        GameManager.Instance.analyticManager.Clicked("customize-plate-panel", "back-button");
    }

    private void OnPressedApplyButton()
    {
        GameManager.Instance.dataManager.user.gameData.plate.text = plate.text;
        GameManager.Instance.dataManager.user.gameData.plate.baseColor = plateImage.color.GetCode();
        GameManager.Instance.dataManager.user.gameData.plate.textColor = plate.textComponent.color.GetCode();
        GameManager.Instance.dataManager.SaveUser();
        ShowroomManager.Instance.current.onPlateChanged.Invoke();
        OnPressedBackButton();
        GameManager.Instance.audioManager.Play("tap");
        GameManager.Instance.analyticManager.Clicked("customize-plate-panel", "apply-button");
    }

    private void OnPickerChanged(Color color)
    {
        plateImage.color = color;
        plate.textComponent.color = Color.Lerp(Color.black, Color.white, picker.blackSpectrum.value);
    }

    private void OnPlateChanged(string val)
    {
        if (plate.text.Length > 10) plate.text = val.Substring(0, 10);
    }

    private void OnDestroy()
    {
        backButton?.onClick.RemoveAllListeners();
        applyButton?.onClick.RemoveAllListeners();
        onComplete = null;
        plate.onValueChanged.RemoveAllListeners();
    }

    public static EKCanvas Get(UnityAction onComplete = null)
    {
        GameObject prefab = Resources.Load<GameObject>("_ui/_showroom/_customize-panel/plate-panel");
        PlatePanel pp = Instantiate(prefab).GetComponent<PlatePanel>().Construct(onComplete);
        return pp.gameObject.AddComponent<EKCanvas>().Construct(null);
    }
}