using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MeleeRoaming_StoppedSubState : MeleeRoaming_SubState
{
    public UnityEvent<EnemyRoamingState> FixedUpdate_Event;

    public void CheckForEndOfCoroutine(EnemyRoamingState enemyRoamingState)
    {
        if(enemyRoamingState.iEnemy.waitOnPointTimer_Ref == null)
        {
            enemyRoamingState.ToggleMovement(true);
        }
    }
    public override void OnFixedUpdate(EnemyRoamingState enemyRoamingState)
    {
        FixedUpdate_Event.Invoke(enemyRoamingState);
    }
}
