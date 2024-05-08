using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeRoaming_MovingSubState : MeleeRoaming_SubState
{
    public override void OnFixedUpdate(EnemyRoamingState enemyRoamingState)
    {
        IEnemy iEnemy = enemyRoamingState.iEnemy;
        Vector3 movingDirection = iEnemy.navMeshAgent.velocity;
        iEnemy.LerpLookAt(movingDirection * 5f);
        if (iEnemy.CheckForProximityOfPoint() && !iEnemy.enqueued)
        {
            AIPathPoint currentPathPoint = iEnemy.aiPathList[iEnemy.currentPathPoint];
            bool isWaitOnPoint = currentPathPoint.waitTimeOnPoint > 0;
            if (isWaitOnPoint)
            {
                if (currentPathPoint.lookAroundOnPoint)
                {
                    if (iEnemy.TryStartRandomLookAround(currentPathPoint.waitTimeOnPoint, out Coroutine coroutine))
                    {
                        EnemyControl.Instance.AddAIToNavmeshQueue(iEnemy, iEnemy.NextPathPoint());
                        enemyRoamingState.StopMovementAndLookAround();
                    }
                }
                else
                {
                    iEnemy.StartWaitOnPoint(currentPathPoint.waitTimeOnPoint);
                    enemyRoamingState.StopMovement();
                }
            }
            else EnemyControl.Instance.AddAIToNavmeshQueue(iEnemy, iEnemy.NextPathPoint());
        }
    }
}
