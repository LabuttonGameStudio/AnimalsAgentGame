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
        if (iEnemy.stateEnum != RangedEnemy.CurrentStateEnum.observing)
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
