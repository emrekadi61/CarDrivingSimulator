using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class ColorPicker : MonoBehaviour
{
    public RectTransform rect;
    public RectTransform colorSpectrumRect;
    public RectTransform cursorRect;
    public bool isCircle;

    public CustomSlider blackSpectrum;

    private Vector2 sizeDelta;
    public Color color;
    private float black = 0.5f;
    private UnityAction<Color> onColorSelected;
    private Image colorSpectrumImage;

    [HideInInspector] public ColorPickerCursor cursor;
    [HideInInspector] public bool isActive;

    public ColorPicker Construct(UnityAction<Color> onColorSelected, Color color)
    {
        this.onColorSelected = onColorSelected;
        this.color = color;

        colorSpectrumImage = colorSpectrumRect.GetComponent<Image>();

        sizeDelta = rect.sizeDelta;
        cursor = GetComponentInChildren<ColorPickerCursor>();
        cursor.Construct(c => OnColorSelected(c, black), colorSpectrumRect, cursorRect, isCircle);

        blackSpectrum?.Construct(black, f => OnColorSelected(this.color, f));

        return this;
    }

    private void OnColorSelected(Color _color, float _black)
    {
        this.color = _color;
        black = _black;
        Color tmp = Color.Lerp(this.color, Color.black , black / 1f);
        colorSpectrumImage.color = Color.Lerp(Color.white, Color.black, black / 1f);
        onColorSelected?.Invoke(tmp);
    }
}