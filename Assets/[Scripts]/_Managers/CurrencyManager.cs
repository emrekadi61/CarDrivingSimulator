using UnityEngine.Events;

public class CurrencyManager : ManagerBase
{
    public int dollar;
    public int diamond;

    public OnCurrencyChanged onCurrencyChanged = new OnCurrencyChanged();

    private User.GameData gameData { get { return GameManager.Instance.dataManager.user.gameData; } }
    private UnityAction<bool> result;

    private void Start()
    {
        dollar = gameData.dollar;
        diamond = gameData.diamond;
    }

    public void Pay(Price price)
    {
        Currency cr = price.type == 0 ? Currency.Dollar : Currency.Diamond;
        switch (cr)
        {
            case Currency.Dollar:

                dollar -= price.amount;
                onCurrencyChanged?.Invoke(cr, dollar);
                break;
            case Currency.Diamond:
                diamond -= price.amount;
                onCurrencyChanged?.Invoke(cr, diamond);
                break;
        }
    }

    public void PayWithoutConfirmation(Price price, UnityAction<bool> result)
    {
        this.result = result;
        Currency cr = price.type == 0 ? Currency.Dollar : Currency.Diamond;

        if (!HaveEnoughCurrency(cr, price.amount))
        {
            this.result?.Invoke(false);
            this.result = null;
            return;
        }

        switch (cr)
        {
            case Currency.Dollar:
                dollar -= price.amount;
                onCurrencyChanged?.Invoke(cr, dollar);
                this.result?.Invoke(true);
                this.result = null;
                break;
            case Currency.Diamond:
                diamond -= price.amount;
                onCurrencyChanged?.Invoke(cr, diamond);
                this.result?.Invoke(true);
                this.result = null;
                break;
        }
    }

    public void Pay(Price price, UnityAction<bool> result)
    {
        this.result = result;
        Currency cr = price.type == 0 ? Currency.Dollar : Currency.Diamond;
        switch (cr)
        {
            case Currency.Dollar:
                UIManager.Instance.Push(PaymentConfirmation.Get(price, b =>
                {
                    if (!b)
                    {
                        this.result?.Invoke(false); this.result = null;
                        return;
                    }

                    dollar -= price.amount;
                    onCurrencyChanged?.Invoke(cr, dollar);
                    this.result?.Invoke(true); this.result = null;
                }));
                break;
            case Currency.Diamond:
                UIManager.Instance.Push(PaymentConfirmation.Get(price, b =>
                {
                    if (!b)
                    {
                        this.result?.Invoke(false); this.result = null;
                        return;
                    }

                    diamond -= price.amount;
                    onCurrencyChanged?.Invoke(cr, diamond);
                    this.result?.Invoke(true); this.result = null;
                }));
                break;
        }
    }

    public void Earn(Price price)
    {
        Currency cr = price.type == 0 ? Currency.Dollar : Currency.Diamond;
        switch (cr)
        {
            case Currency.Dollar:
                dollar += price.amount;
                onCurrencyChanged?.Invoke(cr, dollar);
                break;
            case Currency.Diamond:
                diamond += price.amount;
                onCurrencyChanged?.Invoke(cr, diamond);
                break;
        }
    }

    public bool HaveEnoughCurrency(Currency _type, int _amount)
    {
        if (_type == Currency.Dollar && dollar >= _amount) return true;
        else if (_type == Currency.Diamond && diamond >= _amount) return true;
        return false;
    }
}

[System.Serializable]
public enum Currency : int
{
    Dollar = 0,
    Diamond = 1
}
public class OnCurrencyChanged : UnityEvent<Currency, int> { }