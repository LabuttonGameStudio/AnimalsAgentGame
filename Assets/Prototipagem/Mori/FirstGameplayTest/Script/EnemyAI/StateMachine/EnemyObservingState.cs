using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static IEnemy;

public class EnemyObservingState : EnemyState
{
    public EnemyObservingState(IEnemy enemyCtrl) : base(enemyCtrl)
    {
        enemyControl = enemyCtrl;
    }

    public override void OnVisibilityUpdate()
    {
        enemyControl.ToggleIncreaseDetectionCoroutine(enemyControl.CheckForLOS(ArmadilloPlayerController.Instance.gameObject));

    }

    public override void OnActionUpdate()
    {

    }
    public override void OnEnterState()
    {
        enemyControl.navMeshAgent.isStopped = true;
        enemyControl.ToggleIncreaseDetectionCoroutine(true);
    }

    public override void OnExitState()
    {

    }
}
