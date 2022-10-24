using UnityEngine;

public class MiniMapCamera : MonoBehaviour
{
    public float currentHeight = 250f;
    public float targetHeight = 250f;

    private Transform target;
    private RCC_CarControllerV3 rcc;
    private int miniMapMode;

    public MiniMapCamera Construct(RCC_CarControllerV3 rcc, int miniMapMode = 1)
    {
        this.rcc = rcc;
        this.target = this.rcc.transform;
        this.miniMapMode = miniMapMode;

        return this;
    }

    private void FixedUpdate()
    {
        if (target == null)
        {
            target = LevelManager.Instance.car.transform;
            return;
        }
        if (miniMapMode == 0) FollowOnTop();
        else if (miniMapMode == 1) Follow3D();
    }

    private void FollowOnTop()
    {
        if (rcc.speed > 100f) targetHeight = 450f;
        else if (rcc.speed > 50f) targetHeight = 350f;
        else targetHeight = 250f;

        currentHeight = Mathf.Lerp(currentHeight, targetHeight, Time.fixedDeltaTime * 10f);

        transform.position = target.position + Vector3.up * currentHeight;
        Quaternion q = target.rotation;
        q.x = 0f; q.z = 0f;
        transform.rotation = q * Quaternion.Euler(90f, 0, 0);
    }

    private Vector3 offset = new Vector3(0f, 0.75f, -1f);
    private float distanceTarget = 50f;
    private float currentDistance = 50f;
    private void Follow3D()
    {
        if (rcc.speed > 100f) distanceTarget = 250f;
        else if (rcc.speed > 50f) distanceTarget = 175f;
        else distanceTarget = 125f;

        currentDistance = Mathf.Lerp(currentDistance, distanceTarget, Time.fixedDeltaTime * 10f);

        transform.position = target.position + target.TransformVector(offset) * currentDistance;
        transform.rotation = Quaternion.LookRotation((target.position + Vector3.down * 10f) - transform.position);
    }
}