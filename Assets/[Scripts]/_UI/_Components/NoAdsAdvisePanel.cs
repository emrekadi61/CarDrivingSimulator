using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Utils.UI;

public class NoAdsAdvisePanel : MonoBehaviour
{
    public RectTransform panelRect;
    public RectTransform textRect;
    public RectTransform yesButtonRect;
    public RectTransform noButtonRect;

    private Text text;
    private Button yesButton;
    private Button noButton;

    public NoAdsAdvisePanel Construct()
    {
        text = textRect.GetComponent<Text>();
        yesButton = yesButtonRect.GetButton(OnPressedYesButton);
        noButton = noButtonRect.GetButton(OnPressedNoButton);

        text.text = "DO YOU WANT TO REMOVE ADS FOR\n<size=35><color='#3CCF4E'><b>" + GameManager.Instance.sdkManager.iapManager.GetPrice("com.zekgames.cardrivingsim2022.no_ads") + "</b></color></size>";

        GameManager.Instance.analyticManager.Showed("no-ads-advise-panel");

        Appear();

        return this;
    }

    private DelayHandler dh;
    private void Appear(float duration = 0.25f)
    {
        panelRect.anchoredPosition += new Vector2(1000f, 0f);
        panelRect.DOAnchorPosX(panelRect.anchoredPosition.x - 1000f, duration);

        dh = GameManager.Instance.delayManager.Set(10f, OnTimeOut);
    }

    private void OnTimeOut()
    {
        Disappear();
        GameManager.Instance.analyticManager.Clicked("no-ads-advise-panel", "closed-self");
    }

    private void Disappear(float duration = 0.25f)
    {
        panelRect.DOAnchorPosX(panelRect.anchoredPosition.x + 1000f, duration).OnComplete(() => Destroy(gameObject));
    }

    private void OnPressedYesButton()
    {
        Disappear();
        if (dh) dh.Cancel();
        GameManager.Instance.sdkManager?.iapManager.Buy("com.zekgames.cardrivingsim2022.no_ads", b =>
        {
            if (b) GameManager.Instance.sdkManager?.OnBoughtedNoAds();
        });
        GameManager.Instance.analyticManager.Clicked("no-ads-advise-panel", "yes-button");
    }

    private void OnPressedNoButton()
    {
        Disappear();
        if (dh) dh.Cancel();
        GameManager.Instance.analyticManager.Clicked("no-ads-advise-panel", "no-button");
    }

    private void OnDestroy()
    {
        panelRect?.DOKill();
        yesButton?.onClick.RemoveAllListeners();
        noButton?.onClick.RemoveAllListeners();
    }

    public static EKCanvas Get()
    {
        NoAdsAdvisePanel p = Instantiate(Resources.Load<GameObject>("_ui/no-ads-advise-panel")).GetComponent<NoAdsAdvisePanel>().Construct();
        return p.gameObject.AddComponent<EKCanvas>().Construct(null);
    }
}