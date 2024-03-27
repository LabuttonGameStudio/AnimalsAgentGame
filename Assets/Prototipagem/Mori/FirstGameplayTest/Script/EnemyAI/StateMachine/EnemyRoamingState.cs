using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static IEnemy;

public class EnemyRoamingState : EnemyState
{
    public EnemyRoamingState(IEnemy enemyCtrl) : base(enemyCtrl)
    {
        enemyControl = enemyCtrl;
    }
    public override void OnVisibilityUpdate()
    {
        if(enemyControl.CheckForLOS(ArmadilloPlayerController.Instance.gameObject))
        {
            enemyControl.ChangeCurrentAIState(AIState.Observing);
        }
    }

    public override void OnActionUpdate()
    {
        enemyControl.CheckForProximityOfPoint();
    }

    public override void OnEnterState()
    {
        enemyControl.SetNextDestinationOfNavmesh();
    }

    public override void OnExitState()
    {

    }
}
