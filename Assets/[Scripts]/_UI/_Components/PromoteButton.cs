using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using Utils.UI;
using Utils.Strings;

public class PromoteButton : MonoBehaviour
{
    public RectTransform icon1Rect;
    public RectTransform icon2Rect;
    public RectTransform textRect;

    public bool isNoAds;

    private Image icon1;
    private Image icon2;
    private Text text;
    private Button button;

    private UnityAction<Price> onPressed;
    private Price price;

    public void Construct(bool isNoAds, Price price, UnityAction<Price> onPressed)
    {
        this.isNoAds = isNoAds;
        this.price = price;
        this.onPressed = onPressed;

        text = textRect.GetComponent<Text>();
        icon1 = icon1Rect.GetComponent<Image>();

        button = GetComponent<RectTransform>().GetButton(OnPressed);

        if (this.isNoAds)
        {
            icon1.sprite = Resources.Load<Sprite>("_sprites/_iap/no_ads-white");
            icon2?.gameObject.SetActive(false);
            text.text = "NO ADS <size=40>" + GameManager.Instance.sdkManager.iapManager.GetPrice(GameManager.Instance.dataManager.iap.objects.Find(c => c.id.Contains("no_ads")).id) + "</size>";
            return;
        }

        icon1.sprite = price.type == 0 ? GameManager.Instance.statics.dollarSprite : GameManager.Instance.statics.diamondSprite;
        icon1.color = Color.white;

        text.fontSize = 20;
        text.text = "GET <size=" + (text.fontSize * 2) + ">" + price.amount.ToCurrencyString() + "</size> " + (price.type == 0 ? "DOLLAR" : "DIAMOND");

        icon2 = icon2Rect.GetComponent<Image>();
        icon2.sprite = Resources.Load<Sprite>("_sprites/video");
        icon2.color = Color.white;
    }

    private void OnDestroy()
    {
        button?.onClick.RemoveAllListeners();
    }

    private void OnPressed()
    {
        onPressed?.Invoke(price);
    }
}