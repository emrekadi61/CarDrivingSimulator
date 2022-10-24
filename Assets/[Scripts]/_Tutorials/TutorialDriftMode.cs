using UnityEngine;
using DG.Tweening;

public class TutorialDriftMode : MonoBehaviour
{
    public RectTransform rect;

    private readonly string DATA = "tutorial-drift-mode-count";
    private int showCount = 5;
    private int shownCount = 0;

    public TutorialDriftMode Construct(RectTransform modeRect)
    {
        rect.anchorMin = modeRect.anchorMin;
        rect.anchorMax = modeRect.anchorMax;
        rect.pivot = modeRect.pivot;
        rect.anchoredPosition = modeRect.anchoredPosition + new Vector2(modeRect.rect.size.x + 30f, -(modeRect.rect.size.y - rect.rect.size.y) / 2f);

        shownCount = PlayerPrefs.GetInt(DATA, 0);
        PlayerPrefs.SetInt(DATA, ++shownCount);

        if (shownCount > showCount)
        {
            Destroy(gameObject);
        }
        else
        {

            float duration = 1f;
            rect
            .DOAnchorPosX(rect.anchoredPosition.x + 100f, duration)
            .SetLoops(6, LoopType.Yoyo)
            .OnComplete(() =>
            {
                rect.DOScaleX(0f, 0.25f).OnComplete(() => Destroy(gameObject));
            });
        }
        return this;
    }

    private void OnDestroy()
    {
        rect.DOKill();
    }

    public static EKCanvas Get(RectTransform context)
    {
        TutorialDriftMode tdm = Instantiate(Resources.Load<GameObject>("_ui/_tutorials/drift-mode")).GetComponent<TutorialDriftMode>().Construct(context);
        return tdm.gameObject.AddComponent<EKCanvas>().Construct(null);
    }
}
