using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class RangedEnemyState
{
    public RangedEnemyState(RangedEnemy enemyCtrl)
    {
        iEnemy = enemyCtrl;
    }
    public RangedEnemy iEnemy;
    public abstract void OnVisibilityUpdate();
    public abstract void OnActionUpdate();
    public abstract void OnFixedUpdate();
    public abstract void OnEnterState();
    public abstract void OnExitState();
}
