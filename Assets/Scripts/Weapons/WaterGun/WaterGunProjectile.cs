using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Damage;
public class WaterGunProjectile : MonoBehaviour
{
    [HideInInspector] public Rigidbody rb;
    [HideInInspector] public Vector3 playerPos;
    [HideInInspector] public float duration;
    [HideInInspector] public float timer;
    [HideInInspector] public float bulletDamage;
    [HideInInspector] public float minbulletDamage;
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
        int layerInt = other.gameObject.layer;
        if (layerInt == LayerMask.NameToLayer("Ground"))
        {
            WaterGunProjectileManager.Instance.SpawnPuddle(rb.position);
            WaterGunProjectileManager.Instance.OnHit(rb.position);
            DisableProjectile();
        }
        else if (other.CompareTag("Puddle"))
        {
            DisableProjectile();
            other.GetComponent<WaterPuddle>().AddSize(1);
        }
        else if (other.TryGetComponent(out IDamageable damageable) && !other.CompareTag("Player"))
        {
            float damageAmount = (1 - timer / duration) * bulletDamage;
            damageAmount = Mathf.Max(damageAmount, minbulletDamage);
            Damage damage = new Damage(damageAmount, DamageType.Water, true, playerPos);
            damageable.TakeDamage(damage);
            if(damageable.isDead())
            {
                WaterGunVisual.Instance.OnLetalShot();
            }
            else
            {
                if(damage.wasCritical) WaterGunVisual.Instance.OnHeadshotShot();
                else WaterGunVisual.Instance.OnBodyShot();
            }
            DisableProjectile();
        }
    }
    private void DisableProjectile()
    {
        timer = 0;
        rb.velocity = Vector3.zero;
        WaterGunProjectileManager.Instance.ReturnProjectileToPool(this);
        gameObject.SetActive(false);
    }
}
