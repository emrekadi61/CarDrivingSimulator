using System.Collections.Generic;
using UnityEngine;

public class CarPaintSystem : MonoBehaviour
{
    public PaintData bodyPaint;
    public PaintData bodyDetailPaint;
    public PaintData rimPaint;
    public PaintData windowPaint;

    private Car car;

    public CarPaintSystem Construct(Car car)
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
        windowPaint.SetColor(car.data.visual.window.color.GetColor(), car.data.visual.window.tint);
    }

    [System.Serializable]
    public class PaintData
    {
        public Color color;
        [Range(0f, 1f)] public float tint;
        public string materialName;
        public List<PaintPart> parts;

        public void SetColor()
        {
            parts.ForEach(p => p.rend?.SetColor(color, p.index, "_BaseColor"));
        }

        public void SetColor(Color? _color = null, float? _tint = null)
        {
            color = _color ?? color;
            tint = _tint ?? tint;

            parts.ForEach(p => p.rend?.SetFloat("_Smoothness", tint, p.index));
            parts.ForEach(p => p.rend?.SetColor(color, p.index, "_BaseColor"));
        }

        public void SetColor(bool isWindow, Color? _color = null, float? _tint = null)
        {
            color = _color ?? color;
            tint = _tint ?? tint;
            color.a = tint;
            parts.ForEach(p => p.rend?.SetColor(color, p.index, "_BaseColor"));
        }

        public void FindParts(GameObject go)
        {
            parts.Clear();
            parts = new List<PaintPart>();
            MeshRenderer[] renderers = go.GetComponentsInChildren<MeshRenderer>();

            for (int i = 0; i < renderers.Length; i++)
            {
                MeshRenderer r = renderers[i];

                for (int j = 0; j < r.sharedMaterials.Length; j++)
                {
                    if (r.sharedMaterials[j].name.ToLower().Contains(materialName))
                    {
                        PaintPart p = new PaintPart(j, r);
                        parts.Add(p);
                    }
                }
            }
        }
    }

    [System.Serializable]
    public class PaintPart
    {
        public int index;
        public Renderer rend;

        public PaintPart(int index, Renderer rend)
        {
            this.index = index;
            this.rend = rend;
        }
    }
}