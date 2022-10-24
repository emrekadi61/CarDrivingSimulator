using System.Linq;
using System.Collections;
using UnityEngine;

public class CarShowroomMode : MonoBehaviour
{
    [HideInInspector] public Car car;
    [HideInInspector] public bool steer = true;

    public CarShowroomMode Construct(Car car)
    {
        this.car = car;
        car.rcc.engineRunning = false;
        car.lightSystem.enabled = false;
        car.lightSystem.headlightLowBeam.SetIntensity(0.5f);
        car.lightSystem.stopLight.SetIntensity(0.5f);
        car.lightSystem.brakeLight.SetIntensity(1f);
        car.lightSystem.fogLight.SetIntensity(0.75f);
        car.lightSystem.indicatorLightLeft.SetIntensity(0f);
        car.lightSystem.indicatorLightRight.SetIntensity(0f);

        GetComponentsInChildren<AudioSource>().ToList().ForEach(c => c.volume = 0f);

        return this;
    }

    private void LateUpdate()
    {
        if (!car || !steer) return;

        car.rcc.steerInput = 1f;
    }

    public void Run()
    {
        car.rcc.StartEngine();
        ShowroomCameraManager.Instance.Focus("run", GameManager.Instance.statics.cameraFocusDuration, () =>
        {
            StartCoroutine(RunCor());
        });
    }

    private IEnumerator RunCor()
    {
        car.lightSystem.headlightHighBeam.SetIntensity(1f);
        yield return new WaitForSeconds(0.1f);
        car.lightSystem.headlightHighBeam.SetIntensity(0f);
        yield return new WaitForSeconds(0.05f);
        car.lightSystem.headlightHighBeam.SetIntensity(1f);
        yield return new WaitForSeconds(0.1f);
        car.lightSystem.headlightHighBeam.SetIntensity(0f);
        SceneLoader.Get("Game");
    }
}