using UnityEngine;

public class CameraTiltController : MonoBehaviour
{
    private RCC_Camera rccCamera;
    private RCC_CarControllerV3 rcc;

    public CameraTiltController Construct(RCC_Camera rccCamera, RCC_CarControllerV3 rcc)
    {
        this.rccCamera = rccCamera;
        this.rcc = rcc;
        return this;
    }

    private float targetAngle;
    private void LateUpdate()
    {
        if (!rccCamera || !rcc) return;

        targetAngle = rcc.steerInput * -45f;
        rccCamera.TPSYaw = Mathf.Lerp(rccCamera.TPSYaw, targetAngle, Time.smoothDeltaTime * 3f);
    }

    private void OnDisable()
    {
        if (rccCamera) rccCamera.TPSYaw = 0f;
    }
}