using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedEnemyAttackingState : RangedEnemyState
{
    public RangedEnemyAttackingState(RangedEnemy enemyCtrl) : base(enemyCtrl)
    {
        iEnemy = enemyCtrl;
    }

    public override void OnActionUpdate()
    {
        
    }

    public override void OnEnterState()
    {
        Debug.Log("Attacking Enter");
    }

    public override void OnExitState()
    {
        Debug.Log("Attacking Exit");
    }

    public override void OnFixedUpdate()
    {
        Debug.Log("Attacking FixedUpdate");
    }

    public override void OnVisibilityUpdate()
    {
        if (iEnemy.stateEnum != RangedEnemy.CurrentStateEnum.attacking)
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
