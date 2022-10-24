using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Utils.Strings;

public class CurrencyEarnPanel : MonoBehaviour
{
    public RectTransform rect;
    public RectTransform iconRect;
    public RectTransform textRect;

    private Image icon;
    private Text text;
    private Price price;
    private Tween tween;

    public CurrencyEarnPanel Construct(Price price)
    {
        this.price = price;
        icon = iconRect.GetComponent<Image>();
        text = textRect.GetComponent<Text>();

        icon.sprite = price.type == 0 ? GameManager.Instance.statics.dollarSprite : GameManager.Instance.statics.diamondSprite;
        icon.color = price.type == 0 ? GameManager.Instance.statics.dollarColor : GameManager.Instance.statics.diamondColor;

        Appear();

        return this;
    }

    private void Appear(float duration = 0.5f)
    {
        text.text = "0";
        rect.anchoredPosition += new Vector2(0f, 1000f);

        textRect.DOScale(1f, duration);
        rect
        .DOAnchorPosY(rect.anchoredPosition.y - 1000f, duration)
        .SetEase(Ease.OutBack)
        .OnComplete(() =>
        {
            float f = 0f;
            tween = DOTween
            .To(() => f, x => f = x, price.amount, duration * 2f)
            .OnUpdate(() =>
            {
                text.text = ((int)f).ToCurrencyString();
            })
            .OnComplete(() =>
            {
                text.text = ((int)price.amount).ToCurrencyString();
                Disappear();
            });
        });
    }

    private void Disappear(float duration = 0.5f)
    {
        rect
        .DOAnchorPosY(rect.anchoredPosition.y + 1000f, duration)
        .SetDelay(duration * 3f)
        .SetEase(Ease.InBack)
        .OnComplete(() => Destroy(gameObject));
    }

    private void OnDestroy()
    {
        tween?.Kill();
        rect.DOKill();
    }

    public static EKCanvas Get(Price price)
    {
        CurrencyEarnPanel cep = Instantiate(Resources.Load<GameObject>("_ui/_game/currency-earn-canvas")).GetComponent<CurrencyEarnPanel>().Construct(price);
        return cep.gameObject.AddComponent<EKCanvas>().Construct(null);
    }
}
