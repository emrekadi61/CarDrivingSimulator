using UnityEngine;
using UnityEngine.UI;
using Utils.Strings;
using DG.Tweening;

public class JumpPointPanel : MonoBehaviour
{
    public RectTransform rect;
    public RectTransform textRect;

    private Text text;

    public JumpPointPanel Construct()
    {
        text = textRect.GetComponent<Text>();
        return this;
    }

    private void Appear(float duration = 0.25f)
    {
        rect.localScale = new Vector3(0f, 1f, 1f);

        rect.DOScaleX(1f, duration);
    }

    public void Disappear(float duration = 0.25f)
    {
        textRect.SetParent(transform);
        textRect.DOScale(1.5f, duration);
        textRect.DOAnchorPosY(textRect.anchoredPosition.y + 50f, duration);
        rect.DOScaleX(0f, duration);
        GameManager.Instance.delayManager.Set(1.5f, () => Destroy(gameObject));
    }

    public void SetText(float val)
    {
        text.text = ((int)val).ToCurrencyString();
    }

    public static JumpPointPanel Get()
    {
        JumpPointPanel jpp = Instantiate(Resources.Load<GameObject>("_ui/_game/jump-canvas")).GetComponent<JumpPointPanel>().Construct();
        UIManager.Instance.Push(jpp.gameObject.AddComponent<EKCanvas>());
        return jpp;
    }
}