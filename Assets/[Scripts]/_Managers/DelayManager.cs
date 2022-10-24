using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DelayManager : ManagerBase
{
    private List<DelayHandler> delayQueue = new List<DelayHandler>();

    public DelayHandler Set(float duration, UnityAction onComplete)
    {
        DelayHandler dh = gameObject.AddComponent<DelayHandler>();
        delayQueue.Add(dh);
        dh.SetDelay(duration, () =>
        {
            delayQueue.Remove(dh);
            onComplete?.Invoke();
        });

        return dh;
    }

    public DelayHandler Set(int frameCount, UnityAction onComplete)
    {
        DelayHandler dh = gameObject.AddComponent<DelayHandler>();
        delayQueue.Add(dh);
        dh.SetDelay(frameCount, () =>
        {
            delayQueue.Remove(dh);
            onComplete?.Invoke();
        });

        return dh;
    }
}