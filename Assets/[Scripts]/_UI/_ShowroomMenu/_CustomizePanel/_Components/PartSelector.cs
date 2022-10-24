using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class PartSelector : MonoBehaviour
{
    public float enableAlpha = 0.8f;
    public float disableAlpha = 0.6f;

    public Color enableTextColor;
    public Color disableTextColor;

    public Part[] parts;

    public int selection = 0;

    public OnPartChanged onPartChanged = new OnPartChanged();

    private void Start()
    {
        Set(selection);
    }

    private void Set(int index)
    {
        selection = index;
        for (int i = 0; i < parts.Length; i++)
        {
            Part p = parts[i];
            Color c = p.background.color;
            c.a = i == index ? enableAlpha : disableAlpha;
            p.background.color = c;
            p.text.color = i == index ? enableTextColor : disableTextColor;

            p.button.onClick.RemoveAllListeners();
            int ind = i;
            p.button.onClick.AddListener(() =>
            {
                Set(ind);
                onPartChanged?.Invoke(ind);
                GameManager.Instance.audioManager.Play("tap");
            });
        }
    }

    private void OnDestroy()
    {
        onPartChanged?.RemoveAllListeners();
    }

    [System.Serializable]
    public class Part
    {
        public Image background;
        public Text text;
        public Button button;
    }

    public class OnPartChanged : UnityEvent<int> { }
}