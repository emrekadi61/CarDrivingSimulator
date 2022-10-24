using UnityEngine;

public class CarPaintSystemV2 : MonoBehaviour
{
    public PaintDataV2 bodyPaint;
    public PaintDataV2 bodyDetailPaint;
    public PaintDataV2 rimPaint;
    public PaintDataV2 windowPaint;

    private Car car;

    public CarPaintSystemV2 Construct(Car car)
    {
        this.car = car;

        bodyPaint.FindParts(car.gameObject);
        bodyDetailPaint.FindParts(car.gameObject);
        windowPaint.FindParts(car.gameObject);

        return this;
    }

    public void Save()
    {
        car.data.visual.body.color = bodyPaint.color.GetCode();
        car.data.visual.body.tint = bodyPaint.tint;

        car.data.visual.bodyDetail.color = bodyDetailPaint.color.GetCode();
        car.data.visual.bodyDetail.tint = bodyDetailPaint.tint;

        car.data.visual.rim.color = rimPaint.color.GetCode();
        car.data.visual.rim.tint = rimPaint.tint;

        car.data.visual.window.color = windowPaint.color.GetCode();
        car.data.visual.window.tint = windowPaint.tint;
    }

    public void ReturnOriginal()
    {
        bodyPaint.SetColor(car.data.visual.body.color.GetColor(), car.data.visual.body.tint);
        bodyDetailPaint.SetColor(car.data.visual.bodyDetail.color.GetColor(), car.data.visual.bodyDetail.tint);
        rimPaint.SetColor(car.data.visual.rim.color.GetColor(), car.data.visual.rim.tint);
        windowPaint.SetColor(true, car.data.visual.window.color.GetColor(), car.data.visual.window.tint);
    }

    [System.Serializable]
    public class PaintDataV2
    {
        public Color color;
        [Range(0f, 1f)] public float tint;
        public string materialName;
        public Material material;

        public void SetColor(Color? _color = null, float? _tint = null)
        {
            color = _color ?? color;
            tint = _tint ?? tint;

            material?.SetFloat("_Smoothness", tint);
            material?.SetColor("_BaseColor", color);
        }

        public void SetColor(bool _isWindow, Color? _color = null, float? _tint = null)
        {
            color = _color ?? color;
            tint = _tint ?? tint;
            color.a = tint;

            material?.SetColor("_BaseColor", color);
        }

        public void FindParts(GameObject go)
        {
            material = null;
            MeshRenderer[] renderers = go.GetComponentsInChildren<MeshRenderer>();

            for (int i = 0; i < renderers.Length; i++)
            {
                MeshRenderer mr = renderers[i];
                Material[] mats = mr.sharedMaterials;

                for (int j = 0; j < mr.sharedMaterials.Length; j++)
                {
                    if (mr.sharedMaterials[j].name.ToLower().Contains(materialName))
                    {
                        if (material == null)
                        {
                            material = Instantiate(mr.sharedMaterials[j]);
                            material.name = mats[j].name.Split(' ')[0] + "-generated";
                        }
                        mats[j] = material;
                    }
                }
                mr.sharedMaterials = mats;
            }
        }
    }
}