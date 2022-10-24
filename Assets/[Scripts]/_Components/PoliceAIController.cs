using UnityEngine;

public class PoliceAIController : MonoBehaviour
{
    public RCC_AICarController policeCar;
    public PoliceMode currentMode;

    private RCC_CarControllerV3 targetCar;
    private float distance = 0f;

    private void Start()
    {
        targetCar = LevelManager.Instance.car.rcc;
        policeCar.gameObject.AddComponent<CarCheckerCollision>().Construct(targetCar);
        ChangeMode(GameManager.Instance.statics.policeChase ? PoliceMode.Follow : PoliceMode.Patrol);
    }

    public void ChangeMode(PoliceMode mode)
    {
        currentMode = mode;

        switch (currentMode)
        {
            case PoliceMode.Patrol:
                policeCar.navigationMode = RCC_AICarController.NavigationMode.FollowWaypoints;
                policeCar.maximumSpeed = 30f;
                break;
            case PoliceMode.Chase:
                policeCar.navigationMode = RCC_AICarController.NavigationMode.ChaseTarget;
                policeCar.maximumSpeed = 200f;
                break;
            case PoliceMode.Follow:
                policeCar.navigationMode = RCC_AICarController.NavigationMode.FollowTarget;
                policeCar.maximumSpeed = 200f;
                break;
        }
    }

    private void Update()
    {
        if (policeCar == null || targetCar == null || currentMode == PoliceMode.Patrol) return;

        distance = (policeCar.transform.position - targetCar.transform.position).magnitude;

        if (currentMode == PoliceMode.Follow && distance < policeCar.stopFollowDistance)
        {
            ChangeMode(PoliceMode.Chase);
        }
        else if (currentMode == PoliceMode.Chase && distance > policeCar.stopFollowDistance)
        {
            ChangeMode(PoliceMode.Follow);
        }
    }

    public enum PoliceMode : int
    {
        Patrol = 0,
        Chase = 1,
        Follow = 2
    }
}
