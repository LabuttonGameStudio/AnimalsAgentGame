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

        Debug.Log("Roaming Enter");
    }

    public override void OnExitState()
    {
        Debug.Log("Roaming Exit");
    }

    public override void OnFixedUpdate()
    {
        Debug.Log("Roaming FixedUpdate");
    }

    public override void OnVisibilityUpdate()
    {
        if (iEnemy.stateEnum != RangedEnemy.CurrentStateEnum.roaming)
        {
            switch (iEnemy.stateEnum)
            {
                case RangedEnemy.CurrentStateEnum.roaming:
                    iEnemy.ChangeCurrentState(iEnemy.enemyRoamingState);
                    break;
                case RangedEnemy.CurrentStateEnum.observing:
                    iEnemy.ChangeCurrentState(iEnemy.enemyObservingState);
                    break;
                case RangedEnemy.CurrentStateEnum.searching:
                    iEnemy.ChangeCurrentState(iEnemy.enemySearchingState);
                    break;
                case RangedEnemy.CurrentStateEnum.attacking:
                    iEnemy.ChangeCurrentState(iEnemy.enemyAttackingState);
                    break;
            }
        }
    }

    private Coroutine loopRoamingPath_Ref;
    public IEnumerator LoopRoamingPath_Coroutine()
    {
        iEnemy.TrySetNextDestination(iEnemy.aiPathList[0].transformOfPathPoint.position);
        while(true)
        {
            yield return iEnemy.WaitToReachNextPoint_Coroutine();
        }
    }
}
