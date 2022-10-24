using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Utils.Strings;
using Utils.UI;
using DG.Tweening;

public class LuckyWheelPanel : MonoBehaviour
{
    public RectTransform walletRect;
    public RectTransform backgroundRect;
    public RectTransform patternRect;
    public RectTransform headerRect;
    public RectTransform wheelPanelRect;
    public RectTransform wheelRect;
    public RectTransform spinButtonRect;
    public RectTransform noThanksButtonRect;
    public RectTransform claimButtonRect;
    public RectTransform claim2XbuttonRect;
    public RectTransform rewardContainerRect;

    public AnimationCurve animationCurve;

    private int rewardCount = 8;
    private Price[] prices;
    private RectTransform[] rewards;
    private Image background;
    private Image pattern;
    private Button spinButton;
    private Button noThanksButton;
    private Button claimButton;
    private Button claim2XButton;

    private UnityAction onComplete;
    private int rewardIndex;
    private bool secondTour;

    private LuckyWheelPanel Construct(UnityAction onComplete = null)
    {
        this.onComplete = onComplete;
        Align();
        background = backgroundRect.GetComponent<Image>();
        pattern = patternRect.GetComponent<Image>();
        spinButton = spinButtonRect.GetButton(OnPressedSpinButton);
        Appear();

        GameManager.Instance.analyticManager.Showed("lucky-wheel-panel");

        return this;
    }

    private void Appear(float duration = 0.25f)
    {
        Color c = background.color; c.a = 0f; background.color = c;
        c = pattern.color; c.a = 0f; pattern.color = c;

        headerRect.anchoredPosition += new Vector2(0f, 1000f);
        spinButtonRect.anchoredPosition += new Vector2(0f, -1000f);
        noThanksButtonRect.anchoredPosition += new Vector2(0f, -1000f);
        claimButtonRect.anchoredPosition += new Vector2(0f, -1000f);
        claim2XbuttonRect.anchoredPosition += new Vector2(0f, -1000f);
        wheelPanelRect.localScale = Vector3.zero;

        background.DOFade(1f, duration); pattern.DOFade(0.1f, duration);
        headerRect.DOAnchorPosY(headerRect.anchoredPosition.y - 1000f, duration);
        spinButtonRect.DOAnchorPosY(spinButtonRect.anchoredPosition.y + 1000f, duration);
        wheelPanelRect.DOScale(1f, duration);

        spinButtonRect.Find("video-icon").gameObject.SetActive(false);
    }

    private void Disappear(float duration = 0.25f)
    {
        background.DOFade(0f, duration); pattern.DOFade(0f, duration);
        headerRect.DOAnchorPosY(headerRect.anchoredPosition.y + 1000f, duration);
        claimButtonRect.DOAnchorPosY(claimButtonRect.anchoredPosition.y - 1000f, duration);
        claim2XbuttonRect.DOAnchorPosY(claim2XbuttonRect.anchoredPosition.y - 1000f, duration);
        wheelPanelRect.DOScale(0f, duration).OnComplete(() => Destroy(gameObject));
    }

