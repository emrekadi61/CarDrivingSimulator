using UnityEngine;

public class EKPoliceSiren : MonoBehaviour
{
    public CarLightSystem.EKLightDataV2 redLight;
    public CarLightSystem.EKLightDataV2 blueLight;

    private void Start()
    {
        redLight.FindParts(gameObject);
        blueLight.FindParts(gameObject);
    }

    private void Update()
    {
        if (Mathf.Approximately((int)(Time.time) % 2, 0) && Mathf.Approximately((int)(Time.time * 20) % 3, 0))
        {
            redLight.SetIntensity(1f);
        }
        else
        {
            redLight.SetIntensity(0f);
            if (Mathf.Approximately((int)(Time.time * 20) % 3, 0)) blueLight.SetIntensity(1f);
            else blueLight.SetIntensity(0f);
        }
    }
}