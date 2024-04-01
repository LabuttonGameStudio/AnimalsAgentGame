using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySearchingState : MeleeEnemyState
{
    public EnemySearchingState(EnemyMelee enemyCtrl) : base(enemyCtrl)
    {
        enemyControl = enemyCtrl;
    }
    Vector3 lastKnownPlayerPos;
    public override void OnVisibilityUpdate()
    {
        GameObject playerGO = ArmadilloPlayerController.Instance.gameObject;
        if (enemyControl.CheckForLOS(playerGO))
        {
            enemyControl.ToggleIncreaseDetectionCoroutine(true);
            lastKnownPlayerPos = playerGO.transform.position;
        }
        else
        {
            enemyControl.ToggleIncreaseDetectionCoroutine(false);
        }

    }

    public override void OnActionUpdate()
    {
        enemyControl.SetNextDestinationOfNavmesh(lastKnownPlayerPos);
        enemyControl.transform.LookAt(new Vector3(lastKnownPlayerPos.x,enemyControl.transform.position.y,lastKnownPlayerPos.z));
    }
    public override void OnEnterState()
    {
        enemyControl.navMeshAgent.isStopped = false;
        enemyControl.SetNextDestinationOfNavmesh(lastKnownPlayerPos);
        enemyControl.navMeshAgent.stoppingDistance = 10;
    }

    public override void OnExitState()
    {

    }
}
