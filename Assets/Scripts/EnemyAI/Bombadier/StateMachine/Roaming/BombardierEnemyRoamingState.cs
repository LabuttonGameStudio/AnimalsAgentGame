using System.Collections;
using UnityEngine;

public class BombardierEnemyRoamingState : BombardierEnemyState
{
    public BombardierEnemyRoamingState(EnemyBombardier enemyCtrl) : base(enemyCtrl)
    {
        iEnemy = enemyCtrl;
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
        //Debug.Log("Roaming Enter");
    }

    public override void OnExitState()
    {
        if (loopRoamingPath_Ref != null)
        {
            iEnemy.StopCoroutine(loopRoamingPath_Ref);
            //loopRoamingPath_Ref = null;
        }
        //Debug.Log("Roaming Exit");
    }

    public override void OnFixedUpdate()
    {
        //Debug.Log("Roaming FixedUpdate");
    }

    public override void OnVisibilityUpdate()
    {
        if (iEnemy.CheckForPlayerLOS() > 0)
        {
            iEnemy.lastKnownPlayerPos = ArmadilloPlayerController.Instance.transform.position;
            iEnemy.ChangeCurrentState(iEnemy.enemyObservingState);
        }
    }

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
                yield return new WaitForSeconds(iEnemy.aiPathList[iEnemy.currentPathPoint].waitTimeOnPoint);

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
                    iEnemy.animator.SetBool("isWalking", false);
                    if (iEnemy.aiPathList[iEnemy.currentPathPoint].waitTimeOnPoint > 0)
                    {
                        yield return new WaitForSeconds(iEnemy.aiPathList[iEnemy.currentPathPoint].waitTimeOnPoint);
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
