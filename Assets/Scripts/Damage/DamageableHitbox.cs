using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DamageableHitbox : MonoBehaviour,IDamageable
{
    [SerializeField] private float damageMultiplier = 1f;
    [HideInInspector]public UnityEvent<Damage> OnTakeDamage;
    public bool isDead()
    {
        throw new System.NotImplementedException();
    }

    public void TakeDamage(Damage damage)
    {
        damage.damageAmount *= damageMultiplier;
        OnTakeDamage.Invoke(damage);
    }
}
