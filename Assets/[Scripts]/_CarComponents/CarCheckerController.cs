using UnityEngine;

public class CarCheckerController : MonoBehaviour
{
    [HideInInspector] public CarCheckerDrift driftChecker;
    [HideInInspector] public CarCheckerHighSpeed highSpeedChecker;
    [HideInInspector] public CarCheckerJump jumpChecker;
    [HideInInspector] public CarCheckerCollision collisionChecker;

    private Car car;

    public CarCheckerController Construct(Car car)
    {
        this.car = car;
        driftChecker = gameObject.AddComponent<CarCheckerDrift>().Construct(this.car);
        highSpeedChecker = gameObject.AddComponent<CarCheckerHighSpeed>().Construct(this.car);
        jumpChecker = gameObject.AddComponent<CarCheckerJump>().Construct(this.car);
        collisionChecker = gameObject.AddComponent<CarCheckerCollision>().Construct(this.car.rcc);

        return this;
    }
}