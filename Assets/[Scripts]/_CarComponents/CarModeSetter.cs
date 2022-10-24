using UnityEngine;
using UnityEngine.Events;

public class CarModeSetter : MonoBehaviour
{
    private Car car;
    public CarDriveMode mode = CarDriveMode.Sim;

    public OnModeChanged onModeChanged = new OnModeChanged();

    public CarModeSetter Construct(Car car)
    {
        this.car = car;
        RCC.SetBehavior(5);
        SetMode(mode);
        onModeChanged.AddListener(SetMode);
        return this;
    }

    public void SetMode(CarDriveMode mode)
    {
        this.mode = mode;
        switch (this.mode)
        {
            case CarDriveMode.Sim: SimMode(); break;
            case CarDriveMode.Drift: DriftMode(); break;
        }
    }

    private void OnDestroy()
    {
        onModeChanged?.RemoveAllListeners();
    }

    private void SimMode()
    {
        RCC_Settings.BehaviorType b = RCC_Settings.Instance.selectedBehaviorType;
        b.counterSteering = false;
        b.limitSteering = true;
        b.ABS = true;
        b.ESP = true;
        b.TCS = true;

        b.applyExternalWheelFrictions = false;
        b.applyRelativeTorque = false;

        b.highSpeedSteerAngleMinimum = 0f;
        b.highSpeedSteerAngleMaximum = 60f;
        b.highSpeedSteerAngleAtspeedMinimum = 0f;
        b.highSpeedSteerAngleAtspeedMaximum = 350f;

        b.counterSteeringMinimum = 0.1f;
        b.counterSteeringMaximum = 1f;

        b.steeringSensitivityMinimum = 0.1f;
        b.steeringSensitivityMaximum = 1f;

        b.steerHelperAngularVelStrengthMinimum = 0.1f;
        b.steerHelperAngularVelStrengthMaximum = 0.1f;
        b.steerHelperLinearVelStrengthMinimum = 0.1f;
        b.steerHelperLinearVelStrengthMaximum = 0.1f;
        b.tractionHelperStrengthMinimum = 0.1f;
        b.tractionHelperStrengthMaximum = 0.1f;

        b.antiRollFrontHorizontalMinimum = 1000f;
        b.antiRollRearHorizontalMinimum = 1000f;

        b.gearShiftingDelayMaximum = car.data.specifications.transmission - (car.data.upgrades.transmission * car.data.upgradeIncrements.transmission);

        b.angularDrag = 0.1f;
        b.angularDragHelperMinimum = 0.1f;
        b.angularDragHelperMaximum = 0.5f;

        b.forwardExtremumSlip = 0.4f;
        b.forwardExtremumValue = 1f;
        b.forwardAsymptoteSlip = 0.8f;
        b.forwardAsymptoteValue = 0.75f;

        b.sidewaysExtremumSlip = 0.2f;
        b.sidewaysExtremumValue = 1f;
        b.sidewaysAsymptoteSlip = 0.5f;
        b.sidewaysAsymptoteValue = 0.75f;

        car.rcc.maxEngineTorque = car.data.specifications.engineTorque;

        RCC_Settings.Instance.useAutomaticClutch = false;
        car.rcc.steerHelperAngularVelStrength = car.data.specifications.steering + (car.data.upgrades.steering * car.data.upgradeIncrements.steering);
        car.rcc.steerHelperLinearVelStrength = car.data.specifications.steering + (car.data.upgrades.steering * car.data.upgradeIncrements.steering);
    }

    private void DriftMode()
    {
        RCC_Settings.BehaviorType b = RCC_Settings.Instance.selectedBehaviorType;
        b.counterSteering = true;
        b.limitSteering = false;
        b.ABS = false;
        b.ESP = false;
        b.TCS = false;

        b.applyExternalWheelFrictions = true;
        b.applyRelativeTorque = true;

        b.highSpeedSteerAngleMinimum = 5f;
        b.highSpeedSteerAngleMaximum = 5f;
        b.highSpeedSteerAngleAtspeedMinimum = 120f;
        b.highSpeedSteerAngleAtspeedMaximum = 120f;

        b.counterSteeringMinimum = 0.5f;
        b.counterSteeringMaximum = 0.5f;

        b.steeringSensitivityMinimum = 1f;
        b.steeringSensitivityMaximum = 1f;

        b.steerHelperAngularVelStrengthMinimum = 0.05f;
        b.steerHelperAngularVelStrengthMaximum = 0.05f;
        b.steerHelperLinearVelStrengthMinimum = 0f;
        b.steerHelperLinearVelStrengthMaximum = 0f;
        b.tractionHelperStrengthMinimum = 0.2f;
        b.tractionHelperStrengthMaximum = 0.2f;

        b.antiRollFrontHorizontalMinimum = 0f;
        b.antiRollRearHorizontalMinimum = 0f;

        b.gearShiftingDelayMaximum = car.data.specifications.transmission - (car.data.upgrades.transmission * car.data.upgradeIncrements.transmission);

        b.angularDrag = 0.5f;
        b.angularDragHelperMinimum = 0.1f;
        b.angularDragHelperMaximum = 0.1f;

        b.forwardExtremumSlip = 0.45f;
        b.forwardExtremumValue = 1f;
        b.forwardAsymptoteSlip = 1f;
        b.forwardAsymptoteValue = 1f;

        b.sidewaysExtremumSlip = 0.45f;
        b.sidewaysExtremumValue = 1f;
        b.sidewaysAsymptoteSlip = 0.5f;
        b.sidewaysAsymptoteValue = 0.5f;

        car.rcc.maxEngineTorque = car.data.specifications.engineTorque * 1.25f;

        RCC_Settings.Instance.useAutomaticClutch = true;

        car.rcc.steerHelperAngularVelStrength = 0.4f;
        car.rcc.steerHelperLinearVelStrength = 0.4f;
    }

    [System.Serializable]
    public enum CarDriveMode : int
    {
        Sim = 0,
        Drift = 1
    }

    public class OnModeChanged : UnityEvent<CarDriveMode> { }
}