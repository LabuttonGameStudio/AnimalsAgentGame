using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomContainerGenerator : MonoBehaviour
{
    [SerializeField]
    private Mesh[] meshesOfContainer;
    [SerializeField] private Material[] containerMaterials;
    public void SetRandomColors()
    {
        MeshRenderer[] meshRenderers;
        float randomSeed = Mathf.Pow(Random.value+1, Random.value + 1);
        meshRenderers = FindObjectsByType<MeshRenderer>(FindObjectsSortMode.None);
        foreach (MeshRenderer meshRenderer in meshRenderers)
        {
            Mesh mesh = meshRenderer.GetComponent<MeshFilter>().sharedMesh;
            bool doesMatchMesh = false;
            foreach (Mesh containerMesh in meshesOfContainer)
            {
                if (containerMesh == mesh)
                {
                    doesMatchMesh = true;
                    break;
                }
            }
            if (!doesMatchMesh || meshRenderer.transform.parent == null) continue;
            float randomGenNumber = Mathf.Abs(Mathf.RoundToInt(meshRenderer.transform.parent.GetInstanceID() * randomSeed / 10f % 1 * 10));
            meshRenderer.sharedMaterial = containerMaterials[Mathf.RoundToInt(randomGenNumber /1/(containerMaterials.Length-1))];
        }
    }

}
