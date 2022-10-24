using UnityEngine;

public class CarLightSystem : MonoBehaviour
{
    #region V1
    // public EKLightData headlightLowBeam;
    // public EKLightData headlightHighBeam;
    // public EKLightData stopLight;
    // public EKLightData brakeLight;
    // public EKLightData reverseLight;
    // public EKLightData indicatorLightLeft;
    // public EKLightData indicatorLightRight;
    // public EKLightData fogLight;
    // public EKLightData dayTimeLight;
    #endregion

    public EKLightDataV2 headlightLowBeam;
    public EKLightDataV2 headlightHighBeam;
    public EKLightDataV2 stopLight;
    public EKLightDataV2 brakeLight;
    public EKLightDataV2 reverseLight;
    public EKLightDataV2 indicatorLightLeft;
    public EKLightDataV2 indicatorLightRight;
    public EKLightDataV2 fogLight;
    public EKLightDataV2 dayTimeLight;

    private Car car;

    public CarLightSystem Construct(Car car)
    {
        this.car = car;

        headlightLowBeam.FindParts(car.gameObject);
        headlightHighBeam.FindParts(car.gameObject);
        stopLight.FindParts(car.gameObject);
        brakeLight.FindParts(car.gameObject);
        reverseLight.FindParts(car.gameObject);
        indicatorLightLeft.FindParts(car.gameObject);
        indicatorLightRight.FindParts(car.gameObject);
        fogLight.FindParts(car.gameObject);
        dayTimeLight.FindParts(car.gameObject);

        #region V1
        // FindMaterials();            
        #endregion

        RCC_InputManager.OnLowBeamHeadlights += OnLowBeamHeadlights;
        RCC_InputManager.OnHighBeamHeadlights += OnHighBeamHeadlights;
        RCC_InputManager.OnIndicatorLeft += OnIndicatorLeft;
        RCC_InputManager.OnIndicatorRight += OnIndicatorRight;
        RCC_InputManager.OnIndicatorHazard += OnIndicatorHazard;

        dayTimeLight.SetIntensity(1f);
        if (car.rcc.lowBeamHeadLightsOn)
        {
            headlightLowBeam.SetIntensity(1f);
            stopLight.SetIntensity(1f);
        }

        return this;
    }

    private void Update()
    {
        Indicators();
        brakeLight.SetIntensity(car.rcc.brakeInput);
        reverseLight.SetIntensity(car.rcc.direction == -1 ? 1f : 0f);
    }

    public void SetHazards()
    {
        OnIndicatorHazard();
    }

    private void OnLowBeamHeadlights()
    {
        headlightLowBeam.SetIntensity(headlightLowBeam.intensity >= 1f ? 0f : 1f);
        stopLight.SetIntensity(stopLight.intensity >= 1f ? 0f : 1f);
    }

    private void OnHighBeamHeadlights()
    {
        headlightHighBeam.SetIntensity(headlightHighBeam.intensity >= 1f ? 0f : 1f);
    }

    private IndicatorStatus indicators = IndicatorStatus.Off;
    private float indicatorTimer = 0f;
    private bool indicatorsOn;
    private void Indicators()
    {
        if (indicators == IndicatorStatus.Off) return;

        indicatorTimer += Time.deltaTime;

        if (indicatorTimer >= 0.5f)
        {
            indicatorTimer = 0f;
            indicatorsOn = !indicatorsOn;
            switch (indicators)
            {
                case IndicatorStatus.Left:
                    indicatorLightLeft.SetIntensity(indicatorsOn ? 1f : 0f);
                    indicatorLightRight.SetIntensity(0f);
                    break;
                case IndicatorStatus.Right:
                    indicatorLightRight.SetIntensity(indicatorsOn ? 1f : 0f);
                    indicatorLightLeft.SetIntensity(0);
                    break;
                case IndicatorStatus.All:
                    indicatorLightLeft.SetIntensity(indicatorsOn ? 1f : 0f);
                    indicatorLightRight.SetIntensity(indicatorsOn ? 1f : 0f);
                    break;
            }
        }
    }

    private void OnIndicatorLeft()
    {
        if (indicators != IndicatorStatus.Left)
        {
            indicatorTimer = 0.5f;
            indicators = IndicatorStatus.Left;

        }
        else
        {
            indicators = IndicatorStatus.Off;
            indicatorLightLeft.SetIntensity(0f);
            indicatorLightRight.SetIntensity(0f);
        }
    }

    private void OnIndicatorRight()
    {
        if (indicators != IndicatorStatus.Right)
        {
            indicatorTimer = 0.5f;
            indicators = IndicatorStatus.Right;

        }
        else
        {
            indicators = IndicatorStatus.Off;
            indicatorLightLeft.SetIntensity(0f);
            indicatorLightRight.SetIntensity(0f);
        }
    }

    private void OnIndicatorHazard()
    {
        if (indicators != IndicatorStatus.All)
        {
            indicatorTimer = 0.5f;
            indicators = IndicatorStatus.All;
        }
        else
        {
            indicators = IndicatorStatus.Off;
            indicatorLightLeft.SetIntensity(0f);
            indicatorLightRight.SetIntensity(0f);
        }
    }

