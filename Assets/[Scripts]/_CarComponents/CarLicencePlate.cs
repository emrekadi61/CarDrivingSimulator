using UnityEngine;
using TMPro;

public class CarLicencePlate : MonoBehaviour
{
    public TextMeshPro tmp;
    public int materialIndex = 1;

    public void SetPlate(string plate, Color plateColor, Color plateTextColor)
    {
        tmp.text = plate;
        tmp.color = plateTextColor;
        MeshRenderer r = GetComponentInChildren<MeshRenderer>();
        r.SetColor(plateColor, materialIndex);
    }
}