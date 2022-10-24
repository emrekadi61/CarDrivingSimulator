using UnityEngine;
using Dreamteck.Splines;

public class Ramp : MonoBehaviour
{
    private SplineComputer sc;

    private void Awake()
    {
        sc = GetComponentInChildren<SplineComputer>();
        if (sc == null) return;
        sc.updateMode = SplineComputer.UpdateMode.None;
        sc.RebuildImmediate();
    }
}