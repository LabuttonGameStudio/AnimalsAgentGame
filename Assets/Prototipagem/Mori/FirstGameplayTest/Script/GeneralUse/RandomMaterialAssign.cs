using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomMaterialAssign : MonoBehaviour
{
    [SerializeField] private Mesh[] meshs;
    [SerializeField] private Material[] materials;
    [SerializeField] private bool basedOnParent;
    public void SetRandomColors()
    {
        MeshRenderer[] meshRenderers;
        float randomSeed = Mathf.Pow(Random.value + 1, Random.value + 1);
        meshRenderers = FindObjectsByType<MeshRenderer>(FindObjectsSortMode.None);
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
                float randomGenNumber = Mathf.Abs(Mathf.RoundToInt(meshRenderer.transform.parent.GetInstanceID() * randomSeed / 10f % 1 * 10));
                meshRenderer.sharedMaterial = materials[Mathf.RoundToInt(randomGenNumber / 1 / (materials.Length - 1))];
            }
            else
            {
                if (!doesMatchMesh) continue;
                float randomGenNumber = Mathf.Abs(Mathf.RoundToInt(meshRenderer.transform.GetInstanceID() * randomSeed / 10f % 1 * 10));
                Debug.Log(Mathf.RoundToInt(randomGenNumber / (1 / (materials.Length - 1))));
                meshRenderer.sharedMaterial = materials[Mathf.RoundToInt(randomGenNumber / 1 / (materials.Length - 1))];
            }
        }
    }

}
