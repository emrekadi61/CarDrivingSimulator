using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Utils.UI;

public class LuckyWheelCounter : MonoBehaviour
{
    public RectTransform textRect;

    private RectTransform rect;
    private Text text;
    private Button button;

    private void Start()
    {
        rect = GetComponent<RectTransform>();
        text = textRect.GetComponent<Text>();

        if (GameManager.Instance.luckyWheelSystem.timeLeft.TotalSeconds <= 0f) OnTime();
        else counterCor = StartCoroutine(Counter());
    }

    private void OnTime()
    {
        if (button) return;

        text.text = "READY!";
        text.color = GameManager.Instance.statics.dollarColor;
        button = rect.GetButton(OnPressedButton);
        if (counterCor != null)
        {
            StopCoroutine(counterCor);
            counterCor = null;
        }
    }

    private void OnPressedButton()
    {
        button?.onClick.RemoveAllListeners();
        UIManager.Instance.Push(LuckyWheelPanel.Get(OnPanelEnd));
        GameManager.Instance.analyticManager.Clicked("showroom-main-panel", "lucky-wheel");
    }

    private void OnPanelEnd()
    {
        GameManager.Instance.luckyWheelSystem.OnPanelEnd();
        counterCor = StartCoroutine(Counter());
    }

    private void OnDestroy()
    {
        button?.onClick.RemoveAllListeners();
        if (counterCor != null) StopCoroutine(counterCor);
        counterCor = null;
    }

    private Coroutine counterCor;
    private IEnumerator Counter()
    {
        text.color = Color.white;
        while (true)
        {
            TimeSpan ts = GameManager.Instance.luckyWheelSystem.timeLeft;
            text.text = ts.Hours.ToString("d2") + ":" + ts.Minutes.ToString("d2") + ":" + ts.Seconds.ToString("d2");
            if (ts.TotalSeconds <= 0f) OnTime();
            yield return new WaitForSeconds(1f);
        }
    }
}