using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeRoaming_MovingSubState : MeleeRoaming_SubState
{
    public override void OnFixedUpdate(EnemyRoamingState enemyRoamingState)
    {
        IEnemy iEnemy = enemyRoamingState.iEnemy;
        Vector3 movingDirection = iEnemy.navMeshAgent.velocity;
        iEnemy.LerpLookAt(movingDirection*5f);
        if (iEnemy.CheckForProximityOfPoint() && !iEnemy.enqueued)
        {
            bool isWaitOnPoint = iEnemy.aiPathList[iEnemy.currentPathPoint].waitTimeOnPoint > 0 && iEnemy.waitOnPointTimer_Ref == null;
            if (isWaitOnPoint)
            {
                iEnemy.waitOnPointTimer_Ref = iEnemy.StartCoroutine(
                    iEnemy.WaitOnPointTimer_Coroutine(iEnemy.aiPathList[iEnemy.currentPathPoint].waitTimeOnPoint
                    , iEnemy.aiPathList[iEnemy.currentPathPoint].lookAroundOnPoint));
                EnemyControl.Instance.AddAIToNavmeshQueue(iEnemy, iEnemy.NextPathPoint());
                enemyRoamingState.StopMovement();
            }
            else EnemyControl.Instance.AddAIToNavmeshQueue(iEnemy, iEnemy.NextPathPoint());
        }
    }
}
