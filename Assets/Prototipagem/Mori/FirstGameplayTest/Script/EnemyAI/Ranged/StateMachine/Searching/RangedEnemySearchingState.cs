using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedEnemySearchingState : RangedEnemyState
{
    public RangedEnemySearchingState(RangedEnemy enemyCtrl) : base(enemyCtrl)
    {
        iEnemy = enemyCtrl;
    }

    public override void OnActionUpdate()
    {
        
    }

    public override void OnEnterState()
    {
        Debug.Log("Searching Enter");
    }

    public override void OnExitState()
    {
        Debug.Log("Searching Exit");
    }

    public override void OnFixedUpdate()
    {
        Debug.Log("Searching FixedUpdate");
    }

    public override void OnVisibilityUpdate()
    {
        if (iEnemy.stateEnum != RangedEnemy.CurrentStateEnum.searching)
        {
            switch (iEnemy.stateEnum)
            {
                case RangedEnemy.CurrentStateEnum.roaming:
                    iEnemy.ChangeCurrentState(iEnemy.enemyRoamingState);
                    break;
                case RangedEnemy.CurrentStateEnum.observing:
                    iEnemy.ChangeCurrentState(iEnemy.enemyObservingState);
                    break;
                case RangedEnemy.CurrentStateEnum.searching:
                    iEnemy.ChangeCurrentState(iEnemy.enemySearchingState);
                    break;
                case RangedEnemy.CurrentStateEnum.attacking:
                    iEnemy.ChangeCurrentState(iEnemy.enemyAttackingState);
                    break;
            }
        }
    }
}
