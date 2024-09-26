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
        Debug.Log("Observing FixedUpdate");
    }

    public override void OnVisibilityUpdate()
    {
        
    }
}
