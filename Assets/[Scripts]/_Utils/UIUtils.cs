namespace Utils.UI
{
    using UnityEngine;
    using UnityEngine.UI;
    using UnityEngine.Events;

    public static class UIUtils
    {
        public static Button GetButton(this RectTransform rect, UnityAction onClick)
        {
            Button b = rect.GetComponent<Button>();
            if (b == null) b = rect.gameObject.AddComponent<Button>();
            b.onClick.AddListener(onClick);
            return b;
        }
    }
}