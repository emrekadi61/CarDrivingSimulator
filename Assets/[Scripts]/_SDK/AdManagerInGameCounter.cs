using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class AdManagerInGameCounter : MonoBehaviour
{
    public RectTransform panelRect;
    public RectTransform sliderRect;

    private Image slider;

    private float secondRef;
    private float second;
    private bool completed;

    public AdManagerInGameCounter Construct(float second)
    {
        this.second = this.secondRef = second;
        slider = sliderRect.GetComponent<Image>();
        return this;
    }

    private void Appear(float duration = 0.25f)
    {
        panelRect.localScale = Vector3.zero;
        panelRect.DOScale(1f, duration).SetEase(Ease.OutBack);
    }

    private void Disappear(float duration = 0.25f)
    {
        panelRect
        .DOScale(0f, duration)
        .SetEase(Ease.InBack)
        .OnComplete(() =>
        {
            GameManager.Instance.adManager.ShowInterstitial(() =>
            {
                UIManager.Instance.Push(NoAdsAdvisePanel.Get());
                Destroy(gameObject);
            });
        });
    }

    private void Update()
    {
        if (completed) return;

        second -= Time.deltaTime;
        slider.fillAmount = second / secondRef;
        if (second <= 0f)
        {
            completed = true;
            Disappear();
        }
    }

    public static EKCanvas Get(float second)
    {
        AdManagerInGameCounter c = Instantiate(Resources.Load<GameObject>("_ui/ingame-ad-counter-canvas")).GetComponent<AdManagerInGameCounter>().Construct(second);
        return c.gameObject.AddComponent<EKCanvas>().Construct(null);
    }
}