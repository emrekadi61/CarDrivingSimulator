using UnityEngine;
using UnityEngine.UI;

public class EKDashboard : MonoBehaviour
{
    public RectTransform speedTextRect;
    public RectTransform gearTextRect;
    public RectTransform rpmRect;
    public RectTransform speedRect;

    private Text speedText;
    private Text gearText;
    private Image rpm;
    private Image speed;

    private RCC_CarControllerV3 rcc;
    private float maxRPM;
    private float maxSpeed;

    public EKDashboard Construct(RCC_CarControllerV3 rcc)
    {
        this.rcc = rcc;

        speedText = speedTextRect.GetComponent<Text>();
        gearText = gearTextRect.GetComponent<Text>();
        rpm = rpmRect.GetComponent<Image>();
        speed = speedRect.GetComponent<Image>();

        SetSpeedText(0f);
        rpm.fillAmount = speed.fillAmount = 0f;
        maxRPM = rcc.maxEngineRPM;
        maxSpeed = rcc.maxspeed;

        return this;
    }

    private void Update()
    {
        if (!rcc) return;

        SetSpeedText(rcc.speed);
        SetGearText(rcc.currentGear);

        rpm.fillAmount = rcc.engineRPM / maxRPM;
        speed.fillAmount = rcc.speed / maxSpeed;
    }

    private void SetSpeedText(float speed)
    {
        int kmhSize = (int)(speedText.fontSize * 0.5f);
        speedText.text = ((int)speed).ToString() + " <size=" + ((int)kmhSize) + ">KM/H</size>";
    }

    private void SetGearText(int gear)
    {
        if (rcc.direction == -1) gearText.text = "R";
        else gearText.text = "D" + (gear + 1);
    }
}