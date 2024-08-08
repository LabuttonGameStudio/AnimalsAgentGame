using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Damage;
public class MeleeHitbox : MonoBehaviour
{
    private Collider hitCollider;
    [SerializeField]private ParticleSystem hitParticle;
    private void Awake()
    {
        hitCollider = GetComponent<Collider>();
        damageablesInHitbox = new List<IDamageable>();
    }
    private List<IDamageable> damageablesInHitbox;
    private void OnTriggerEnter(Collider other)
    {
        IDamageable damageable = other.GetComponent<IDamageable>();
        if (damageable != null)
        {
            if (other.CompareTag("Player")) return;
            damageablesInHitbox.Add(damageable);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        IDamageable damageable = other.GetComponent<IDamageable>();
        if (damageable != null && damageablesInHitbox.Contains(damageable))
        {
            if (other.CompareTag("Player")) return;
            damageablesInHitbox.Remove(damageable);
        }
    }
    public void Hit(float meleeDamage, float meleeStabModifier)
    {
        foreach (IDamageable damageable in damageablesInHitbox)
        {
            float meleeFinalDamage = meleeDamage;
            damageable.TakeDamage(new Damage(meleeFinalDamage, DamageType.Slash, true, ArmadilloPlayerController.Instance.transform.position));
        }
        hitParticle.Play();
    }
}
