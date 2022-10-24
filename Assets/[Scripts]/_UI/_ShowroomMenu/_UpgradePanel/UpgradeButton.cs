using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using DG.Tweening;
using Utils.UI;
using Utils.Strings;

public class UpgradeButton : MonoBehaviour
{
    private RectTransform sliderFillRect;
    private RectTransform priceIconRect;
    private RectTransform priceTextRect;
    private RectTransform buttonRect;
    private RectTransform buttonTextRect;
    private RectTransform videoAdRect;

    private Image sliderFill;
    private Image priceIcon;
    private Text priceText;
    private Button button;

    private int level;
    private int maxLevel;

    private UnityAction<Price, bool> onPressed;
    private Price price;
    private bool videoAd;
    private int videoCount = 0;
    private int videoLimit = 1;

    public UpgradeButton Construct(int level, int maxLevel, Price price, UnityAction<Price, bool> onPressed, float delay = 0.25f)
    {
        this.level = level;
        this.maxLevel = maxLevel;
        this.onPressed = onPressed;
        this.price = price;

        sliderFillRect = transform.Find("slider/fill").GetComponent<RectTransform>();
        priceIconRect = transform.Find("price/icon").GetComponent<RectTransform>();
        priceTextRect = transform.Find("price/text").GetComponent<RectTransform>();
        buttonRect = transform.Find("button").GetComponent<RectTransform>();
        buttonTextRect = transform.Find("button/text").GetComponent<RectTransform>();
        videoAdRect = transform.Find("button/video-ad").GetComponent<RectTransform>();

        sliderFill = sliderFillRect.GetComponent<Image>();
        priceIcon = priceIconRect.GetComponent<Image>();
        priceText = priceTextRect.GetComponent<Text>();
        button = buttonRect.GetButton(OnPressedButton);

        sliderFill.fillAmount = 0f;
        sliderFill.DOFillAmount((float)level / (float)maxLevel, 0.5f).SetDelay(delay);

        priceText.text = this.price.amount.ToCurrencyString();

        priceIcon.color = this.price.type == 0 ? GameManager.Instance.statics.dollarColor : GameManager.Instance.statics.diamondColor;
        priceIcon.sprite = this.price.type == 0 ? GameManager.Instance.statics.dollarSprite : GameManager.Instance.statics.diamondSprite;

        if (level >= maxLevel)
        {
            button.interactable = false;
            buttonTextRect.GetComponent<Text>().text = "MAX LEVEL";
            videoAdRect.gameObject.SetActive(false);
            buttonTextRect.gameObject.SetActive(true);
            button.GetComponent<Image>().color = GameManager.Instance.statics.dollarColor;
        }
        else
        {
            bool haveMoney = GameManager.Instance.currencyManager.HaveEnoughCurrency(this.price.type == 0 ? Currency.Dollar : Currency.Diamond, this.price.amount);
            if (!haveMoney && videoCount < videoLimit)
            {
                videoAd = !haveMoney;
                videoAdRect.gameObject.SetActive(true);
                buttonTextRect.gameObject.SetActive(false);
            }
            else
            {
                videoAdRect.gameObject.SetActive(false);
                buttonTextRect.gameObject.SetActive(true);
                button.interactable = haveMoney;
            }
            button.GetComponent<Image>().color = (!haveMoney && videoCount < videoLimit) ? GameManager.Instance.statics.diamondColor : GameManager.Instance.statics.dollarColor;
        }

        return this;
    }

    public void Set(int level, int maxLevel, Price price)
    {
        this.price = price;
        sliderFill.DOComplete();
        priceText.DOComplete();

        sliderFill.DOFillAmount((float)level / (float)maxLevel, 0.25f);
        priceText.DOText(this.price.amount.ToCurrencyString(), 0.25f);

        priceIcon.color = this.price.type == 0 ? GameManager.Instance.statics.dollarColor : GameManager.Instance.statics.diamondColor;
        priceIcon.sprite = this.price.type == 0 ? GameManager.Instance.statics.dollarSprite : GameManager.Instance.statics.diamondSprite;

        if (level >= maxLevel)
        {
            button.interactable = false;
            buttonTextRect.GetComponent<Text>().text = "MAX LEVEL";
            videoAdRect.gameObject.SetActive(false);
            buttonTextRect.gameObject.SetActive(true);
            button.GetComponent<Image>().color = GameManager.Instance.statics.dollarColor;
        }
        else
        {
            bool haveMoney = GameManager.Instance.currencyManager.HaveEnoughCurrency(this.price.type == 0 ? Currency.Dollar : Currency.Diamond, this.price.amount);
            if (!haveMoney && videoCount < videoLimit)
            {
                videoAd = !haveMoney;
                videoAdRect.gameObject.SetActive(true);
                buttonTextRect.gameObject.SetActive(false);
            }
            else
            {
                videoAdRect.gameObject.SetActive(false);
                buttonTextRect.gameObject.SetActive(true);
                button.interactable = haveMoney;
            }

            button.GetComponent<Image>().color = (!haveMoney && videoCount < videoLimit) ? GameManager.Instance.statics.diamondColor : GameManager.Instance.statics.dollarColor;
        }
    }

    private void OnPressedButton()
    {
        if (videoAd) videoCount++;
        onPressed?.Invoke(price, videoAd);
        GameManager.Instance.audioManager.Play("tap");
    }

    private void OnDestroy()
    {
        button?.onClick.RemoveAllListeners();
    }
}