    private void OnPressedSpinButton()
    {
        spinButton?.onClick.RemoveAllListeners();
        spinButtonRect.DOAnchorPosY(spinButtonRect.anchoredPosition.y - 1000f, 0.25f);

        float perAngle = (Mathf.PI * 2f) / rewardCount;
        rewardIndex = Random.Range(0, prices.Length);
        float targetAngle = (rewardIndex * perAngle) + ((Mathf.PI * 2f) * Random.Range(3, 5)) + offsetAngle;
        targetAngle *= Mathf.Rad2Deg;

        if (secondTour)
        {
            GameManager.Instance.adManager.ShowRewarded(b =>
            {
                if (b)
                {
                    noThanksButton?.onClick.RemoveAllListeners();
                    noThanksButtonRect.DOComplete();
                    noThanksButtonRect.DOAnchorPosY(noThanksButtonRect.anchoredPosition.y - 1000f, 0.25f);

                    GameManager.Instance.audioManager.Play("spin-wheel-1");
                    float t = 0f;
                    wheelRect.DOComplete();
                    wheelRect
                    .DOLocalRotate(new Vector3(0, 0, targetAngle), 5f, RotateMode.FastBeyond360)
                    .SetEase(Ease.OutQuart)
                    .OnUpdate(() =>
                    {
                        t += Time.deltaTime;
                        float p = animationCurve.Evaluate(t / 5f);
                        GameManager.Instance.audioManager.SetPitch("spin-wheel-1", 1.5f - p);
                    })
                    .OnComplete(OnSpinEnd);
                    GameManager.Instance.analyticManager.Clicked("lucky-wheel-panel", "spin-button-reward");
                }
            });
            return;
        }

        GameManager.Instance.audioManager.Play("spin-wheel-1");
        float t = 0f;
        wheelRect.DOComplete();
        wheelRect
        .DOLocalRotate(new Vector3(0, 0, targetAngle), 5f, RotateMode.FastBeyond360)
        .SetEase(Ease.OutQuart)
        .OnUpdate(() =>
        {
            t += Time.deltaTime;
            float p = animationCurve.Evaluate(t / 5f);
            GameManager.Instance.audioManager.SetPitch("spin-wheel-1", 1.5f - p);
        })
        .OnComplete(OnSpinEnd);
        GameManager.Instance.analyticManager.Clicked("lucky-wheel-panel", "spin-button");
    }

    private void OnPressedClaimButton()
    {
        claimButton?.onClick.RemoveAllListeners();
        claim2XButton?.onClick.RemoveAllListeners();

        float duration = 0.25f;

        claimButtonRect.DOComplete(); claim2XbuttonRect.DOComplete();
        spinButtonRect.DOComplete(); noThanksButtonRect.DOComplete();

        claimButtonRect.DOAnchorPosY(claimButtonRect.anchoredPosition.y - 1000f, duration);
        claim2XbuttonRect.DOAnchorPosY(claim2XbuttonRect.anchoredPosition.y - 1000f, duration);

        GameManager.Instance.currencyManager.Earn(prices[rewardIndex]);
        GameManager.Instance.analyticManager.Clicked("lucky-wheel-panel", "claim-button");

        Animate();
    }

    private void OnPressedClaim2XButton()
    {
        GameManager.Instance.adManager.ShowRewarded(b =>
        {
            if (b)
            {
                GameManager.Instance.currencyManager.Earn(prices[rewardIndex]);
                OnPressedClaimButton();
                GameManager.Instance.analyticManager.Clicked("lucky-wheel-panel", "claim-2x-button");
            }
        });
    }

    private void OnPressedNoThanksButton()
    {
        float duration = 0.25f;
        spinButtonRect.DOAnchorPosY(spinButtonRect.anchoredPosition.y - 1000f, duration);
        noThanksButtonRect.DOAnchorPosY(noThanksButtonRect.anchoredPosition.y - 1000f, duration);
        Disappear();

        GameManager.Instance.analyticManager.Clicked("lucky-wheel-panel", "no-thanks-button");
    }

    private void OnSpinEnd()
    {
        GameManager.Instance.audioManager.Stop("spin-wheel-1");
        float duration = 0.25f;
        spinButtonRect.Find("video-icon").gameObject.SetActive(true);

        rewards[rewardIndex].DOScale(1.5f, duration);

        claimButton = claimButtonRect.GetButton(OnPressedClaimButton);
        claim2XButton = claim2XbuttonRect.GetButton(OnPressedClaim2XButton);

        claim2XbuttonRect.DOAnchorPosY(claim2XbuttonRect.anchoredPosition.y + 1000f, duration);
        claimButtonRect.DOAnchorPosY(claimButtonRect.anchoredPosition.y + 1000f, duration).SetDelay(duration * 2f);
    }

