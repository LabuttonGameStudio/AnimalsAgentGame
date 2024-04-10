using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomContainerGenerator : MonoBehaviour
{
    [SerializeField]
    private Mesh meshesOfContainer;
    [SerializeField] private Material containerMaterial;
    private Material[] containerMaterials;
    private void OnValidate()
    {
        if(containerMaterials != null)
        {
            containerMaterials = new Material[4];
            containerMaterials[0] = new Material(containerMaterial);
            containerMaterials[1] = new Material(containerMaterial);
            containerMaterials[2] = new Material(containerMaterial);
            containerMaterials[3] = new Material(containerMaterial);
        }
    }
    private void SetRandomColors()
    {
        MeshRenderer[] meshRenderers;
        meshRenderers = FindObjectsByType<MeshRenderer>(FindObjectsSortMode.None);
        
    }
}
