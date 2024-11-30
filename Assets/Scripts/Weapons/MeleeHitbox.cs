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
        rbInHitbox = new Dictionary<Rigidbody, IDamageable>();
        collidersInHitbox = new Dictionary<Collider, IDamageable>();
    }
    [HideInInspector] public Dictionary<Rigidbody, IDamageable> rbInHitbox;
    [HideInInspector] public Dictionary<Collider, IDamageable> collidersInHitbox;

    public void CheckListIntegrity()
    {
        for (int i = 0; i < rbInHitbox.Count; ++i)
        {
            if (!rbInHitbox.ElementAt(i).Key.gameObject.activeInHierarchy || rbInHitbox.ElementAt(i).Value.isDead())
            {
                rbInHitbox.Remove(rbInHitbox.ElementAt(i).Key);
                i--;
            }
        }
        for (int i = 0; i < collidersInHitbox.Count; ++i)
        {
            if (!collidersInHitbox.ElementAt(i).Key.gameObject.activeInHierarchy || collidersInHitbox.ElementAt(i).Value.isDead())
            {
                collidersInHitbox.Remove(collidersInHitbox.ElementAt(i).Key);
                i--;
            }
        }
    }
    public List<IDamageable> GetListOfDamageables()
    {
        CheckListIntegrity();
        List<IDamageable> list = new List<IDamageable>();

        for (int i = 0; i < rbInHitbox.Count; ++i)
        {
            list.Add(rbInHitbox.ElementAt(i).Value);
        }
        for (int i = 0; i < collidersInHitbox.Count; ++i)
        {
            list.Add(collidersInHitbox.ElementAt(i).Value);
        }
        return list;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) return;
        //Do q coloca lista por collider fazer por rigidbody
        if (other.attachedRigidbody != null)
        {
            if (!rbInHitbox.ContainsKey(other.attachedRigidbody))
            {
                if (other.TryGetComponent(out IDamageable iDamageable))
                {
                    rbInHitbox.Add(other.attachedRigidbody, iDamageable);
                }
            }
        }
        else
        {
            if (!collidersInHitbox.ContainsKey(other))
            {
                if (other.TryGetComponent(out IDamageable iDamageable))
                {
                    collidersInHitbox.Add(other, iDamageable);
                }
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player")) return;
        if (other.attachedRigidbody != null)
        {
            if (rbInHitbox.ContainsKey(other.attachedRigidbody))
            {
                rbInHitbox.Remove(other.attachedRigidbody);
            }
        }
        else
        {
            if (collidersInHitbox.ContainsKey(other))
            {
                collidersInHitbox.Remove(other);
            }
        }
    }
}
