using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySearchingState : MeleeEnemyState
{
    public EnemySearchingState(EnemyMelee enemyCtrl) : base(enemyCtrl)
    {
        iEnemy = enemyCtrl;
    }
    public override void OnVisibilityUpdate()
    {
        if (iEnemy.CheckForPlayerLOS() > 0)
        {
            //iEnemy.IncreaseDetection();
            iEnemy.lastKnownPlayerPos = ArmadilloPlayerController.Instance.transform.position;
        }
        else
        {
            //iEnemy.DecreaseDetection();
        }

    }

    public override void OnActionUpdate()
    {
        //iEnemy.SetNextDestinationOfNavmesh(iEnemy.lastKnownPlayerPos);
        //iEnemy.transform.LookAt(new Vector3(iEnemy.lastKnownPlayerPos.x, iEnemy.transform.position.y, iEnemy.lastKnownPlayerPos.z));
    }
    public override void OnEnterState()
    {
        //iEnemy.navMeshAgent.isStopped = false;
        //iEnemy.navMeshAgent.updateRotation = false;
        //iEnemy.SetNextDestinationOfNavmesh(iEnemy.lastKnownPlayerPos);
        //iEnemy.navMeshAgent.stoppingDistance = 10;
    }

    public override void OnExitState()
    {
        //iEnemy.navMeshAgent.isStopped = true;
       // iEnemy.navMeshAgent.updateRotation = true;
        //iEnemy.navMeshAgent.stoppingDistance = 0;
    }

    public override void OnFixedUpdate() 
    {
        //Vector3 direction = iEnemy.lastKnownPlayerPos;
        //direction.y = 0;
        //direction -= iEnemy.transform.position;
        //Quaternion lookAtRotation = Quaternion.LookRotation(direction);
        //iEnemy.transform.rotation = Quaternion.Lerp(iEnemy.transform.rotation, lookAtRotation, 4 * Time.fixedDeltaTime);
    }
}