    private void OnDestroy()
    {
        RCC_InputManager.OnLowBeamHeadlights -= OnLowBeamHeadlights;
        RCC_InputManager.OnHighBeamHeadlights -= OnHighBeamHeadlights;
        RCC_InputManager.OnIndicatorLeft -= OnIndicatorLeft;
        RCC_InputManager.OnIndicatorRight -= OnIndicatorRight;
        RCC_InputManager.OnIndicatorHazard -= OnIndicatorHazard;
    }

    #region V1
    /*
    private void FindMaterials()
    {
        Renderer[] renderers = GetComponentsInChildren<Renderer>();

        for (int i = 0; i < renderers.Length; i++)
        {
            Renderer r = renderers[i];

            for (int j = 0; j < r.sharedMaterials.Length; j++)
            {
                Check(r, j, headlightLowBeam.materialName, headlightLowBeam.lightPower, headlightLowBeam.lights);
                Check(r, j, headlightHighBeam.materialName, headlightHighBeam.lightPower, headlightHighBeam.lights);
                Check(r, j, stopLight.materialName, stopLight.lightPower, stopLight.lights);
                Check(r, j, brakeLight.materialName, brakeLight.lightPower, brakeLight.lights);
                Check(r, j, reverseLight.materialName, reverseLight.lightPower, reverseLight.lights);
                Check(r, j, indicatorLightLeft.materialName, indicatorLightLeft.lightPower, indicatorLightLeft.lights);
                Check(r, j, indicatorLightRight.materialName, indicatorLightRight.lightPower, indicatorLightRight.lights);
                Check(r, j, fogLight.materialName, fogLight.lightPower, fogLight.lights);
                Check(r, j, dayTimeLight.materialName, dayTimeLight.lightPower, dayTimeLight.lights);
            }
        }
    }
    */

    /*
    private void Check(Renderer rend, int materialIndex, string materialName, float power, List<EKLight> list)
    {
        if (rend.sharedMaterials[materialIndex].name.ToLower().Contains(materialName))
        {
            EKLight l = new EKLight();
            l.renderer = rend;
            l.index = materialIndex;
            l.normalColor = rend.sharedMaterials[materialIndex].GetColor("_BaseColor") * Mathf.Pow(2, -10f);
            l.enableColor = rend.sharedMaterials[materialIndex].GetColor("_BaseColor") * Mathf.Pow(2, power);
            list.Add(l);

            rend.SetColor(l.normalColor, materialIndex, "_EmissionColor");
        }
    }
    */

    /*
    [System.Serializable]
    public class EKLightData
    {
        public string materialName;
        public float lightPower;
        [Range(0f, 1f)] public float intensity = 0f;
        public List<EKLight> lights = new List<EKLight>();

        public void SetIntensity()
        {
            lights.ForEach(c =>
            {
                Color color = Color.Lerp(lights.First().normalColor, lights.First().enableColor, intensity);
                c.renderer.SetColor(color, c.index, "_EmissionColor");
            });
        }
    */

    /*
        public void SetIntensity(float _intensity)
        {
            if (intensity == _intensity) return;

            intensity = _intensity;
            lights.ForEach(c =>
            {
                Color color = Color.Lerp(lights.First().normalColor, lights.First().enableColor, intensity);
                c.renderer.SetColor(color, c.index, "_EmissionColor");
            });
        }
    }
    */

    /*
    [System.Serializable]
    public class EKLight
    {
        public Renderer renderer;
        public int index;
        [ColorUsage(false, true)] public Color normalColor;
        [ColorUsage(false, true)] public Color enableColor;
    }
    */
    #endregion

    [System.Serializable]
    public class EKLightDataV2
    {
        public string materialName;
        public float lightPower;
        [Range(0f, 1f)] public float intensity = 0f;
        public EKlightV2 light;

        public void SetIntensity()
        {
            if (light == null) return;
            light?.material?.SetColor("_EmissionColor", Color.Lerp(light.normalColor, light.enableColor, intensity));
        }

        public void SetIntensity(float _intensity)
        {
            if (light == null || intensity == _intensity) return;
            intensity = _intensity;
            light?.material?.SetColor("_EmissionColor", Color.Lerp(light.normalColor, light.enableColor, intensity));
        }

        public void FindParts(GameObject go)
        {
            light = null;
            MeshRenderer[] renderers = go.GetComponentsInChildren<MeshRenderer>();

            for (int i = 0; i < renderers.Length; i++)
            {
                MeshRenderer mr = renderers[i];
                Material[] mats = mr.sharedMaterials;

                for (int j = 0; j < mr.sharedMaterials.Length; j++)
                {
                    if (mr.sharedMaterials[j].name.ToLower().Contains(materialName))
                    {
                        if (light == null)
                        {
                            light = new EKlightV2();
                            light.material = Instantiate(mr.sharedMaterials[j]);
                            light.material.name = mats[j].name.Split(' ')[0] + "-generated-(light)";

                            light.normalColor = light.material.GetColor("_BaseColor") * Mathf.Pow(2, -10f);
                            light.enableColor = light.material.GetColor("_BaseColor") * Mathf.Pow(2, lightPower);
                        }
                        mats[j] = light.material;
                    }
                    mr.sharedMaterials = mats;
                }
            }
            SetIntensity();
        }
    }

    [System.Serializable]
    public class EKlightV2
    {
        public Material material;
        [ColorUsage(false, true)] public Color normalColor;
        [ColorUsage(false, true)] public Color enableColor;
    }

    [System.Serializable]
    public enum IndicatorStatus : int
    {
        Off = 0,
        Left = 1,
        Right = 2,
        All = 3
    }
}