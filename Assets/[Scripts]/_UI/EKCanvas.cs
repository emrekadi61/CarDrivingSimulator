using UnityEngine;
using UnityEngine.Events;
using DG.Tweening;

[RequireComponent(typeof(CanvasGroup))]
public class EKCanvas : MonoBehaviour
{
    public UIPanel panel;

    public Canvas canvas { get { return GetComponent<Canvas>(); } }
    private CanvasGroup group { get { return GetComponent<CanvasGroup>(); } }

    public EKCanvas Construct(UIPanel panel = null)
    {
        this.panel = panel;
        return this;
    }

    public void Fade(bool isOpen, UnityAction onComplete = null, float duration = 0.25f)
    {
        if (isOpen) gameObject.SetActive(true);

        group
        .DOFade(isOpen ? 1f : 0f, duration)
        .OnComplete(() =>
        {
            if (!isOpen) gameObject.SetActive(false);
            onComplete?.Invoke();
        });
    }

    private void OnDestroy()
    {
        UIManager.Instance?.Pop(this);
    }
}