    private void Animate()
    {
        RectTransform sample = rewards[rewardIndex].Find("icon").GetComponent<RectTransform>();

        for (int i = 0; i < Mathf.Clamp(prices[rewardIndex].amount, 1, 10); i++)
        {
            RectTransform r = Instantiate(sample, transform).GetComponent<RectTransform>();

            r.position = sample.position;
            r.eulerAngles = sample.eulerAngles;
            r.sizeDelta = sample.sizeDelta * sample.parent.localScale.x;

            float dur = Random.Range(0.5f, 1.5f);
            float del = Random.Range(0.1f, 0.25f);

            r.DOMove(walletRect.position, dur).SetDelay(del).OnComplete(() => Destroy(r.gameObject));
            r.DORotate(Vector3.zero, dur).SetDelay(del);
        }
        GameManager.Instance.delayManager.Set(1.75f, RecalculateRewards);
    }

    private void RecalculateRewards()
    {
        if (secondTour)
        {
            Disappear();
            return;
        }

        rewards[rewardIndex].DOScale(1f, 0.25f).OnComplete(() =>
        {
            for (int i = 0; i < rewardCount; i++)
            {
                Price p = prices[i];
                p.amount *= 2;
                rewards[i].Find("text").GetComponent<Text>().DOText(p.amount.ToCurrencyString(), 0.25f);
            }
        });

        float duration = 0.25f;
        spinButtonRect.DOAnchorPosY(spinButtonRect.anchoredPosition.y + 1000f, duration);
        noThanksButtonRect.DOAnchorPosY(noThanksButtonRect.anchoredPosition.y + 1000f, duration).SetDelay(2f);

        secondTour = true;
        spinButton = spinButtonRect.GetButton(OnPressedSpinButton);
        noThanksButton = noThanksButtonRect.GetButton(OnPressedNoThanksButton);
    }

    private float offsetAngle;
    private void Align()
    {
        RectTransform sample = rewardContainerRect.GetChild(0).GetComponent<RectTransform>();

        float perAngle = (Mathf.PI * 2f) / rewardCount;
        offsetAngle = perAngle / 2f;
        float radius = 200f;

        prices = new Price[rewardCount];
        rewards = new RectTransform[rewardCount];

        for (int i = 0; i < rewardCount; i++)
        {
            RectTransform r = Instantiate(sample, rewardContainerRect).GetComponent<RectTransform>();
            float angle = (perAngle * i) + offsetAngle;
            r.anchoredPosition = new Vector2(Mathf.Sin(angle), Mathf.Cos(angle)) * radius;
            r.localEulerAngles = new Vector3(0f, 0f, -Mathf.Rad2Deg * angle);

            Price p = new Price();
            if (Random.Range(0f, 1f) < 0.2f)
            {
                p.type = 1;
                p.amount = Random.Range(1, 3);
            }
            else
            {
                p.type = 0;
                p.amount = Random.Range(1, 6) * 500;
            }

            SetPrice(r, p);
            prices[i] = p;
            rewards[i] = r;
        }

        Destroy(sample.gameObject);
    }

    private void SetPrice(RectTransform r, Price p)
    {
        Image img = r.Find("icon").GetComponent<Image>();
        Text text = r.Find("text").GetComponent<Text>();
        img.color = p.type == 0 ? GameManager.Instance.statics.dollarColor : GameManager.Instance.statics.diamondColor;
        img.sprite = p.type == 0 ? GameManager.Instance.statics.dollarSprite : GameManager.Instance.statics.diamondSprite;
        text.text = p.amount.ToCurrencyString();
    }

    private void OnDestroy()
    {
        onComplete?.Invoke();
        noThanksButton?.onClick.RemoveAllListeners();
        spinButton?.onClick.RemoveAllListeners();
        claimButton?.onClick.RemoveAllListeners();
        claim2XButton?.onClick.RemoveAllListeners();
    }

    public static EKCanvas Get(UnityAction onComplete = null)
    {
        GameObject prefab = Resources.Load<GameObject>("_ui/_lucky-wheel-panel/lucky-wheel-canvas");
        LuckyWheelPanel lwp = Instantiate(prefab).GetComponent<LuckyWheelPanel>().Construct(onComplete);
        return lwp.gameObject.AddComponent<EKCanvas>();
    }
}