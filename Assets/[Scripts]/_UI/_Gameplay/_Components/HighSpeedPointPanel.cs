using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Utils.Strings;

public class HighSpeedPointPanel : MonoBehaviour
{
    public RectTransform panelRect;
    public RectTransform textRect;

    private Text text;

    public HighSpeedPointPanel Construct()
    {
        text = textRect.GetComponent<Text>();
        return this;
    }

    private void Appear(float duration = 0.25f)
    {
        panelRect.anchoredPosition += new Vector2(1000f, 0f);
        text.text = "0";

        panelRect.DOAnchorPosX(panelRect.anchoredPosition.x - 1000f, duration);
    }

    public void SetPointText(float point)
    {
        text.text = ((int)point).ToCurrencyString();
    }

    public void Disappear(float duration = 0.25f)
    {
        panelRect.DOScale(1.5f, duration);
        panelRect.DOAnchorPosY(panelRect.anchoredPosition.y - 150f, duration);
        GameManager.Instance.delayManager.Set(1.1f, () =>
        {
            panelRect.DOAnchorPosX(panelRect.anchoredPosition.x + 1000f, duration).OnComplete(() => Destroy(gameObject));
        });
    }

    public static HighSpeedPointPanel Get()
    {
        HighSpeedPointPanel hspp = Instantiate(Resources.Load<GameObject>("_ui/_game/high-speed-canvas")).GetComponent<HighSpeedPointPanel>().Construct();
        UIManager.Instance.Push(hspp.gameObject.AddComponent<EKCanvas>());
        return hspp;
    }
}