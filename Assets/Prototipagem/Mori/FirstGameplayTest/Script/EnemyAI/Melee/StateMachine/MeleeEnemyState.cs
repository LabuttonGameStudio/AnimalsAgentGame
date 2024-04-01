using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MeleeEnemyState
{
    public MeleeEnemyState(EnemyMelee enemyCtrl)
    {
        enemyControl = enemyCtrl;
    }
    protected EnemyMelee enemyControl;
    public abstract void OnActionUpdate();
    public abstract void OnVisibilityUpdate();

    public abstract void OnEnterState();
    public abstract void OnExitState();
}
