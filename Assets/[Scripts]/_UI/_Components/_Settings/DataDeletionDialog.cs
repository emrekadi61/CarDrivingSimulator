using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using DG.Tweening;
using Utils.UI;

public class DataDeletionDialog : MonoBehaviour
{
    public RectTransform backgroundRect;
    public RectTransform panelRect;
    public RectTransform yesButtonRect;
    public RectTransform noButtonRect;

    private Image background;
    private Button yesButton;
    private Button noButton;

    private UnityAction<bool> onComplete;

    public void Construct(UnityAction<bool> onComplete)
    {
        this.onComplete = onComplete;

        background = backgroundRect.GetComponent<Image>();
        yesButton = yesButtonRect.GetButton(OnPressedYesButton);
        noButton = noButtonRect.GetButton(OnPressedNoButton);
        Appear();
    }

    private void Appear(float duration = 0.25f)
    {
        Color c = background.color; c.a = 0f; background.color = c;
        background.DOFade(0.8f, duration);

        panelRect.localScale = new Vector3(0f, 1f, 1f);
        panelRect.DOScaleX(1f, duration);
    }

    private void Disappear(float duration = 0.25f)
    {
        background.DOFade(0f, duration);
        panelRect.DOScaleX(0f, duration).OnComplete(() => Destroy(gameObject));
    }

    private void OnPressedYesButton()
    {
        onComplete?.Invoke(true);
        Disappear();
    }

    private void OnPressedNoButton()
    {
        onComplete?.Invoke(false);
        Disappear();
    }

    private void OnDestroy()
    {
        panelRect.DOKill(); background.DOKill();
        yesButton?.onClick.RemoveAllListeners();
        noButton?.onClick.RemoveAllListeners();
    }

    public static void Get(UnityAction<bool> onComplete, Transform context)
    {
        Instantiate(Resources.Load<GameObject>("_ui/_settings/data-deletion-panel"), context)
        .GetComponent<DataDeletionDialog>().Construct(onComplete);
    }
}