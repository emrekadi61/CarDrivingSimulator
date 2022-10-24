using UnityEngine;
using UnityEngine.UI;

public class CustomSlider : MonoBehaviour
{
    public bool isVertical;
    public bool useBackgroundFillArea;
    public bool showValue;
    public float min = 0f;
    public float max = 1f;
    public float value = 0f;

    private RectTransform fillRect;
    private Image fill;
    private CustomSliderHandle handle;
    private RectTransform rect { get { return GetComponent<RectTransform>(); } }
    private UnityEngine.EventSystems.PointerEventData eventData { get { return handle.pointerEventData; } }
    private UnityEngine.Events.UnityAction<float> onValueChanged;

    public CustomSlider Construct(float _val, UnityEngine.Events.UnityAction<float> _onValueChanged)
    {
        value = _val;
        onValueChanged = _onValueChanged;
        handle = GetComponentInChildren<CustomSliderHandle>();
        rect.pivot = handle.rect.pivot = Vector2.one * 0.5f;

        if (useBackgroundFillArea)
        {
            fillRect = transform.Find("visual/fill").GetComponent<RectTransform>();
            fillRect.anchorMin = fillRect.anchorMax = fillRect.pivot = isVertical ? new Vector2(0.5f, 0f) : new Vector2(0f, 0.5f);
            // fill = fillRect.GetComponent<Image>();
            // fill.fillMethod = isVertical ? Image.FillMethod.Vertical : Image.FillMethod.Horizontal;
        }

        handle.text.gameObject.SetActive(showValue);

        if (isVertical)
        {
            handle.rect.anchoredPosition = new Vector2(0f, (((value - min) / (max - min)) * rect.sizeDelta.y));
            if (useBackgroundFillArea) fillRect.sizeDelta = new Vector2(fillRect.sizeDelta.x, handle.rect.anchoredPosition.y);
            // if (useBackgroundFillArea) fill.fillAmount = value / max;
        }
        else
        {
            handle.rect.anchoredPosition = new Vector2((((value - min) / (max - min)) * rect.sizeDelta.x), 0f);
            if (useBackgroundFillArea) fillRect.sizeDelta = new Vector2(handle.rect.anchoredPosition.x, fillRect.sizeDelta.y);
            // if (useBackgroundFillArea) fill.fillAmount = value / max;
        }

        if (showValue) handle.text.text = ((int)value).ToString();

        return this;
    }

    public void SetValue(float val)
    {
        value = val;
        if (isVertical)
        {
            handle.rect.anchoredPosition = new Vector2(0f, (((value - min) / (max - min)) * rect.sizeDelta.y));
            if (useBackgroundFillArea) fillRect.sizeDelta = new Vector2(fillRect.sizeDelta.x, handle.rect.anchoredPosition.y);
            // if (useBackgroundFillArea) fill.fillAmount = value / max;
        }
        else
        {
            handle.rect.anchoredPosition = new Vector2((((value - min) / (max - min)) * rect.sizeDelta.x), 0f);
            if (useBackgroundFillArea) fillRect.sizeDelta = new Vector2(handle.rect.anchoredPosition.x, fillRect.sizeDelta.y);
            // if (useBackgroundFillArea) fill.fillAmount = value / max;
        }
    }

    private void Update()
    {
        if (handle == null || eventData == null) return;

        Vector2 p = eventData.position;
        if (isVertical)
        {
            p.x = rect.position.x;
            handle.rect.position = p;

            Vector2 ap = handle.rect.anchoredPosition;
            ap.y = Mathf.Clamp(ap.y, 0f, rect.sizeDelta.y);
            handle.rect.anchoredPosition = ap;

            value = ((handle.rect.anchoredPosition.y / rect.sizeDelta.y) * (max - min)) + min;
            if (useBackgroundFillArea) fillRect.sizeDelta = new Vector2(fillRect.sizeDelta.x, handle.rect.anchoredPosition.y);
            // if (useBackgroundFillArea) fill.fillAmount = value / max;
        }
        else
        {
            p.y = rect.position.y;
            handle.rect.position = p;

            Vector2 ap = handle.rect.anchoredPosition;
            ap.x = Mathf.Clamp(ap.x, 0f, rect.sizeDelta.x);
            handle.rect.anchoredPosition = ap;

            value = ((handle.rect.anchoredPosition.x / rect.sizeDelta.x) * (max - min)) + min;
            if (useBackgroundFillArea) fillRect.sizeDelta = new Vector2(handle.rect.anchoredPosition.x, fillRect.sizeDelta.y);
            // if (useBackgroundFillArea) fill.fillAmount = value / max;
        }

        if (showValue) handle.text.text = ((int)value).ToString();
        onValueChanged?.Invoke(value);
    }

    private void OnDestroy()
    {
        onValueChanged = null;
    }
}