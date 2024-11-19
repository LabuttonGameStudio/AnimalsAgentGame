using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriangleCode : CodeBase
{
    public static Material turnOnMat;
    public static Material turnOffMat;

    private MeshRenderer meshRenderer;

    private void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        if(turnOffMat == null )
        {
            turnOffMat = new Material(meshRenderer.material);
            turnOffMat.SetInt("_Emissive", 0);
        }
        if (turnOnMat == null)
        {
            turnOnMat = new Material(meshRenderer.material);
            turnOnMat.SetInt("_Emissive", 1);
        }
        meshRenderer.sharedMaterial = turnOnMat;
    }
    public override void ToggleLights(bool toggle)
    {
        meshRenderer.sharedMaterial = toggle ? turnOnMat : turnOffMat;
    }
}
