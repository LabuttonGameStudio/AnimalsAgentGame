using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySearchingState : MeleeEnemyState
{
    public EnemySearchingState(EnemyMelee enemyCtrl) : base(enemyCtrl)
    {
        enemyControl = enemyCtrl;
    }
    public override void OnVisibilityUpdate()
    {
        GameObject playerGO = ArmadilloPlayerController.Instance.gameObject;
        if (enemyControl.CheckForLOS(playerGO))
        {
            enemyControl.IncreaseDetection();
            enemyControl.lastKnownPlayerPos = playerGO.transform.position;
        }
        else
        {
            enemyControl.DecreaseDetection();
        }

    }

    public override void OnActionUpdate()
    {
        enemyControl.SetNextDestinationOfNavmesh(enemyControl.lastKnownPlayerPos);
        enemyControl.transform.LookAt(new Vector3(enemyControl.lastKnownPlayerPos.x,enemyControl.transform.position.y, enemyControl.lastKnownPlayerPos.z));
    }
    public override void OnEnterState()
    {
        enemyControl.navMeshAgent.isStopped = false;
        enemyControl.navMeshAgent.updateRotation = false;
        enemyControl.SetNextDestinationOfNavmesh(enemyControl.lastKnownPlayerPos);
        enemyControl.navMeshAgent.stoppingDistance = 10;
    }

    public override void OnExitState()
    {
        enemyControl.navMeshAgent.isStopped = true;
        enemyControl.navMeshAgent.updateRotation = true;
        enemyControl.navMeshAgent.stoppingDistance = 0;
    }

    public override void OnFixedUpdate()
    {
        Vector3 direction = enemyControl.lastKnownPlayerPos;
        direction.y = 0;
        direction -= enemyControl.transform.position;
        Quaternion lookAtRotation = Quaternion.LookRotation(direction);
        enemyControl.transform.rotation = Quaternion.Lerp(enemyControl.transform.rotation, lookAtRotation, 4 * Time.fixedDeltaTime);
    }
}
