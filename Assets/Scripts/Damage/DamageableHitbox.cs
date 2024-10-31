using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DamageableHitbox : MonoBehaviour,IDamageable
{
    [HideInInspector]public Collider[] _Collider;
    [SerializeField] private float damageMultiplier = 1f;
    [HideInInspector]public UnityEvent<Damage> OnTakeDamage;
    [HideInInspector] public bool _IsDead;

    private void Awake()
    {
        _Collider = GetComponents<Collider>();
    }
    public bool isDead()
    {
        return _IsDead;
    }

    public void TakeDamage(Damage damage)
    {
        if (_IsDead) return;
        damage.damageAmount *= damageMultiplier;
        OnTakeDamage.Invoke(damage);
    }
}
