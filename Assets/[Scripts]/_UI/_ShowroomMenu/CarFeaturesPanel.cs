using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class CarFeaturesPanel : MonoBehaviour
{
    public float maxEngineTorque = 1500;
    public float maxHandling = 1f;
    public float maxBrake = 10000;

    public RectTransform speedFillRect;
    public RectTransform handlingFillRect;
    public RectTransform brakesFillRect;
    public RectTransform horsePowerTextRect;
    public RectTransform massTextRect;
    public RectTransform drivingWheelTextRect;

    #region Components
    private Image speedFill;
    private Image handlingFill;
    private Image brakesFill;
    private Text horsePowerText;
    private Text massText;
    private Text drivingWheelText;
    #endregion

    public CarFeaturesPanel Construct(Car car)
    {
        speedFill = speedFillRect.GetComponent<Image>();
        handlingFill = handlingFillRect.GetComponent<Image>();
        brakesFill = brakesFillRect.GetComponent<Image>();

        horsePowerText = horsePowerTextRect.GetComponent<Text>();
        massText = massTextRect.GetComponent<Text>();
        drivingWheelText = drivingWheelTextRect.GetComponent<Text>();

        ShowroomManager.Instance.onCarChanged.AddListener(OnCarChanged);

        return this;
    }

    private void OnCarChanged(Car car)
    {
        speedFill.DOComplete();
        handlingFill.DOComplete();
        brakesFill.DOComplete();

        speedFill.fillAmount = handlingFill.fillAmount = brakesFill.fillAmount = 0f;
        float duration = 0.5f;
        speedFill.DOFillAmount(car.rcc.maxEngineTorque / maxEngineTorque, duration);
        handlingFill.DOFillAmount(car.rcc.steerHelperLinearVelStrength / maxHandling, duration);
        brakesFill.DOFillAmount(car.rcc.brakeTorque / maxBrake, duration);

        horsePowerText.text = ((int)((float)car.rcc.maxEngineTorque / 2f)) + " HP";
        massText.text = car.GetComponent<Rigidbody>().mass + " KG";
        drivingWheelText.text = car.rcc.wheelTypeChoise.ToString().ToUpper();

        car.onUpgraded.AddListener(OnUpgraded);
    }

    private void OnUpgraded()
    {
        speedFill.DOComplete();
        handlingFill.DOComplete();
        brakesFill.DOComplete();

        Car car = ShowroomManager.Instance.current;
        
        float duration = 0.5f;
        speedFill.DOFillAmount(car.rcc.maxEngineTorque / maxEngineTorque, duration);
        handlingFill.DOFillAmount(car.rcc.steerHelperLinearVelStrength / maxHandling, duration);
        brakesFill.DOFillAmount(car.rcc.brakeTorque / maxBrake, duration);

        horsePowerText.text = ((int)((float)car.rcc.maxEngineTorque / 2f)) + " HP";
        massText.text = car.GetComponent<Rigidbody>().mass + " KG";
        drivingWheelText.text = car.rcc.wheelTypeChoise.ToString().ToUpper();
    }
}