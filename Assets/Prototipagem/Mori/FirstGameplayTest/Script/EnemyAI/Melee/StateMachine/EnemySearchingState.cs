using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemySearchingState : MeleeEnemyState
{
    private enum ObservingStates
    {
        Null,
        Tracking,
        LookingAround
    }
    private ObservingStates currentState;
    public EnemySearchingState(EnemyMelee enemyCtrl) : base(enemyCtrl)
    {
        iEnemy = enemyCtrl;
    }
    public override void OnVisibilityUpdate()
    {
        if (iEnemy.CheckForPlayerLOS() > 0)
        {
            OnPlayerInLOS();
            iEnemy.lastKnownPlayerPos = ArmadilloPlayerController.Instance.transform.position;
        }
        else
        {
            OnPlayerOutOfLOS();
        }

    }

    public override void OnActionUpdate()
    {
        //iEnemy.SetNextDestinationOfNavmesh(iEnemy.lastKnownPlayerPos);
        //iEnemy.transform.LookAt(new Vector3(iEnemy.lastKnownPlayerPos.x, iEnemy.transform.position.y, iEnemy.lastKnownPlayerPos.z));
    }
    public override void OnEnterState()
    {
        iEnemy.ToggleAlert(true);
        iEnemy.navMeshAgent.isStopped = false;
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
    private void OnPlayerInLOS()
    {
        if (currentState != ObservingStates.Tracking)
        {
            currentState = ObservingStates.Tracking;
            StopLookAround();
            tracking_Ref = iEnemy.StartCoroutine(Tracking_Coroutine());
        }
    }

    private void OnPlayerOutOfLOS()
    {
        if (currentState != ObservingStates.LookingAround)
        {
            currentState = ObservingStates.LookingAround;
            StopTracking();
            walkAround_Ref = iEnemy.StartCoroutine(WalkAround_Coroutine());
        }
    }
    #region Walk Around

    private bool TryRandomPatrol(Vector3 startPos, Vector2 distance)
    {
        float randomX = (Random.value * 2) - 1;
        float randomZ = (Random.value * 2) - 1;
        Vector2 direction = new Vector3(randomX * distance.x, 0, randomZ * distance.y);
        if (NavMesh.SamplePosition(startPos + new Vector3(direction.x, 0, direction.y), out NavMeshHit navMeshHit, 1f, NavMesh.AllAreas))
        {
            return iEnemy.TrySetNextDestination(navMeshHit.position);
        }
        return false;
    }

    private Coroutine walkAround_Ref;
    private IEnumerator WalkAround_Coroutine()
    {
        yield return new WaitForSeconds(1);
        Vector3 startPos = iEnemy.lastKnownPlayerPos;

        for (int i = 0; i < 3; i++)
        {
            while (true)
            {
                if (TryRandomPatrol(startPos, Vector2.one * 5f))
                {
                    break;
                }
                yield return null;
            }
            while (true)
            {
                if (!iEnemy.navMeshAgent.pathPending)
                {
                    break;
                }
                yield return null;
            }
            while (true)
            {
                Vector3 movingDirection = iEnemy.navMeshAgent.velocity + iEnemy.transform.position;
                iEnemy.LerpLookAt(movingDirection, 2f);
                if (iEnemy.CheckForProximityOfPoint())
                {
                    break;
                }
                yield return null;
            }
            yield return new WaitForSeconds(1);
        }
        walkAround_Ref = null;
    }
    private void StopLookAround()
    {
        iEnemy.StopLookAround();
        if (walkAround_Ref != null)
        {
            iEnemy.StopCoroutine(walkAround_Ref);
            walkAround_Ref = null;
        }
    }
    #endregion

    #region Tracking
    private Coroutine tracking_Ref;
    private IEnumerator Tracking_Coroutine()
    {
        while (true)
        {
            Vector3 position = iEnemy.lastKnownPlayerPos;
            iEnemy.LerpLookAt(position, 5);
            yield return new WaitForFixedUpdate();
        }
    }
    private void StopTracking()
    {
        if (tracking_Ref != null)
        {
            iEnemy.StopCoroutine(tracking_Ref);
            tracking_Ref = null;
        }
    }
    #endregion
}
