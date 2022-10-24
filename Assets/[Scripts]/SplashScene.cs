using System.Collections;
using UnityEngine;
using DG.Tweening;

public class SplashScene : MonoBehaviour
{
    private RectTransform iconRect;
    private RectTransform loadingIconRect;

    private void Start()
    {
        iconRect = transform.Find("icon").GetComponent<RectTransform>();
        loadingIconRect = transform.Find("loading-icon").GetComponent<RectTransform>();

        iconRect.DOAnchorPosY(iconRect.anchoredPosition.y - 1000f, 1.5f).SetEase(Ease.OutBounce);
        loadingIconRect.DOLocalRotateQuaternion(Quaternion.Euler(0f, 0f, 180f), 1f).SetLoops(-1, LoopType.Incremental);

        GameManager.Instance.delayManager.Set(2f, () => StartCoroutine(WaitManagers()));
    }

    private IEnumerator WaitManagers()
    {
        yield return new WaitUntil(() => GameManager.Instance.readyForStart);
        SceneLoader.Get("Showroom", true);
    }

    private void OnDestroy()
    {
        iconRect?.DOKill();
        loadingIconRect?.DOKill();
    }
}