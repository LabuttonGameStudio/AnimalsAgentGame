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
            iEnemy.IncreaseDetection();
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
        StopLookAround();
        StopTracking();
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
            Debug.Log("Searching_InLos");
            currentState = ObservingStates.Tracking;
            StopLookAround();
            tracking_Ref = iEnemy.StartCoroutine(Tracking_Coroutine());
        }
    }

    private void OnPlayerOutOfLOS()
    {
        if (currentState != ObservingStates.LookingAround)
        {
            Debug.Log("Searching_OutLos");
            currentState = ObservingStates.LookingAround;
            StopTracking();
            walkAround_Ref = iEnemy.StartCoroutine(WalkAround_Coroutine());
        }
    }
    #region Walk Around
    Vector2 lastRandomPos;
    private bool TryRandomPatrol(Vector3 startPos, Vector2 distance)
    {
        float randomX;
        float randomZ;
        while (true)
        {
            randomX = (Random.value * 2) - 1;
            randomZ = (Random.value * 2) - 1;
            if (Vector2.Distance(new Vector2(randomX, randomZ), lastRandomPos) > 0.5f)
            {
                lastRandomPos = new Vector2(randomX, randomZ);
                break;
            }
        }
        Vector2 direction = new Vector2(randomX * distance.x, randomZ * distance.y);
        if (NavMesh.SamplePosition(startPos + new Vector3(direction.x, 0, direction.y), out NavMeshHit navMeshHit, 5f, NavMesh.AllAreas))
        {
            return iEnemy.TrySetNextDestination(navMeshHit.position);
        }
        return false;
    }

    private Coroutine walkAround_Ref;
    private IEnumerator WalkAround_Coroutine()
    {
        iEnemy.navMeshAgent.ResetPath();
        yield return new WaitForSeconds(1f);
        Vector3 startPos = iEnemy.lastKnownPlayerPos;

        for (int i = 0; i <= 3; i++)
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
                iEnemy.LerpLookAt(movingDirection, 1f);
                if (iEnemy.CheckForProximityOfPoint())
                {
                    break;
                }
                yield return null;
            }
            Debug.Log("Finish Search" + i);
            yield return new WaitForSeconds(1);
        }
        yield return new WaitForSeconds(1);
        walkAround_Ref = null;
        iEnemy.SetDetectionLevel(0);
        iEnemy.ChangeCurrentAIBehaviour(AIBehaviourEnums.AIBehaviour.Roaming);
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
            Vector3 playerDirection = iEnemy.transform.position - position;
            playerDirection.y = 0;
            position = position + (playerDirection.normalized * 3f);
            iEnemy.TrySetNextDestination(position);
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
