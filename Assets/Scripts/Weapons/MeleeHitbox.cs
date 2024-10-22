using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static Damage;
public class MeleeHitbox : MonoBehaviour
{
    private Collider hitCollider;
    private void Awake()
    {
        hitCollider = GetComponent<Collider>();
        damageablesInHitbox = new Dictionary<Collider, IDamageable>();
    }
    [HideInInspector] public Dictionary<Collider, IDamageable> damageablesInHitbox;

    public void CheckListIntegrity()
    {
        for (int i = 0; i < damageablesInHitbox.Count; ++i)
        {
            if (!damageablesInHitbox.ElementAt(i).Key.gameObject.activeInHierarchy || damageablesInHitbox.ElementAt(i).Value.isDead())
            {
                damageablesInHitbox.Remove(damageablesInHitbox.ElementAt(i).Key);
                i--;
            }
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) return;
        if (other.TryGetComponent(out IDamageable iDamageable))
        {
            damageablesInHitbox.Add(other, iDamageable);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player")) return;
        if (damageablesInHitbox.ContainsKey(other))
        {
            damageablesInHitbox.Remove(other);
        }
    }
}
