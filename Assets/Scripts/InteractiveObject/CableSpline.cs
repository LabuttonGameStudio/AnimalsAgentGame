using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;

public class CableSpline : MonoBehaviour
{
    private static Material enabledMat;
    private static Material disabledMat;

    SplineContainer spline;
    MeshRenderer meshrenderer;
    private void Awake()
    {
        spline = GetComponent<SplineContainer>();
        meshrenderer = GetComponent<MeshRenderer>();

        if (disabledMat == null && enabledMat == null)
        {
            disabledMat = new Material(meshrenderer.material);
            enabledMat = new Material(meshrenderer.material);

            disabledMat.SetInt("_Emission", 0);
            enabledMat.SetInt("_Emission", 1);
        }

        meshrenderer.sharedMaterial = disabledMat;
    }

    public void ToggleSpline(bool toggle)
    {
        meshrenderer.sharedMaterial = toggle ? enabledMat : disabledMat;
    }
}
