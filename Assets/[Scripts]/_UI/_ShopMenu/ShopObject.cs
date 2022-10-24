using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using Utils.Strings;
using Utils.UI;

public class ShopObject : MonoBehaviour
{
    public RectTransform rect { get { return GetComponent<RectTransform>(); } }
    public RectTransform titleTextRect;
    public RectTransform iconRect;
    public RectTransform priceRect;
    public RectTransform priceIconRect;
    public RectTransform priceTextRect;
    public RectTransform descTextRect;
    public RectTransform buttonTextRect;

    private Text titleText;
    private Image icon;
    private Image priceIcon;
    private Text priceText;
    private Text descText;
    private Text buttonText;
    private Button button;

    // private UnityAction<IAPObject> onClicked;
    private IAPObject iap;

    public ShopObject Construct(IAPObject iap)
    {
        titleText = titleTextRect.GetComponent<Text>();
        icon = iconRect.GetComponent<Image>();
        priceIcon = priceIconRect.GetComponent<Image>();
        priceText = priceTextRect.GetComponent<Text>();
        descText = descTextRect.GetComponent<Text>();
        buttonText = buttonTextRect.GetComponent<Text>();

        // this.onClicked = onClicked;
        this.iap = iap;

        Set();

        return this;
    }

    private void Set()
    {
        titleText.text = iap.title;
        icon.sprite = Resources.Load<Sprite>("_sprites/_iap/" + iap.icon);

        if (iap.price.type == -1)
        {
            priceRect.gameObject.SetActive(false);
            descText.gameObject.SetActive(true);

            descText.text = iap.description;
        }
        else
        {
            priceRect.gameObject.SetActive(true);
            descText.gameObject.SetActive(false);

            priceIcon.sprite = iap.price.type == 0 ? GameManager.Instance.statics.dollarSprite : GameManager.Instance.statics.diamondSprite;
            priceIcon.color = iap.price.type == 0 ? GameManager.Instance.statics.dollarColor : GameManager.Instance.statics.diamondColor;

            priceText.text = iap.price.amount.ToCurrencyString();
        }

        if (!(iap.id.Contains("no_ads") && GameManager.Instance.dataManager.user.boughtedNoAds))
        {
            button = rect.GetButton(OnPressedButton);
            buttonText.text = GameManager.Instance.sdkManager.iapManager.GetPrice(iap.id);
        }
        else
        {
            buttonText.text = "YOU GOT IT";
        }
    }

    private void OnPressedButton()
    {
        GameManager.Instance.sdkManager.iapManager.Buy(iap.id, b =>
        {
            if (b) OnBoughted();
        });
        GameManager.Instance.analyticManager.Clicked("shop-panel", "shop-object-[" + iap.id + "]");
    }

    private void OnBoughted()
    {
        if (iap.id.Contains("no_ads"))
        {
            GameManager.Instance.sdkManager.OnBoughtedNoAds();
            buttonText.text = "YOU GOT IT";
            button?.onClick.RemoveAllListeners();
            Destroy(button);
            GameManager.Instance.analyticManager.IAP(iap.id.Split('.').ToList().Last());
            return;
        }

        if (iap.price.type >= 0) GameManager.Instance.currencyManager.Earn(iap.price);
    }

    private void OnDestroy()
    {
        button?.onClick.RemoveAllListeners();
    }
}