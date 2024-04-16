using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class RandomMaterialAssign : MonoBehaviour
{
    [SerializeField] private Mesh[] meshs;
    [SerializeField] private Material[] materials;
    [SerializeField] private bool basedOnParent;
    public void SetRandomColors()
    {
        MeshRenderer[] meshRenderers;
        Random.InitState((int)DateTime.Now.Ticks);
        int randomSeed = Mathf.RoundToInt((Random.value*10 + 1)* (Random.value * 10 + 1));
        meshRenderers = FindObjectsByType<MeshRenderer>(FindObjectsSortMode.None);
        Debug.Log("Found "+meshRenderers.Length + " matching meshes");
        foreach (MeshRenderer meshRenderer in meshRenderers)
        {
            Mesh mesh = meshRenderer.GetComponent<MeshFilter>().sharedMesh;
            bool doesMatchMesh = false;
            foreach (Mesh containerMesh in meshs)
            {
                if (containerMesh == mesh)
                {
                    doesMatchMesh = true;
                    break;
                }
            }
            if (basedOnParent)
            {
                if (!doesMatchMesh || meshRenderer.transform.parent == null) continue;
                Random.InitState(meshRenderer.transform.parent.GetInstanceID()* randomSeed);
                int randomGenNumber = Random.Range(0, materials.Length);
                meshRenderer.sharedMaterial = materials[randomGenNumber];
            }
            else
            {
                if (!doesMatchMesh) continue;
                Random.InitState(meshRenderer.transform.GetInstanceID() * randomSeed);
                int randomGenNumber = Random.Range(0, materials.Length);
                meshRenderer.sharedMaterial = materials[randomGenNumber];
            }
        }
    }

}
