using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class SandProjectile : MonoBehaviour
{
    public static float damageAmount;
    [SerializeField]private DecalProjector bulletDecalProjector;
    private void Awake()
    {
        damageAmount = 30;
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.TryGetComponent(out IDamageable damageable))
        {
            damageable.TakeDamage(new Damage(damageAmount,Damage.DamageType.Slash,false,Vector3.zero));
        }
        else if(!other.isTrigger)
        {
            bulletDecalProjector.transform.parent = null;
            bulletDecalProjector.enabled = true;
            gameObject.SetActive(false);
        }
    }
}
