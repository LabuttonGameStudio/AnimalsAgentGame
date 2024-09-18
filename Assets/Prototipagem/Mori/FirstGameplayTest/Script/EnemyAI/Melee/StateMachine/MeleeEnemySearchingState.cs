using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MeleeEnemySearchingState : MeleeEnemyState
{
    private enum ObservingStates
    {
        Null,
        Tracking,
        LookingAround
    }
    private ObservingStates currentState;
    public MeleeEnemySearchingState(EnemyMelee enemyCtrl) : base(enemyCtrl)
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
            iEnemy.ResetTickInterval();
        }

    }

    public override void OnActionUpdate()
    {

    }
    public override void OnEnterState()
    {
        iEnemy.ToggleAlert(true);
        iEnemy.navMeshAgent.isStopped = false;
    }

    public override void OnExitState()
    {
        StopLookAround();
        StopTracking();
        iEnemy.heardPlayer = false;
    }

    public override void OnFixedUpdate()
    {
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
        iEnemy.currentAction = EnemyMelee.Actions.Observing;
        iEnemy.navMeshAgent.ResetPath();
        float timer = 0;
        while(timer<1)
        {
            iEnemy.LerpLookAt(iEnemy.lastKnownPlayerPos, 3);
            timer += Time.deltaTime;
            yield return null;
        }
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
            iEnemy.currentAction = EnemyMelee.Actions.Moving;
            iEnemy.animator.SetBool("isWalking", true);
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
            iEnemy.currentAction = EnemyMelee.Actions.Observing;
            iEnemy.animator.SetBool("isWalking", false);
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
        iEnemy.currentAction = EnemyMelee.Actions.Moving;
        while (true)
        {
            Vector3 position = iEnemy.lastKnownPlayerPos;
            iEnemy.LerpLookAt(position, 5);
            Vector3 playerDirection = iEnemy.transform.position - position;
            playerDirection.y = 0;
            position = position + (playerDirection.normalized * 3f);
            iEnemy.TrySetNextDestination(position);
            iEnemy.animator.SetBool("isWalking", !iEnemy.CheckForProximityOfPoint());
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
