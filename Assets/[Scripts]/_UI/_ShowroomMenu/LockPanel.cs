using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using Utils.UI;
using Utils.Strings;

public class LockPanel : MonoBehaviour
{
    public RectTransform priceTextRect;
    public RectTransform priceIconRect;
    public RectTransform buyButtonRect;

    private Text priceText;
    private Image priceIcon;
    private Button buyButton;

    private UnityAction onPressedBuy;

    public void Active(bool isActive, UnityAction onPressedBuy = null, int price = -1, Currency currenyType = Currency.Dollar)
    {
        gameObject.SetActive(isActive);
        buyButton?.onClick.RemoveAllListeners();
        
        if (!isActive) return;
        else if (buyButton) buyButton.onClick.AddListener(OnPressedButton);

        this.onPressedBuy = onPressedBuy;
        if (!priceText) priceText = priceTextRect.GetComponent<Text>();
        if (!priceIcon) priceIcon = priceIconRect.GetComponent<Image>();
        if (!buyButton) buyButton = buyButtonRect.GetButton(OnPressedButton);

        priceText.text = price.ToCurrencyString();
        priceIcon.sprite = Resources.Load<Sprite>("_sprites/_currency/" + currenyType.ToString().ToLower());
        priceIcon.color = currenyType == Currency.Dollar ? GameManager.Instance.statics.dollarColor : GameManager.Instance.statics.diamondColor;
    }

    private void OnPressedButton()
    {
        onPressedBuy?.Invoke();
    }

    private void OnDestroy()
    {
        buyButton?.onClick.RemoveAllListeners();
    }
}