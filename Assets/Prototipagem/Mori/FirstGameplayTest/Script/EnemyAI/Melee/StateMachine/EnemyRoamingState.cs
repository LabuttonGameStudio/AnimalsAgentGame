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
        if (enemyControl.CheckForLOS(ArmadilloPlayerController.Instance.gameObject))
        {
            enemyControl.IncreaseDetection();
        }
        else
        {
            enemyControl.DecreaseDetection();
        }
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

    public override void OnFixedUpdate()
    {
        
    }
}
