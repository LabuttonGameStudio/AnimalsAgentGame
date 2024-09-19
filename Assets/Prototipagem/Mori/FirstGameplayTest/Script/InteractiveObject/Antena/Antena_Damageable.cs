using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Damage;

public class Antena_Damageable :MonoBehaviour,IRequirements, IDamageable
{
    public bool isTurnedOn { get;  set; }
    public INeedRequirements connectedObject { get; set; }
    private BulletManager objectiveManager;

    [SerializeField] public MeshRenderer[] meshRenderers;

    public void TakeDamage(Damage damage)
    {
        if (!isTurnedOn)
        {
            switch (damage.damageType)
            {
                case DamageType.Eletric:
                case DamageType.Hacking:
                    isTurnedOn = true;
                    meshRenderers[0].sharedMaterial.SetInt("_Light_on_off", 1);
                    if (connectedObject != null) connectedObject.OnRequirementChange();
                    // Notifica o ObjectiveManager
                    if (objectiveManager != null)
                        objectiveManager.OnAntennaActivated();
                    break;
            }
        }
    }

    void Awake()
    {
        Material material = new Material(meshRenderers[0].material);
        meshRenderers[0].sharedMaterial = material;
        meshRenderers[1].sharedMaterial = material;
        objectiveManager = Object.FindFirstObjectByType<BulletManager>();
        
    }
}
