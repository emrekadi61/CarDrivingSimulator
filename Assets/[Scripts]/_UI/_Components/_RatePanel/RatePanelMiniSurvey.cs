using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using Utils.UI;
using DG.Tweening;

public class RatePanelMiniSurvey : MonoBehaviour
{
    public RectTransform panel;
    public RectTransform textRect;
    public RectTransform yesButtonRect;
    public RectTransform noButtonRect;

    private Text text;
    private RectTransform rect;
    private Button yesButton;
    private Button noButton;

    private UnityAction<bool> onComplete;

    public RatePanelMiniSurvey Construct(UnityAction<bool> onComplete, MiniSuvey data)
    {
        this.onComplete = onComplete;
        rect = GetComponent<RectTransform>();
        text = textRect.GetComponent<Text>();

        yesButton = yesButtonRect.GetButton(OnPressedYesButton);
        noButton = noButtonRect.GetButton(OnPressedNoButton);

        text.text = data.titleText;
        yesButtonRect.GetComponentInChildren<Text>().text = data.yesButtonText;
        noButtonRect.GetComponentInChildren<Text>().text = data.noButtonText;

        GameManager.Instance.analyticManager.Showed("rate-survey-panel");

        Appear();

        return this;
    }

    private void Appear(float duration = 0.25f)
    {
        panel.localScale = new Vector3(0f, 1f, 1f);
        panel.DOScaleX(1f, duration);
    }

    private void Disappear(float duration = 0.25f)
    {
        panel.DOScaleX(0f, duration).OnComplete(() => Destroy(gameObject));
    }

    private void OnPressedYesButton()
    {
        onComplete?.Invoke(true);
        Disappear();

        GameManager.Instance.analyticManager.Clicked("rate-survey-panel", "yes-button");
    }

    private void OnPressedNoButton()
    {
        onComplete?.Invoke(false);
        Disappear();

        GameManager.Instance.analyticManager.Clicked("rate-survey-panel", "no-button");
    }

    private void OnDestroy()
    {
        yesButton?.onClick.RemoveAllListeners();
        noButton?.onClick.RemoveAllListeners();
        onComplete = null;
    }

    public static void Get(UnityAction<bool> onComplete, RectTransform context, MiniSuvey data)
    {
        GameObject prefab = Resources.Load<GameObject>("_ui/_rate-panel/mini-survey");
        Instantiate(prefab, context).GetComponent<RatePanelMiniSurvey>().Construct(onComplete, data);
    }
}