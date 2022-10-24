using UnityEngine;
using UnityEngine.UI;
using Utils.UI;
using Utils.Strings;
using DG.Tweening;

public class CurrencyPanel : MonoBehaviour
{
    public RectTransform dollarTextRect;
    public RectTransform diamondTextRect;
    public RectTransform addMoneyButtonRect;

    private Text dollarText;
    private Text diamondText;
    private Button addMoneyButton;

    private int dollar;
    private int diamond;

    private void Awake()
    {
        dollarText = dollarTextRect.GetComponent<Text>();
        diamondText = diamondTextRect.GetComponent<Text>();
        if (addMoneyButtonRect) addMoneyButton = addMoneyButtonRect.GetButton(OnPressedAddMoneyButton);
    }

    private void Start()
    {
        dollar = GameManager.Instance.dataManager.user.gameData.dollar;
        diamond = GameManager.Instance.dataManager.user.gameData.diamond;

        dollarText.text = dollar.ToCurrencyString();
        diamondText.text = diamond.ToCurrencyString();

        GameManager.Instance.currencyManager.onCurrencyChanged.AddListener(OnCurrencyChanged);
    }

    private void OnPressedAddMoneyButton()
    {
        UIManager.Instance.Push(ShopMenu.Get());
    }

    private Tween dollarTween;
    private Tween diamondTween;
    private void OnCurrencyChanged(Currency type, int amount)
    {
        switch (type)
        {
            case Currency.Dollar:
                dollarTween?.Kill();
                float f1 = (float)dollar;
                dollarTween = DOTween.To(() => f1, x => f1 = x, amount, 0.5f)
                .OnUpdate(() =>
                {
                    dollarText.text = ((int)f1).ToCurrencyString();
                })
                .OnComplete(() =>
                {
                    dollar = amount;
                    dollarText.text = dollar.ToCurrencyString();
                    dollarTween.Kill();
                    dollarTween = null;
                });
                break;

            case Currency.Diamond:
                diamondTween?.Kill();
                float f2 = (float)diamond;
                diamondTween = DOTween.To(() => f2, x => f2 = x, amount, 0.5f)
                .OnUpdate(() =>
                {
                    diamondText.text = ((int)f2).ToCurrencyString();
                })
                .OnComplete(() =>
                {
                    diamond = amount;
                    diamondText.text = diamond.ToCurrencyString();
                    diamondTween.Kill();
                    diamondTween = null;
                });
                break;
        }
    }

    private void OnDestroy()
    {
        addMoneyButton?.onClick.RemoveAllListeners();
    }
}