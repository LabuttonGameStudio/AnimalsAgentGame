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
        if (iEnemy.CheckForPlayerLOS() > 0)
        {
            iEnemy.lastKnownPlayerPos = ArmadilloPlayerController.Instance.transform.position;
            iEnemy.ChangeCurrentState(iEnemy.enemyAttackingState);
        }
    }

    private Coroutine loopRoamingPath_Ref;
    public IEnumerator LoopRoamingPath_Coroutine()
    {
        while(true)
        {
            yield return iEnemy.WaitToReachNextPoint_Coroutine();
        }
    }
}
