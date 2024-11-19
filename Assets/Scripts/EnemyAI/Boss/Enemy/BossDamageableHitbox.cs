using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossDamageableHitbox : MonoBehaviour,IDamageable
{
    [SerializeField]private float maxHp;
    private float currentHp;

    [HideInInspector]public bool isBroken;
    [SerializeField] private GameObject brokenVersion;

    [SerializeField]ParticleSystem onHitVFX;
    [SerializeField]ParticleSystem onExplosionVFX;
    private void Awake()
    {
        currentHp = maxHp;
    }
    public bool isDead()
    {
        return isBroken; 
    }

    public void TakeDamage(Damage damage)
    {
        currentHp -= damage.damageAmount;
        if(currentHp <=0)
        {
            EnemyBoss.Instance.OnBatteryDestroyed();
            FPCameraShake.StartShake(0.75f, 2f, 1f);
            onExplosionVFX.Play();
            brokenVersion.SetActive(true);
            gameObject.SetActive(false);
        }
        else
        {
            onHitVFX.Play();
        }
    }
}
