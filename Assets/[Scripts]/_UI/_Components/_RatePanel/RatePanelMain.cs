using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using Utils.UI;
using DG.Tweening;

public class RatePanelMain : MonoBehaviour
{
    public RectTransform rect;
    public RectTransform headTextRect;
    public RectTransform bodyTextRect;
    public RectTransform yesButtonRect;
    public RectTransform noButtonRect;
    public RectTransform laterButtonRect;

    private Text headText;
    private Text bodyText;
    private Button yesButton;
    private Button noButton;
    private Button laterButton;

    private UnityAction<int> onComplete;

    public RatePanelMain Construct(UnityAction<int> onComplete, RateMain data)
    {
        this.onComplete = onComplete;

        headText = headTextRect.GetComponent<Text>();
        bodyText = bodyTextRect.GetComponent<Text>();

        yesButton = yesButtonRect.GetButton(OnPressedYesButton);
        noButton = noButtonRect.GetButton(OnPressedNoButton);
        laterButton = laterButtonRect.GetButton(OnPressedLaterButton);

        headText.text = data.titleText;
        bodyText.text = data.bodyText;
        yesButtonRect.GetComponentInChildren<Text>().text = data.yesButtonText;
        noButtonRect.GetComponentInChildren<Text>().text = data.noButtonText;
        laterButtonRect.GetComponentInChildren<Text>().text = data.laterButtonText;

        GameManager.Instance.analyticManager.Showed("rate-main-panel");

        Appear();

        return this;
    }

    private void Appear(float duration = 0.25f)
    {
        rect.localScale = new Vector3(0f, 1f, 1f);

        rect.DOScaleX(1f, duration);
    }

    private void Disappear(float duration = 0.25f)
    {
        rect.DOScaleX(0f, duration).OnComplete(() => Destroy(gameObject));
    }

    private void OnPressedYesButton()
    {
        onComplete?.Invoke(0);
        Disappear();

        GameManager.Instance.analyticManager.Clicked("rate-main-panel", "yes-button");
    }

    private void OnPressedNoButton()
    {
        onComplete?.Invoke(1);
        Disappear();

        GameManager.Instance.analyticManager.Clicked("rate-main-panel", "no-button");
    }

    private void OnPressedLaterButton()
    {
        onComplete?.Invoke(2);
        Disappear();

        GameManager.Instance.analyticManager.Clicked("rate-main-panel", "later-button");
    }

    private void OnDestroy()
    {
        yesButton?.onClick.RemoveAllListeners();
        noButton?.onClick.RemoveAllListeners();
        laterButton?.onClick.RemoveAllListeners();
        onComplete = null;
    }

    public static void Get(UnityAction<int> onComplete, RectTransform context, RateMain data)
    {
        GameObject prefab = Resources.Load<GameObject>("_ui/_rate-panel/main-panel");
        Instantiate(prefab, context).GetComponent<RatePanelMain>().Construct(onComplete, data);
    }
}