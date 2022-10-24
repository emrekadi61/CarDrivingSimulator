using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using Utils.UI;
using DG.Tweening;

public class NoEnoughMoney : MonoBehaviour
{
    public RectTransform panel;
    public RectTransform textRect;
    public RectTransform yesButtonRect;
    public RectTransform noButtonRect;

    private Text text;
    private Button yesButton;
    private Button noButton;

    private UnityAction<bool> result;

    public void Construct(UnityAction<bool> result)
    {
        this.result = result;

        text = textRect.GetComponent<Text>();

        yesButton = yesButtonRect.GetButton(OnPressedYes);
        noButton = noButtonRect.GetButton(OnPressedNo);

        Appear();

        GameManager.Instance.analyticManager.Showed("payment-confirmation-not-enough-money-panel");
    }

    private void Appear(float duration = 0.25f)
    {
        panel.localScale = new Vector3(1f, 0f, 1f);
        panel.DOScaleY(1f, duration);
    }

    private void Disappear(float duration = 0.25f)
    {
        panel
        .DOScaleY(0f, duration)
        .OnComplete(() =>
        {
            Destroy(gameObject);
        });
    }

    private void OnPressedYes()
    {
        Disappear();
        UIManager.Instance.Push(ShopMenu.Get());
        result?.Invoke(true);

        GameManager.Instance.analyticManager.Clicked("payment-confirmation-not-enough-money-panel", "yes-button");
    }

    private void OnPressedNo()
    {
        Disappear();
        result?.Invoke(false);

        GameManager.Instance.analyticManager.Clicked("payment-confirmation-not-enough-money-panel", "no-button");
    }

    public static void Get(UnityAction<bool> result, Transform parent)
    {
        GameObject prefab = Resources.Load<GameObject>("_ui/_payment-confirmation/not-enough-money");
        Instantiate(prefab, parent).GetComponent<NoEnoughMoney>().Construct(result);
    }
}