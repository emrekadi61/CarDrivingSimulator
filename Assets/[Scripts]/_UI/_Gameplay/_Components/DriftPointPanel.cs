using UnityEngine;
using UnityEngine.UI;
using Utils.Strings;
using DG.Tweening;

public class DriftPointPanel : MonoBehaviour
{
    public RectTransform panelRect;
    public RectTransform pointTextRect;
    public RectTransform factorTextRect;
    public RectTransform leftSliderRect;
    public RectTransform rightSliderRect;

    private Text pointText;
    private Text factorText;
    private float sliderWidth;

    public DriftPointPanel Construct()
    {
        pointText = pointTextRect.GetComponent<Text>();
        factorText = factorTextRect.GetComponent<Text>();
        sliderWidth = leftSliderRect.parent.GetComponent<RectTransform>().rect.size.x;
        return this;
    }

    private void Appear(float duration = 0.25f)
    {
        panelRect.localScale = new Vector3(0f, 1f, 1f);
        panelRect.DOScaleX(1f, duration);
    }

    public void Disappear(float duration = 0.25f)
    {
        pointTextRect.SetParent(transform);
        panelRect.DOScaleX(0f, duration);
        pointTextRect.DOScale(2f, duration).SetDelay(duration);
        pointTextRect.DOAnchorPosY(pointTextRect.anchoredPosition.y + 100f, duration);
        GameManager.Instance.delayManager.Set(1.5f, () => Destroy(gameObject));
    }

    public void Set(float point, float factor, float percent)
    {
        factorText.text = "X" + factor;
        pointText.text = ((int)point).ToCurrencyString();
        leftSliderRect.sizeDelta = rightSliderRect.sizeDelta = new Vector2(percent * sliderWidth, leftSliderRect.rect.size.y);
    }

    public static DriftPointPanel Get()
    {
        DriftPointPanel dpp = Instantiate(Resources.Load<GameObject>("_ui/_game/drift-canvas")).GetComponent<DriftPointPanel>().Construct();
        UIManager.Instance.Push(dpp.gameObject.AddComponent<EKCanvas>().Construct());
        return dpp;
    }
}