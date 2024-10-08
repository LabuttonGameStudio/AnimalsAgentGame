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
    private Dictionary<Collider, IDamageable> damageablesInHitbox;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) return;
        IDamageable damageable = other.GetComponent<IDamageable>();
        if (damageable != null)
        {
            damageablesInHitbox.Add(other, damageable);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player")) return;
        if (damageablesInHitbox.TryGetValue(other, out IDamageable damageable))
        {
            damageablesInHitbox.Remove(other);
        }
    }
    public void Hit(float meleeDamage, float meleeStabModifier,out bool isTakedown)
    {
        if (damageablesInHitbox.Count <= 0)
        {
            MeleeAnimator.Instance.PlayRandomMeleeAnimation();
            isTakedown = false;
            return;
        }
        //Check if somehit is a takedown
        for (int i = 0; i < damageablesInHitbox.Count; ++i)
        {
            IDamageable damageable = damageablesInHitbox.ElementAt(i).Value;
            Collider collider = damageablesInHitbox.ElementAt(i).Key;

            if (collider.gameObject.layer != LayerMask.NameToLayer("Enemies")) continue;

            //Check backstab
            Transform enemyTransform = collider.transform;
            Vector3 dirFromEnemyToPlayer = transform.position - enemyTransform.position;
            dirFromEnemyToPlayer.Normalize();
            float dot = Vector3.Dot(enemyTransform.forward, dirFromEnemyToPlayer);

            float meleeFinalDamage = meleeDamage;
            if (dot < 0.25f)
            {
                meleeFinalDamage *= meleeStabModifier;
                StartCoroutine(Takedown(new Damage(meleeFinalDamage, DamageType.Slash, true, ArmadilloPlayerController.Instance.transform.position), damageable));
                MeleeAnimator.Instance.PlayTakedownAnimation();
                isTakedown = true;
                return;
            }
        }
        //Do damage for all entities
        MeleeAnimator.Instance.PlayRandomMeleeAnimation();
        isTakedown = true;
        for (int i = 0; i < damageablesInHitbox.Count; ++i)
        {
            IDamageable damageable = damageablesInHitbox.ElementAt(i).Value;
            damageable.TakeDamage(new Damage(meleeDamage, DamageType.Slash, true, ArmadilloPlayerController.Instance.transform.position));
        }
    }
    public IEnumerator Takedown(Damage damage,IDamageable target)
    {
        yield return new WaitForSeconds(0.42f);
        target.TakeDamage(damage);
    }
}
