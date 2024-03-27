using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EnemyState
{
    public EnemyState(IEnemy enemyCtrl)
    {
        enemyControl = enemyCtrl;
    }
    protected IEnemy enemyControl;
    public abstract void OnActionUpdate();
    public abstract void OnVisibilityUpdate();

    public abstract void OnEnterState();
    public abstract void OnExitState();
}
