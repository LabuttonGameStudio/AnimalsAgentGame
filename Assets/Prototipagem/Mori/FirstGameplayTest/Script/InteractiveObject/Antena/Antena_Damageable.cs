using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Antena_Damageable : MonoBehaviour, IDamageable
{
    [SerializeField] public bool charged;
    private Gerador_Interactive connectedGenerator;
    public void TakeDamage(int damageAmount)
    {
        if (!charged)
        {
            charged = true;
            meshRenderer.sharedMaterial.SetInt("_Light_on_off", 1);
            connectedGenerator.OnRequirementChange();
        }
    }

    MeshRenderer meshRenderer;
    void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        meshRenderer.material = new Material(meshRenderer.material);
    }
    public void DefineConnectedGenerator(Gerador_Interactive generator)
    {
        connectedGenerator = generator;
    }
}
