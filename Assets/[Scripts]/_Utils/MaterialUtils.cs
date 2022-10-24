using UnityEngine;

public static class ColorUtils
{
    public static string GetCode(this Color color)
    {
        return ColorUtility.ToHtmlStringRGBA(color);
    }

    public static Color GetColor(this string code)
    {
        ColorUtility.TryParseHtmlString("#" + code, out Color c);
        return c;
    }

    public static Color GetColor(this string code, float alpha)
    {
        ColorUtility.TryParseHtmlString("#" + code, out Color c);
        c.a = alpha;
        return c;
    }
}

public static class MaterialUtils
{
    public static Material Duplicate(this Material mat)
    {
        Material m = MonoBehaviour.Instantiate(mat);
        m.name += "-generated";
        return m;
    }

    public static void SetColor(this Renderer rend, Color color, int materialIndex = 0, string shaderName = null)
    {
        MaterialPropertyBlock mpb = new MaterialPropertyBlock();
        rend.GetPropertyBlock(mpb, materialIndex);
        mpb.SetColor(shaderName == null ? "_BaseColor" : shaderName, color);
        rend.SetPropertyBlock(mpb, materialIndex);
    }

    public static void SetFloat(this Renderer rend, string valueName, float value, int materialIndex = 0)
    {
        MaterialPropertyBlock mpb = new MaterialPropertyBlock();
        rend.GetPropertyBlock(mpb, materialIndex);
        mpb.SetFloat(valueName, value);
        rend.SetPropertyBlock(mpb, materialIndex);
    }

    public static void SetBool(this Renderer rend, int materialIndex, string variableName, bool active)
    {
        MaterialPropertyBlock mpb = new MaterialPropertyBlock();
        rend.GetPropertyBlock(mpb, materialIndex);
        mpb.SetFloat(variableName, active ? 1f : 0f);
        rend.SetPropertyBlock(mpb, materialIndex);
    }
}