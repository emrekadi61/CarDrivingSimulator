using UnityEngine;
using UnityEngine.EventSystems;

public class InputManagerForOrbit : Singleton<InputManagerForOrbit>, IPointerDownHandler, IPointerUpHandler
{
    public float maxDistance = 100f; // Change this to according the desired precision 

    private Vector2 startedPos;

    private PointerEventData eventData;

    [HideInInspector] public Vector2 input;
    [HideInInspector] public Vector2 delta;
    [HideInInspector] public bool pressing { get { return eventData != null; } }


    public void OnPointerDown(PointerEventData _eventData)
    {
        eventData = _eventData;
        startedPos = eventData.position;
    }

    public void OnPointerUp(PointerEventData _eventData)
    {
        eventData = null;
        delta = Vector2.zero;
        startedPos = Vector2.zero;
        input = Vector2.zero;
    }

    private void Update()
    {
        if (eventData == null) return;
        delta = eventData.position - startedPos;
        delta.x = Mathf.Clamp(delta.x, -maxDistance, maxDistance);
        delta.y = Mathf.Clamp(delta.y, -maxDistance, maxDistance);
        input = delta / maxDistance;
        startedPos = eventData.position;
    }
}