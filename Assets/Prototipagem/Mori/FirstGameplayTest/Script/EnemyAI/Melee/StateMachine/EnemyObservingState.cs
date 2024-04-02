using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using static IEnemy;

public class EnemyObservingState : MeleeEnemyState
{
    public EnemyObservingState(EnemyMelee enemyCtrl) : base(enemyCtrl)
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

    public override void OnFixedUpdate()
    {
        Vector3 direction = enemyControl.lastKnownPlayerPos;
        direction.y = 0;
        direction -= enemyControl.transform.position;
        Quaternion lookAtRotation = Quaternion.LookRotation(direction);
        enemyControl.transform.rotation = Quaternion.Lerp(enemyControl.transform.rotation, lookAtRotation, 3 * Time.fixedDeltaTime);
    }
}
