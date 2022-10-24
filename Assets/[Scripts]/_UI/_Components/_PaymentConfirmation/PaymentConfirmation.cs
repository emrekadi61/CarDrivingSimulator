using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using Utils.UI;
using Utils.Strings;
using DG.Tweening;

public class PaymentConfirmation : MonoBehaviour
{
    public RectTransform backgroundRect;
    public RectTransform panel;
    public RectTransform iconRect;
    public RectTransform textRect;
    public RectTransform yesButtonRect;
    public RectTransform noButtonRect;

    private Image background;
    private Image icon;
    private Text text;
    private Button yesButton;
    private Button noButton;

    private UnityAction<bool> result;
    private Price price;

    public void Construct(Price price, UnityAction<bool> result)
    {
        this.price = price;
        this.result = result;

        background = backgroundRect.GetComponent<Image>();
        icon = iconRect.GetComponent<Image>();
        text = textRect.GetComponent<Text>();

        icon.sprite = price.type == 0 ? GameManager.Instance.statics.dollarSprite : GameManager.Instance.statics.diamondSprite;
        icon.color = price.type == 0 ? GameManager.Instance.statics.dollarColor : GameManager.Instance.statics.diamondColor;

        text.text = this.price.amount.ToCurrencyString();

        yesButton = yesButtonRect.GetButton(OnPressedYes);
        noButton = noButtonRect.GetButton(OnPressedNo);

        GameManager.Instance.analyticManager.Showed("payment-confirmation-panel");

        Appear();
    }

    private void Appear(float duration = 0.25f)
    {
        Color c = background.color; c.a = 0f; background.color = c;
        panel.localScale = new Vector3(1f, 0f, 1f);

        background.DOFade(0.6f, duration);
        panel.DOScaleY(1f, duration);
    }

    private void Disappear(float duration = 0.25f)
    {
        background.DOFade(0f, duration);
        panel
        .DOScaleY(0f, duration)
        .OnComplete(() =>
        {
            Destroy(gameObject);
        });
    }

    private void OnPressedYes()
    {
        if (!GameManager.Instance.currencyManager.HaveEnoughCurrency(price.type == 0 ? Currency.Dollar : Currency.Diamond, price.amount))
        {
            NoEnoughMoney.Get(b =>
            {
                if (!b)
                {
                    Disappear();
                    result?.Invoke(false);
                    GameManager.Instance.analyticManager.Clicked("payment-confirmation-panel", "yes-button");
                }
            }, transform);

            return;
        }
        Disappear();
        result?.Invoke(true);
    }

    private void OnPressedNo()
    {
        Disappear();
        result?.Invoke(false);

        GameManager.Instance.analyticManager.Clicked("payment-confirmation-panel", "no-button");
    }

    public static EKCanvas Get(Price price, UnityAction<bool> result)
    {
        PaymentConfirmation pc = Instantiate(Resources.Load<GameObject>("_ui/_payment-confirmation/main")).GetComponent<PaymentConfirmation>();
        pc.Construct(price, result);
        return pc.gameObject.AddComponent<EKCanvas>().Construct(null);
    }
}