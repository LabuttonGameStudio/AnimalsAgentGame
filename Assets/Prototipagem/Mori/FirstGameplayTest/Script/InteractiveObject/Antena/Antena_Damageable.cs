using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Antena_Damageable : MonoBehaviour, IDamageable
{
    [SerializeField] public bool charged;
    [SerializeField] public MeshRenderer[] meshRenderers;
    private Gerador_Interactive connectedGenerator;
    public void TakeDamage(int damageAmount)
    {
        if (!charged)
        {
            charged = true;
            meshRenderers[0].sharedMaterial.SetInt("_Light_on_off", 1);
            connectedGenerator.OnRequirementChange();
        }
    }

    void Awake()
    {
        Material material = new Material(meshRenderers[0].material);
        meshRenderers[0].sharedMaterial = material;
        meshRenderers[1].sharedMaterial = material;
    }
    public void DefineConnectedGenerator(Gerador_Interactive generator)
    {
        connectedGenerator = generator;
    }
}
