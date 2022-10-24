using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class ColorPickerCursor : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    private bool isCircle;
    private RectTransform cursorRect;
    private RectTransform colorSpectrumRect;
    private Image colorSpectrum { get { return colorSpectrumRect.GetComponent<Image>(); } }
    private RectTransform rect { get { return GetComponent<RectTransform>(); } }
    private PointerEventData eventData;

    private UnityAction<Color> onColorSelected;

    public void Construct(UnityAction<Color> onColorSelected, RectTransform colorSpectrumRect, RectTransform cursorRect, bool isCircle)
    {
        this.onColorSelected = onColorSelected;
        this.colorSpectrumRect = colorSpectrumRect;
        this.cursorRect = cursorRect;
        this.isCircle = isCircle;

        cursorRect.gameObject.SetActive(true);
        cursorRect.anchorMin = cursorRect.anchorMax = new Vector2(0.5f, 0.5f);
        cursorRect.pivot = Vector2.one * 0.5f;
        cursorRect.anchoredPosition = Vector2.zero;
    }

    public void Disable()
    {
        cursorRect.gameObject.SetActive(false);
    }

    public void OnPointerDown(PointerEventData _eventData)
    {
        eventData = _eventData;
    }

    public void OnPointerUp(PointerEventData _eventData)
    {
        eventData = null;
    }

    private void Update()
    {
        if (eventData == null) return;

        Vector2 p = eventData.position;
        cursorRect.position = p;

        Vector2 r = cursorRect.anchoredPosition;
        if (isCircle)
        {
            r = Vector2.ClampMagnitude(r, (rect.sizeDelta.x / 2f) - 3);
        }
        else
        {
            r.x = Mathf.Clamp(r.x, -rect.sizeDelta.x / 2f, rect.sizeDelta.x / 2f);
            r.y = Mathf.Clamp(r.y, -rect.sizeDelta.y / 2f, rect.sizeDelta.y / 2f);
        }

        cursorRect.anchoredPosition = r;

        onColorSelected.Invoke(GetColor(CastAPToCoord(rect, cursorRect, colorSpectrum.sprite), colorSpectrum.sprite));
    }

    private Color GetColor(Vector2Int coord, Sprite _spr)
    {
        Color c = _spr.texture.GetPixel(coord.x, coord.y); c.a = 1f;
        return c;
    }

    private Vector2Int CastAPToCoord(RectTransform _rect, RectTransform _cursor, Sprite _spr)
    {
        Vector2 _percent = (_cursor.anchoredPosition + (rect.sizeDelta / 2f)) / _rect.sizeDelta;
        Vector2 _sprSize = new Vector2(_spr.texture.width, _spr.texture.height);
        Vector2Int coord = new Vector2Int((int)(_sprSize.x * _percent.x), (int)(_sprSize.y * _percent.y));

        return coord;
    }
}