using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyMelee : IEnemy,IDamageable
{
    public void TakeDamage(int damageAmount)
    {
        currentHp -= damageAmount;
        Debug.Log("HP="+currentHp+"| Damage taken="+damageAmount);
        if(currentHp <= 0)
        {
            gameObject.SetActive(false);
        }
    }
    protected override void OnAwake()
    {
        
    }
    protected override void OnStart()
    {
        
    }
    protected override void OnFixedUpdate()
    {
        
    }
}
