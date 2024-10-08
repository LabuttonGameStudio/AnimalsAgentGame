using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeRoaming_MovingSubState : MeleeRoaming_SubState
{
    public override void OnFixedUpdate(MeleeEnemyRoamingState enemyRoamingState)
    {
        IEnemy iEnemy = enemyRoamingState.iEnemy;
        Vector3 movingDirection = iEnemy.navMeshAgent.velocity + iEnemy.transform.position;
        iEnemy.LerpLookAt(movingDirection,2f);
        //If is near enough
        if (iEnemy.CheckForProximityOfPoint() && !iEnemy.enqueued)
        {
            AIPathPoint currentPathPoint = iEnemy.aiPathList[iEnemy.currentPathPoint];
            bool isWaitOnPoint = currentPathPoint.waitTimeOnPoint > 0;

            //Se o inimigo precisa ficar parado no ponto
            if (isWaitOnPoint)
            {
                //Se o inimigo deve olhar aos lados quando esta parado 
                if (currentPathPoint.lookAroundOnPoint)
                {
                    if (iEnemy.TryStartRandomLookAround(currentPathPoint.waitTimeOnPoint, out Coroutine coroutine))
                    {
                        EnemyMasterControl.Instance.AddAIToNavmeshQueue(iEnemy, iEnemy.NextPathPoint());
                        enemyRoamingState.StopMovementAndLookAround();
                    }
                }
                //Se caso ele apenas estiver q permanecer parado
                else
                {
                    EnemyMasterControl.Instance.AddAIToNavmeshQueue(iEnemy, iEnemy.NextPathPoint());
                    iEnemy.StartWaitOnPoint(currentPathPoint.waitTimeOnPoint);
                    enemyRoamingState.StopMovement();
                }
            }
            //Apenas vai para o proximo lugar
            else EnemyMasterControl.Instance.AddAIToNavmeshQueue(iEnemy, iEnemy.NextPathPoint());
        }
    }
}
