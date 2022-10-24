using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using DG.Tweening;
using Utils.UI;
using Utils.Strings;

public class DailyRewardPanel : MonoBehaviour
{
    public RectTransform headerRect;
    public RectTransform claimButtonRect;
    public RectTransform claim2XButtonRect;
    public RectTransform rewardsContainerRect;
    public RectTransform activeRect;
    public RectTransform walletPoint;

    private int activeDay;

    private Image background;
    private Image pattern;
    private Button claimButton;
    private Button claim2XButton;
    private int[] rewards;
    private RectTransform[] rewardRects;
    private GameObject cardPrefab { get { return Resources.Load<GameObject>("_ui/_daily-reward/card"); } }
    private UnityAction onComplete;

    public DailyRewardPanel Construct(int activeDay, UnityAction onComplete = null)
    {
        this.onComplete = onComplete;
        this.activeDay = activeDay;
        rewards = GameManager.Instance.dataManager.dailyRewards.rewards.ToArray();

        activeDay = 6;

        background = transform.Find("background").GetComponent<Image>();
        pattern = transform.Find("background/pattern").GetComponent<Image>();

        claimButton = claimButtonRect.GetButton(OnPressedClaimButton);
        claim2XButton = claim2XButtonRect.GetButton(OnPressedClaim2XButton);
        Align();

        GameManager.Instance.analyticManager.Showed("daily-reward");

        return this;
    }

    private void Disappear(float duration = 0.25f)
    {
        for (int i = 0; i < rewardRects.Length; i++) rewardRects[i].DOScale(0f, duration);
        activeRect.GetComponent<Image>().DOFade(0f, duration);
        background.DOFade(0f, duration);
        pattern.DOFade(0f, duration);
        headerRect.DOAnchorPosY(headerRect.anchoredPosition.y + 1000f, duration);
        claimButtonRect.DOAnchorPosY(claimButtonRect.anchoredPosition.y - 1000f, duration);
        claim2XButtonRect.DOAnchorPosY(claim2XButtonRect.anchoredPosition.y - 1000f, duration)
        .OnComplete(() =>
        {
            onComplete?.Invoke();
            Destroy(gameObject);
        });
    }

    private void Align()
    {
        RectTransform sample = Instantiate(cardPrefab, transform).GetComponent<RectTransform>();
        int rowCount = 3;
        int columnCount = rewards.Length / rowCount;

        float paddingX = 75f;
        float paddingY = 75f;

        int counter = 0;
        rewardRects = new RectTransform[rewards.Length];

        for (int row = 0; row < rowCount; row++)
        {
            for (int col = 0; col < columnCount; col++)
            {
                RectTransform r = Instantiate(cardPrefab, rewardsContainerRect).GetComponent<RectTransform>();

                float xPos = -(((float)(columnCount - 1)) / 2f) * (sample.rect.size.x + paddingX); ;
                xPos += col * (sample.rect.size.x + paddingX);

                float yPos = (((float)(rowCount - 1)) / 2f) * (sample.rect.size.y + paddingY);
                yPos -= row * (sample.rect.size.y + paddingY);

                r.Find("head/text").GetComponent<Text>().text = "DAY " + (counter + 1).ToString();
                r.Find("text").GetComponent<Text>().text = rewards[counter].ToCurrencyString();

                r.anchoredPosition = new Vector2(xPos, yPos);

                rewardRects[counter] = r;

                counter++;
            }
        }

        activeRect.gameObject.SetActive(true);
        activeRect.sizeDelta = sample.sizeDelta * 1.2f;
        activeRect.position = rewardRects[activeDay].position;

        Destroy(sample.gameObject);
    }

    private void OnPressedClaimButton()
    {
        claimButton?.onClick.RemoveAllListeners();
        claim2XButton?.onClick.RemoveAllListeners();

        Animate();
        GameManager.Instance.delayManager.Set(1.75f, () => Disappear());
        GameManager.Instance.currencyManager.Earn(new Price(0, rewards[activeDay]));
        GameManager.Instance.dailyRewardSystem.OnEarnedReward();

        GameManager.Instance.analyticManager.Clicked("daily-reward", "claim");
    }

    private void OnPressedClaim2XButton()
    {
        GameManager.Instance.adManager.ShowRewarded(b =>
        {
            if (b)
            {
                GameManager.Instance.currencyManager.Earn(new Price(0, rewards[activeDay]));
                OnPressedClaimButton();
                GameManager.Instance.analyticManager.Clicked("daily-reward", "claim-2x");
            }
        });
    }

    private void Animate()
    {
        RectTransform sample = rewardRects[activeDay].Find("icon").GetComponent<RectTransform>();

        for (int i = 0; i < 10; i++)
        {
            RectTransform r = Instantiate(sample, sample.position, sample.rotation, transform);
            r
            .DOMove(walletPoint.position, Random.Range(0.5f, 1.5f))
            .SetDelay(Random.Range(0.1f, 0.25f))
            .OnComplete(() =>
            {
                Destroy(r.gameObject);
            });
        }
    }

    private void OnDestroy()
    {
        claimButton?.onClick.RemoveAllListeners();
        claim2XButton?.onClick.RemoveAllListeners();
    }

    public static EKCanvas Get(int activeDay, UnityAction onComplete = null)
    {
        GameObject prefab = Resources.Load<GameObject>("_ui/_daily-reward/daily-reward-canvas");
        DailyRewardPanel drp = Instantiate(prefab).GetComponent<DailyRewardPanel>().Construct(activeDay, onComplete);
        return drp.gameObject.AddComponent<EKCanvas>().Construct(null);
    }
}