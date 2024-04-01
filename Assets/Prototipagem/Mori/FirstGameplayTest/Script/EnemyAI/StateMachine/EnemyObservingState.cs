using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static IEnemy;

public class EnemyObservingState : EnemyState
{
    public EnemyObservingState(IEnemy enemyCtrl) : base(enemyCtrl)
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
        enemyControl.transform.LookAt(lastKnownPlayerPos);
    }
    public override void OnEnterState()
    {
        enemyControl.navMeshAgent.isStopped = true;
        enemyControl.navMeshAgent.updateRotation = false;
    }

    public override void OnExitState()
    {

    }
}
