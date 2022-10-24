using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class CustomSliderHandle : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [HideInInspector] public RectTransform rect { get { return GetComponent<RectTransform>(); } }
    [HideInInspector] public PointerEventData pointerEventData;
    public Text text;

    private void Awake()
    {
        text = GetComponentInChildren<Text>();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        pointerEventData = eventData;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        pointerEventData = null;
    }
}