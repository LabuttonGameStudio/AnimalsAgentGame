using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoisonPuddle : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if(TickDamage_Ref== null)
            {
                TickDamage_Ref = StartCoroutine(TickDamage_Coroutine());
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (TickDamage_Ref != null)
            {
                StopCoroutine(TickDamage_Ref);
                TickDamage_Ref = null;
            }
        }
    }
    private void OnDisable()
    {
        TickDamage_Ref = null;
    }
    private Coroutine TickDamage_Ref;
    IEnumerator TickDamage_Coroutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.5f);
            while (true)
            {
                ArmadilloPlayerController.Instance.hpControl.TakeDamage(new Damage(5, Damage.DamageType.Poison, false, transform.position));
                yield return new WaitForSeconds(0.5f);
            }
        }
    }
}
