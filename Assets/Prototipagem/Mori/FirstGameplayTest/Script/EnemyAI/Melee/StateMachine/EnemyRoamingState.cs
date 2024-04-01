using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static IEnemy;

public class EnemyRoamingState : MeleeEnemyState
{
    public EnemyRoamingState(EnemyMelee enemyCtrl) : base(enemyCtrl)
    {
        enemyControl = enemyCtrl;
    }
    public override void OnVisibilityUpdate()
    {
        enemyControl.ToggleIncreaseDetectionCoroutine(enemyControl.CheckForLOS(ArmadilloPlayerController.Instance.gameObject));
    }

    public override void OnActionUpdate()
    {
        enemyControl.CheckForProximityOfPoint();
    }

    public override void OnEnterState()
    {
        enemyControl.SetNextDestinationOfNavmesh(enemyControl.aiPathList[enemyControl.currentPathPoint].transformOfPathPoint.position);
    }

    public override void OnExitState()
    {
        enemyControl.BreakOnWaitPointCoroutine();
    }
}
