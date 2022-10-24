using System.Collections;
using UnityEngine;

public class AdManagerInGame : MonoBehaviour
{
    private float frequence;
    private float noticeDuration = 5f;

    public AdManagerInGame Construct(float frequence)
    {
        this.frequence = frequence;

        GameManager.Instance.delayManager.Set(1, () =>
        {
            if (UIManager.Instance.GetComponentInChildren<RatePanel>() == null)
            {
                GameManager.Instance.adManager.ShowInterstitial(() =>
                {
                    UIManager.Instance.Push(NoAdsAdvisePanel.Get());
                    if (timerCor == null) timerCor = StartCoroutine(TimerCoroutine());
                });
            }
        });

        return this;
    }

    private void OnDestroy()
    {
        if (timerCor != null) StopCoroutine(timerCor);
    }

    private Coroutine timerCor;
    private IEnumerator TimerCoroutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(frequence - noticeDuration);
            UIManager.Instance.Push(AdManagerInGameCounter.Get(noticeDuration));
            yield return new WaitForSeconds(noticeDuration);
        }
    }
}