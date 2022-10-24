using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class NotificationPanel : MonoBehaviour
{
    public RectTransform textRect;
    public RectTransform iconRect;

    private Canvas canvas;
    private RectTransform rect;
    private Text text;
    private Image icon;


    public void Construct(string notification, Sprite sprite, float duration)
    {
        Construct(notification, 0.5f, duration, sprite);
    }

    public void Construct(string notification, float duration = 1.5f, float appearDuration = 0.5f, Sprite sprite = null)
    {
        canvas = GetComponent<Canvas>();
        canvas.sortingOrder = 100;
        text = textRect.GetComponent<Text>();
        icon = iconRect.GetComponent<Image>();

        rect = transform.GetChild(0).GetComponent<RectTransform>();
        text.text = notification;
        if (sprite)
        {
            icon.gameObject.SetActive(true);
            icon.sprite = sprite;
        }
        else
        {
            icon.gameObject.SetActive(false);
        }

        Appear(appearDuration, duration);
    }

    private void Appear(float appearDuration = 0.5f, float duration = 1.5f)
    {
        rect.anchoredPosition += new Vector2(0f, 1000f);
        DOTween
        .Sequence()
        .Append(rect.DOAnchorPosY(rect.anchoredPosition.y - 1000f, appearDuration))
        .AppendInterval(duration)
        .Append(rect.DOAnchorPosY(rect.anchoredPosition.y + 1000f, appearDuration))
        .OnComplete(() => Destroy(gameObject));
    }

    public static void Get(string notification, float duration = 1.5f, Sprite sprite = null)
    {
        Instantiate(Resources.Load<GameObject>("_ui/notification-canvas")).GetComponent<NotificationPanel>().Construct(notification, sprite, duration);
    }
}