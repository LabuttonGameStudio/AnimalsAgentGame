using System.Collections;
using UnityEngine;

public class RangedEnemyRoamingState : RangedEnemyState
{
    public RangedEnemyRoamingState(RangedEnemy enemyCtrl) : base(enemyCtrl)
    {
        iEnemy = enemyCtrl;
    }

    public override void OnActionUpdate()
    {

    }

    public override void OnEnterState()
    {
        if (iEnemy.isStatic)
        {

        }
        else
        {
            Debug.Log("a");
            if (loopRoamingPath_Ref == null)
            {
                loopRoamingPath_Ref = iEnemy.StartCoroutine(LoopRoamingPath_Coroutine());
            }
        }
        Debug.Log("Roaming Enter");
    }

    public override void OnExitState()
    {
        if (loopRoamingPath_Ref != null)
        {
            iEnemy.StopCoroutine(loopRoamingPath_Ref);
            loopRoamingPath_Ref = null;
        }
        Debug.Log("Roaming Exit");
    }

    public override void OnFixedUpdate()
    {
        Debug.Log("Roaming FixedUpdate");
    }

    public override void OnVisibilityUpdate()
    {
        if (iEnemy.CheckForPlayerLOS() > 0)
        {
            iEnemy.lastKnownPlayerPos = ArmadilloPlayerController.Instance.transform.position;
            iEnemy.ChangeCurrentState(iEnemy.enemyAttackingState);
        }
    }

    private Coroutine loopRoamingPath_Ref;
    public IEnumerator LoopRoamingPath_Coroutine()
    {
        if (iEnemy.TrySetNextDestination(iEnemy.aiPathList[iEnemy.currentPathPoint].transformOfPathPoint.position))
        {
            yield return iEnemy.RoamingWaitToReachNextPoint();
        }
        while (true)
        {
            if (iEnemy.CheckNextPathPoint())
            {
                if (iEnemy.TrySetNextDestination(iEnemy.NextPathPoint()))
                {
                    yield return iEnemy.RoamingWaitToReachNextPoint();
                }
            }
            else
            {
                loopRoamingPath_Ref = null;
                iEnemy.isStatic = true;
                yield break;
            }
            yield return new WaitForFixedUpdate();
        }
    }
}
