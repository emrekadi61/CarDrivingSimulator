using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using DG.Tweening;
using Utils.UI;

public class ShopCodePanel : MonoBehaviour
{
    public RectTransform backgroundRect;
    public RectTransform backButtonPanelRect;
    public RectTransform panelRect;
    public RectTransform submitButtonRect;
    public RectTransform cancelButtonRect;
    public RectTransform inputFieldRect;
    public RectTransform checkingPanelRect;
    public RectTransform checkingPanelTextRect;

    private Image background;
    private Button submitButton;
    private Button cancelButton;
    private Button backButtonPanel;
    private InputField inputField;
    private Text checkingPanelText;

    private UnityAction<bool> onComplete;
    private FirebaseCouponChecker couponChecker;

    public void Construct(UnityAction<bool> onComplete)
    {
        this.onComplete = onComplete;

        background = backgroundRect.GetComponent<Image>();
        inputField = inputFieldRect.GetComponent<InputField>();
        submitButton = submitButtonRect.GetButton(OnPressedSubmitButton);
        cancelButton = cancelButtonRect.GetButton(OnPressedCancelButton);
        backButtonPanel = backButtonPanelRect.GetButton(OnPressedCancelButton);

        checkingPanelText = checkingPanelTextRect.GetComponent<Text>();
        checkingPanelRect.gameObject.SetActive(false);

        inputField.onValueChanged.AddListener(OnTextChanged);
        couponChecker = gameObject.AddComponent<FirebaseCouponChecker>();

        Appear();
    }

    private void Appear(float duration = 0.25f)
    {
        Color c = background.color; c.a = 0f; background.color = c;
        panelRect.anchoredPosition += new Vector2(0f, -1500f);

        background.DOFade(0.85f, duration);
        panelRect.DOAnchorPosY(panelRect.anchoredPosition.y + 1500f, duration);
    }

    private void Disappear(float duration = 0.25f)
    {
        background.DOFade(0f, duration);
        panelRect.DOAnchorPosY(panelRect.anchoredPosition.y - 1500f, duration).OnComplete(() => Destroy(gameObject));
    }

    private void OnPressedSubmitButton()
    {
        CheckCode(inputField.text);
    }

    private void OnPressedCancelButton()
    {
        onComplete?.Invoke(false);
        Disappear();
    }

    private void CheckCode(string code)
    {
        if (string.IsNullOrEmpty(code)) return;

        checkingPanelText.text = "CHECKING..";
        checkingPanelRect.gameObject.SetActive(true);
        couponChecker.CheckCode(code.ToLower(), p =>
        {
            if (p != null)
            {
                checkingPanelText.text = "<color='#3CCF4E'>COUPON IS PROCESSING..</color>";
                GameManager.Instance.delayManager.Set(1f, () =>
                {
                    if (p.type >= 0) GameManager.Instance.currencyManager.Earn(p);
                    else if (p.type == -1) GameManager.Instance.sdkManager.OnBoughtedNoAds();
                    
                    onComplete?.Invoke(true);
                    Disappear();
                });
            }
            else
            {
                checkingPanelText.text = "<color='red'>COUPON IS INVALID</color>";
                GameManager.Instance.delayManager.Set(1f, () =>
                {
                    onComplete?.Invoke(false);
                    Disappear();
                });

            }
        });
    }

    private void OnTextChanged(string text)
    {
        inputField.text = inputField.text.ToUpper();
    }

    private void OnDestroy()
    {
        submitButton?.onClick.RemoveAllListeners();
        cancelButton?.onClick.RemoveAllListeners();
        backButtonPanel?.onClick.RemoveAllListeners();
    }

    public static void Get(UnityAction<bool> onComplete, Transform context)
    {
        Instantiate(Resources.Load<GameObject>("_ui/_shop-panel/code-panel"), context).GetComponent<ShopCodePanel>().Construct(onComplete);
    }
}