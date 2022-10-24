using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using Utils.UI;

public class Selector : MonoBehaviour
{
    public RectTransform[] buttonRects;
    public int value = 0;

    private UnityAction<int> onSelected;

    public Selector Construct(int index, UnityAction<int> onSelected)
    {
        this.onSelected = onSelected;

        value = index;
        for (int i = 0; i < buttonRects.Length; i++)
        {
            int a = i;
            buttonRects[i].GetButton(() => OnPressedButton(a));
        }
        ActivateButton(value);
        return this;
    }

    private void ActivateButton(int index)
    {
        for (int i = 0; i < buttonRects.Length; i++)
        {
            buttonRects[i].GetComponent<Image>().color = value == i ? GameManager.Instance.statics.buttonEnableColor : GameManager.Instance.statics.buttonDisableColor;
            Transform t = buttonRects[i].Find("pattern");
            if (t) t.gameObject.SetActive(value == i);
        }
    }

    private void OnPressedButton(int index)
    {
        value = index;
        ActivateButton(value);
        onSelected.Invoke(index);
        GameManager.Instance.audioManager.Play("tap");
    }
}