using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedEnemyObservingState : RangedEnemyState
{
    public RangedEnemyObservingState(RangedEnemy enemyCtrl) : base(enemyCtrl)
    {
        iEnemy = enemyCtrl;
    }

    public override void OnActionUpdate()
    {
        
    }

    public override void OnEnterState()
    {
        Debug.Log("Observing Enter");
    }

    public override void OnExitState()
    {
        Debug.Log("Observing Exit");
    }

    public override void OnFixedUpdate()
    {
        iEnemy.LerpLookAt(iEnemy.lastKnownPlayerPos, 1);
        Debug.Log("Observing FixedUpdate");
    }

    public override void OnVisibilityUpdate()
    {
        if (iEnemy.CheckForPlayerLOS() > 0)
        {
            
        }
    }
}
