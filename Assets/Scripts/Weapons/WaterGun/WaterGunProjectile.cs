using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Damage;
public class WaterGunProjectile : MonoBehaviour
{
    [HideInInspector] public Rigidbody rb;
    [HideInInspector] public Collider m_collider;
    [HideInInspector] public Vector3 playerPos;
    [HideInInspector] public float duration;
    [HideInInspector] public float timer;
    [HideInInspector] public float bulletDamage;
    [HideInInspector] public float minbulletDamage;
    private void Start()
    {
    
    }
    private void FixedUpdate()
    {
        timer += Time.fixedDeltaTime;
        if (timer > duration)
        {
            DisableProjectile();
        }
        rb.AddForce(Physics.gravity * WaterGunProjectileManager.Instance.gravity, ForceMode.Acceleration);
    }
    private void OnTriggerEnter(Collider other)
    {
        int layerInt = other.gameObject.layer;
        if (layerInt == LayerMask.NameToLayer("Ground"))
        {
            WaterGunProjectileManager.Instance.SpawnPuddle(rb.position);

            Vector3 insidePoint = other.ClosestPointOnBounds(rb.position-rb.velocity*0.025f);
            WaterGunProjectileManager.Instance.OnHit(insidePoint);
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
            WaterGunProjectileManager.Instance.OnHit(rb.position);
            if (damageable.isDead())
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
