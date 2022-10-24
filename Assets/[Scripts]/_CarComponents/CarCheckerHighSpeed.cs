using UnityEngine;

public class CarCheckerHighSpeed : MonoBehaviour
{
    private Car car;
    private RCC_CarControllerV3 rcc;
    private HighSpeedPointPanel speedPanel;
    private DelayHandler delayHandler;
    private bool isHighSpeed;
    private float speedTreshold = 100f;
    private float point = 0f;
    private float pointFactor = 25f;

    public CarCheckerHighSpeed Construct(Car car)
    {
        this.car = car;
        this.rcc = car.rcc;
        LevelManager.Instance?.onGamePaused?.AddListener(this.OnGamePaused);
        return this;
    }

    private void FixedUpdate()
    {
        if (!rcc) return;

        LevelManager.Instance.statistics.trip += ((Time.fixedDeltaTime * rcc.speed) / 1000f);

        GameManager.Instance.dataManager.user.gameData.statistics.trip += ((Time.fixedDeltaTime * rcc.speed) / 1000f);

        if (rcc.speed > speedTreshold) HighSpeed();
        else HighSpeedEnd();
    }

    private void HighSpeed()
    {
        isHighSpeed = true;
        if (!speedPanel) speedPanel = HighSpeedPointPanel.Get();

        float fac = Mathf.RoundToInt((rcc.speed % speedTreshold) / 25f);
        point += Time.fixedDeltaTime * pointFactor * Mathf.Clamp(fac, 1, fac) * car.data.pointFactor;

        LevelManager.Instance.statistics.highSpeedTrip += Time.fixedDeltaTime * rcc.speed;

        GameManager.Instance.dataManager.user.gameData.statistics.highSpeedTrip += ((Time.fixedDeltaTime * rcc.speed) / 1000f);

        speedPanel.SetPointText(point);
    }

    private void HighSpeedEnd()
    {
        if (!isHighSpeed) return; isHighSpeed = false;
        LevelManager.Instance.earnedDollar += (int)point;
        GameManager.Instance.currencyManager.Earn(new Price(0, (int)point));

        GameManager.Instance.dataManager.user.gameData.statistics.experience += (int)point;
        LevelManager.Instance.statistics.experience += (int)point;

        point = 0f;
        speedPanel?.Disappear();
        speedPanel = null;
    }

    private void OnGamePaused()
    {
        if (!speedPanel) return;

        if (!isHighSpeed) return; isHighSpeed = false;
        GameManager.Instance.currencyManager.Earn(new Price(0, (int)point));
        LevelManager.Instance.earnedDollar += (int)point;

        GameManager.Instance.dataManager.user.gameData.statistics.experience += (int)point;
        LevelManager.Instance.statistics.experience += (int)point;

        point = 0f;
        Destroy(speedPanel.gameObject);
        speedPanel = null;
    }

    private void OnDestroy()
    {
        LevelManager.Instance?.onGamePaused?.RemoveListener(this.OnGamePaused);
    }
}