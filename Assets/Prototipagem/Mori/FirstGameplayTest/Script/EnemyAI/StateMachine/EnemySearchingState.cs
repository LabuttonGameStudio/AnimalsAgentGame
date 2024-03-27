using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySearchingState : EnemyState
{
    public EnemySearchingState(IEnemy enemyCtrl) : base(enemyCtrl)
    {
        enemyControl = enemyCtrl;
    }
    public override void OnVisibilityUpdate()
    {
        
    }

    public override void OnActionUpdate()
    {
        
    }
    public override void OnEnterState()
    {

    }

    public override void OnExitState()
    {

    }
}
