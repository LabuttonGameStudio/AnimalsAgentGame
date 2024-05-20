using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMeleeAttackHitBox : MonoBehaviour
{
    [HideInInspector]public EnemyMelee aiController;
    private Collider connectedCollider;
    private void Awake()
    {
        connectedCollider = GetComponent<Collider>();
    }
    public void EnableHitBox()
    {
        connectedCollider.enabled = true;
    }
    public void DisableHitBox()
    {
        connectedCollider.enabled = false;
        targets.Clear();
    }
    public bool DealDamageToThingsInside(Damage damage)
    {
        foreach(IDamageable idamageable in targets)
        {
            if (idamageable == aiController as IDamageable) continue;
            idamageable.TakeDamage(damage);
        }
        return (targets.Count > 0);
    }
    private List<IDamageable> targets = new List<IDamageable>();
    private void OnTriggerEnter(Collider other)
    {
        if(other.TryGetComponent(out IDamageable idamageable))
        {
            targets.Add(idamageable);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out IDamageable idamageable))
        {
            targets.Remove(idamageable);
        }
    }
}
