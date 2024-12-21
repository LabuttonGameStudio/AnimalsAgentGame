using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MeleeEnemyState
{
    public MeleeEnemyState(EnemyMelee enemyCtrl)
    {
        iEnemy = enemyCtrl;
    }
    public EnemyMelee iEnemy;
    public abstract void OnVisibilityUpdate();
    public abstract void OnActionUpdate();
    public abstract void OnFixedUpdate();
    public abstract void OnEnterState();
    public abstract void OnExitState();
}
