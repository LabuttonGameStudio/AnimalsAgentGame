using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Damage;

public class PendriveSniperProjectile : MonoBehaviour
{
    [HideInInspector] public Rigidbody rb;
    [HideInInspector] public Vector3 playerPos;
    [HideInInspector] public float duration;
    [HideInInspector] public float timer;
    [HideInInspector] public float bulletDamage;

    private void FixedUpdate()
    {
        timer += Time.fixedDeltaTime;
        if (timer > duration)
        {
            DisableProjectile();
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out IDamageable damageable) && !other.CompareTag("Player"))
        {
            damageable.TakeDamage(new Damage(bulletDamage, DamageType.Hacking, true, playerPos));
        }
        else if(!other.isTrigger)DisableProjectile();
    }
    private void DisableProjectile()
    {
        timer = 0;
        rb.velocity = Vector3.zero;
        PendriveSniperProjectileManager.Instance.ReturnProjectileToPool(this);
        gameObject.SetActive(false);
    }
}
