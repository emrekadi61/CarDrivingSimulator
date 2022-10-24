using UnityEngine;

public class CarCheckerDrift : MonoBehaviour
{
    private Car car;
    private RCC_CarControllerV3 rcc;
    private DriftPointPanel driftPanel;
    private DelayHandler delayHandler;

    private bool drifting;
    private float driftPointRange = 100f;
    private float pointFactor = 30f;
    private float point = 0f;
    private float temp = 0f;
    private int factor = 1;

    public CarCheckerDrift Construct(Car car)
    {
        this.car = car;
        this.rcc = car.rcc;
        LevelManager.Instance?.onGamePaused?.AddListener(this.OnGamePaused);
        return this;
    }

    private void FixedUpdate()
    {
        if (!rcc) return;

        if (rcc.driftingNow) Drifting();
        else DriftEnd();
    }

    private void Drifting()
    {
        drifting = true;
        if (delayHandler) delayHandler.Cancel();
        if (!driftPanel && point >= 1f) driftPanel = DriftPointPanel.Get();

        float increase = Time.fixedDeltaTime * Mathf.Abs(rcc.driftAngle) * pointFactor * car.data.pointFactor * Mathf.Clamp(rcc.speed / 50f, 1f, 3f);
        temp += increase;
        point += increase * factor;

        if (factor < 5 && temp >= driftPointRange)
        {
            temp = 0f;
            factor++;
        }

        driftPanel?.Set(point, factor, Mathf.Clamp01(temp / driftPointRange));
    }

    private void DriftEnd()
    {
        if (!drifting) return; drifting = false;
        if (!delayHandler) delayHandler = GameManager
                                            .Instance
                                            .delayManager
                                            .Set(2f, () =>
                                            {
                                                GameManager.Instance.currencyManager.Earn(new Price(0, (int)point));
                                                LevelManager.Instance.earnedDollar += (int)point;

                                                GameManager.Instance.dataManager.user.gameData.statistics.experience += (int)point;
                                                GameManager.Instance.dataManager.user.gameData.statistics.drift += (int)point;
                                                LevelManager.Instance.statistics.drift += (int)point;

                                                if (point > LevelManager.Instance.statistics.bestDrift)
                                                {
                                                    LevelManager.Instance.statistics.bestDrift = (int)point;
                                                    if (point > GameManager.Instance.dataManager.user.gameData.statistics.bestDrift)
                                                        GameManager.Instance.dataManager.user.gameData.statistics.bestDrift = (int)point;
                                                }

                                                point = 0f;
                                                temp = 0f;
                                                factor = 1;
                                                driftPanel?.Disappear();
                                                driftPanel = null;
                                            });
    }

    private void OnGamePaused()
    {
        if (!driftPanel) return;
        drifting = false;

        GameManager.Instance.currencyManager.Earn(new Price(0, (int)point));
        LevelManager.Instance.earnedDollar += (int)point;

        GameManager.Instance.dataManager.user.gameData.statistics.experience += (int)point;
        GameManager.Instance.dataManager.user.gameData.statistics.drift += (int)point;
        LevelManager.Instance.statistics.drift += (int)point;

        if (point > LevelManager.Instance.statistics.bestDrift)
        {
            LevelManager.Instance.statistics.bestDrift = (int)point;
            if (point > GameManager.Instance.dataManager.user.gameData.statistics.bestDrift)
                GameManager.Instance.dataManager.user.gameData.statistics.bestDrift = (int)point;
        }

        point = 0f;
        temp = 0f;
        factor = 1;
        Destroy(driftPanel.gameObject);
        driftPanel = null;
    }

    private void OnDestroy()
    {
        LevelManager.Instance?.onGamePaused?.RemoveListener(this.OnGamePaused);
    }
}