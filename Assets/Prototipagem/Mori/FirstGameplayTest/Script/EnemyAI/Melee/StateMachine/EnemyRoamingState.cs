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
        //if (enemyControl.CheckForLOS(ArmadilloPlayerController.Instance.gameObject))
        //{
        //    enemyControl.IncreaseDetection();
        //}
        //else
        //{
        //    enemyControl.DecreaseDetection();
        //}
    }

    public override void OnActionUpdate()
    {
        
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
        if (enemyControl.CheckForProximityOfPoint() && !enemyControl.enqueued)
        {
            if (enemyControl.aiPathList[enemyControl.currentPathPoint].waitTimeOnPoint > 0)
            {
                if (enemyControl.waitOnPointTimer_Ref == null)
                {
                    enemyControl.waitOnPointTimer_Ref = enemyControl.StartCoroutine(
                        enemyControl.WaitOnPointTimer_Coroutine(enemyControl.aiPathList[enemyControl.currentPathPoint].waitTimeOnPoint
                        , enemyControl.aiPathList[enemyControl.currentPathPoint].lookAroundOnPoint));
                }
            }
            EnemyControl.Instance.AddAIToNavmeshQueue(enemyControl, enemyControl.NextPathPoint());
        }
    }
}
