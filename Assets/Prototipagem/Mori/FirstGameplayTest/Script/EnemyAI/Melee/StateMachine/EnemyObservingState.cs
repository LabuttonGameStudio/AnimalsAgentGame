using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static IEnemy;

public class EnemyObservingState : MeleeEnemyState
{
    public EnemyObservingState(EnemyMelee enemyCtrl) : base(enemyCtrl)
    {
        enemyControl = enemyCtrl;
    }
    Vector3 lastKnownPlayerPos;
    public override void OnVisibilityUpdate()
    {
        GameObject playerGO = ArmadilloPlayerController.Instance.gameObject;
        if (enemyControl.CheckForLOS(playerGO))
        {
            enemyControl.IncreaseDetection();
            lastKnownPlayerPos = playerGO.transform.position;
        }
        else
        {
            enemyControl.DecreaseDetection();
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
        enemyControl.navMeshAgent.isStopped = false;
        enemyControl.navMeshAgent.updateRotation = true;
    }
}
