using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class MirrorButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    private UnityAction onPressed;
    public UnityAction onReleased;

    public MirrorButton Construct(UnityAction onPressed, UnityAction onReleased)
    {
        this.onPressed = onPressed;
        this.onReleased = onReleased;
        return this;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        onPressed?.Invoke();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        onReleased?.Invoke();
    }
}
