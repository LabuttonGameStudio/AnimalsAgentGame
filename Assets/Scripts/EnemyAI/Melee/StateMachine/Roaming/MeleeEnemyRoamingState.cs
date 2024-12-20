using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static IEnemy;

public class MeleeEnemyRoamingState : MeleeEnemyState
{
    public MeleeEnemyRoamingState(EnemyMelee enemyCtrl) : base(enemyCtrl)
    {
        iEnemy = enemyCtrl;
    }
    public override void OnVisibilityUpdate()
    {
        if (iEnemy.CheckForPlayerLOS() > 0)
        {
            iEnemy.lastKnownPlayerPos = ArmadilloPlayerController.Instance.transform.position;
            iEnemy.ChangeCurrentState(iEnemy.enemyObservingState);
        }
    }

    public override void OnActionUpdate()
    {

    }

    public override void OnEnterState()
    {
        iEnemy.currentAIBehaviour = AIBehaviourEnums.AIBehaviour.Roaming;
        iEnemy.enemyBehaviourVisual.ChangeVisualState(AIBehaviourEnums.AIBehaviour.Roaming);
        if (iEnemy.isStatic)
        {

        }
        else
        {
            if (loopRoamingPath_Ref != null)
            {
                iEnemy.StopCoroutine(loopRoamingPath_Ref);
                loopRoamingPath_Ref = null;
            }
            loopRoamingPath_Ref = iEnemy.StartCoroutine(LoopRoamingPath_Coroutine());
        }
    }

    public override void OnExitState()
    {
        if (loopRoamingPath_Ref != null)
        {
            iEnemy.StopCoroutine(loopRoamingPath_Ref);
        }
        if(lookAround_Ref !=null)
        {
            iEnemy.StopCoroutine(lookAround_Ref);
            lookAround_Ref = null;
        }
    }

    public override void OnFixedUpdate()
    {
        

    }
    private Coroutine lookAround_Ref;
    private Coroutine loopRoamingPath_Ref;
    public IEnumerator LoopRoamingPath_Coroutine()
    {
        if (iEnemy.TrySetNextDestination(iEnemy.aiPathList[iEnemy.currentPathPoint].transformOfPathPoint.position))
        {
            iEnemy.animator.SetBool("isWalking", true);
            yield return iEnemy.RoamingWaitToReachNextPoint();
            iEnemy.animator.SetBool("isWalking", false);
            if (iEnemy.aiPathList[iEnemy.currentPathPoint].waitTimeOnPoint > 0)
            {
                if (iEnemy.aiPathList[iEnemy.currentPathPoint].lookAroundOnPoint)
                {
                    if (iEnemy.TryStartRandomLookAround(iEnemy.aiPathList[iEnemy.currentPathPoint].waitTimeOnPoint, out Coroutine lookAroundcoroutine))
                    {
                        lookAround_Ref = lookAroundcoroutine;
                        yield return lookAroundcoroutine;
                        lookAround_Ref = null;
                    }
                }
                else
                {
                    yield return new WaitForSeconds(iEnemy.aiPathList[iEnemy.currentPathPoint].waitTimeOnPoint);
                }
            }
        }
        while (true)
        {
            if (iEnemy.CheckNextPathPoint())
            {
                if (iEnemy.TrySetNextDestination(iEnemy.NextPathPoint()))
                {
                    iEnemy.animator.SetBool("isWalking", true);
                    yield return iEnemy.RoamingWaitToReachNextPoint();
                    if (iEnemy.aiPathList[iEnemy.currentPathPoint].waitTimeOnPoint > 0)
                    {
                        iEnemy.animator.SetBool("isWalking", false);
                        if (iEnemy.aiPathList[iEnemy.currentPathPoint].lookAroundOnPoint)
                        {
                            if (iEnemy.TryStartRandomLookAround(iEnemy.aiPathList[iEnemy.currentPathPoint].waitTimeOnPoint, out Coroutine lookAroundcoroutine))
                            {
                                lookAround_Ref = lookAroundcoroutine;
                                yield return lookAroundcoroutine;
                                lookAround_Ref = null;
                            }
                        }
                        else
                        {
                            yield return new WaitForSeconds(iEnemy.aiPathList[iEnemy.currentPathPoint].waitTimeOnPoint);
                        }
                    }
                }
            }
            else
            {
                iEnemy.animator.SetBool("isWalking", false);
                loopRoamingPath_Ref = null;
                iEnemy.isStatic = true;
                yield break;
            }
            yield return new WaitForFixedUpdate();
        }
    }
}
