using UnityEngine;

public class CarCheckerJump : MonoBehaviour
{
    private Car car;
    private RCC_CarControllerV3 rcc;
    private JumpPointPanel jumpPanel;
    private float point;
    private float pointFactor = 10f;
    private bool isJumping;
    private bool respawning;

    public CarCheckerJump Construct(Car car)
    {
        GameManager.Instance.delayManager.Set(1f, () =>
        {
            this.car = car;
            this.rcc = car.rcc;
        });
        LevelManager.Instance?.onGamePaused?.AddListener(this.OnGamePaused);
        return this;
    }

    private void FixedUpdate()
    {
        if (!rcc) return;

        bool b = !rcc.FrontLeftWheelCollider.isGrounded && !rcc.FrontRightWheelCollider.isGrounded && !rcc.RearLeftWheelCollider.isGrounded && !rcc.RearRightWheelCollider.isGrounded;

        if (b) OnJumping();
        else OnJumpingEnd();
    }

    private void OnJumping()
    {
        isJumping = true;
        if (LevelManager.Instance.respawning) return;

        if (!jumpPanel) jumpPanel = JumpPointPanel.Get();

        point += Mathf.Abs(rcc.rigid.velocity.magnitude) * Time.fixedDeltaTime * pointFactor;

        jumpPanel.SetText(point);
    }

    private void OnJumpingEnd()
    {
        if (!isJumping) return; isJumping = false;
        if (LevelManager.Instance.respawning) LevelManager.Instance.respawning = false;
        LevelManager.Instance.earnedDollar += (int)point;
        GameManager.Instance.currencyManager.Earn(new Price(0, (int)point));

        LevelManager.Instance.statistics.experience += (int)point;
        GameManager.Instance.dataManager.user.gameData.statistics.experience += (int)point;

        point = 0f;
        jumpPanel?.Disappear();
        jumpPanel = null;
    }

    private void OnGamePaused()
    {
        if (!jumpPanel) return;
        if (!isJumping) return; isJumping = false;

        if (LevelManager.Instance.respawning) LevelManager.Instance.respawning = false;
        GameManager.Instance.currencyManager.Earn(new Price(0, (int)point));
        LevelManager.Instance.earnedDollar += (int)point;

        LevelManager.Instance.statistics.experience += (int)point;
        GameManager.Instance.dataManager.user.gameData.statistics.experience += (int)point;

        point = 0f;
        Destroy(jumpPanel.gameObject);
        jumpPanel = null;
    }

    private void OnDestroy()
    {
        LevelManager.Instance?.onGamePaused?.RemoveListener(this.OnGamePaused);
    }
}