using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class DelayHandler : MonoBehaviour
{
    public void Cancel()
    {
        if (delayCor != null) StopCoroutine(delayCor);
        Destroy(this);
    }

    public void SetDelay(float duration, UnityAction onComplete)
    {
        if (delayCor == null) delayCor = StartCoroutine(Delay(duration, onComplete));
    }

    private Coroutine delayCor = null;
    private IEnumerator Delay(float duration, UnityAction onComplete)
    {
        yield return new WaitForSeconds(duration);
        onComplete?.Invoke();
        Destroy(this);
    }

    public void SetDelay(int frameCount, UnityAction onComplete)
    {
        if (delayCor == null) delayCor = StartCoroutine(Delay(frameCount, onComplete));
    }

    private IEnumerator Delay(int frame, UnityAction onComplete)
    {
        for (int i = 0; i < frame; i++) yield return new WaitForEndOfFrame();
        onComplete?.Invoke();
        Destroy(this);
    }

    private void OnDestroy()
    {
        if (delayCor != null) StopCoroutine(delayCor);
    }
}