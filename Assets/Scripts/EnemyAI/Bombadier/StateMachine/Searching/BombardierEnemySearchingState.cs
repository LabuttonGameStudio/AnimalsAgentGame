using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BombardierEnemySearchingState : BombardierEnemyState
{
    public BombardierEnemySearchingState(EnemyBombardier enemyCtrl) : base(enemyCtrl)
    {
        iEnemy = enemyCtrl;
    }

    public override void OnActionUpdate()
    {

    }

    public override void OnEnterState()
    {
        iEnemy.currentAIBehaviour = AIBehaviourEnums.AIBehaviour.Searching;
        iEnemy.enemyBehaviourVisual.ChangeVisualState(AIBehaviourEnums.AIBehaviour.Searching);
        if (iEnemy.navMeshAgent.isActiveAndEnabled) iEnemy.navMeshAgent.ResetPath();
        iEnemy.animator.SetBool("isWalking", false);
        if (onPlayerEnterVision_Ref != null)
        {
            iEnemy.StopCoroutine(onPlayerEnterVision_Ref);
            onPlayerEnterVision_Ref = null;
        }
        onPlayerEnterVision_Ref = iEnemy.StartCoroutine(OnPlayerEnterVision_Coroutine());
    }

    public override void OnExitState()
    {
        if (onPlayerEnterVision_Ref != null)
        {
            iEnemy.StopCoroutine(onPlayerEnterVision_Ref);
        }
        if (onPlayerLeaveVision_Ref != null)
        {
            iEnemy.StopCoroutine(onPlayerLeaveVision_Ref);
        }
    }

    public override void OnFixedUpdate()
    {

    }

    public override void OnVisibilityUpdate()
    {
        if (iEnemy.CheckForPlayerLOS() > 0)
        {
            iEnemy.lastKnownPlayerPos = ArmadilloPlayerController.Instance.transform.position;
            iEnemy.IncreaseDetection();

            if (onPlayerEnterVision_Ref == null)
            {
                if (onPlayerLeaveVision_Ref != null)
                {
                    iEnemy.StopCoroutine(onPlayerLeaveVision_Ref);
                    onPlayerLeaveVision_Ref = null;
                }

                onPlayerEnterVision_Ref = iEnemy.StartCoroutine(OnPlayerEnterVision_Coroutine());
            }
        }
        else
        {
            if (onPlayerLeaveVision_Ref == null)
            {
                if (onPlayerEnterVision_Ref != null)
                {
                    iEnemy.StopCoroutine(onPlayerEnterVision_Ref);
                    onPlayerEnterVision_Ref = null;
                }

                onPlayerLeaveVision_Ref = iEnemy.StartCoroutine(OnPlayerLeaveVision_Coroutine());
            }
        }
    }

    private Coroutine onPlayerEnterVision_Ref;
    private IEnumerator OnPlayerEnterVision_Coroutine()
    {
        while (true)
        {
            iEnemy.LerpLookAt(iEnemy.lastKnownPlayerPos, 1);
            yield return new WaitForFixedUpdate();
        }
    }
    private Coroutine onPlayerLeaveVision_Ref;
    private IEnumerator OnPlayerLeaveVision_Coroutine()
    {
        if (iEnemy.navMeshAgent.isActiveAndEnabled) iEnemy.navMeshAgent.ResetPath();
        float timer = 0;
        while (timer < 1)
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
                yield return new WaitForFixedUpdate();
            }
            while (true)
            {
                if (!iEnemy.navMeshAgent.pathPending)
                {
                    break;
                }
                yield return new WaitForFixedUpdate();
            }
            iEnemy.animator.SetBool("isWalking", true);
            while (true)
            {
                Vector3 movingDirection = iEnemy.navMeshAgent.velocity + iEnemy.transform.position;
                iEnemy.LerpLookAt(movingDirection, 1f);
                if (iEnemy.CheckForProximityOfPoint())
                {
                    break;
                }
                yield return new WaitForFixedUpdate();
            }
            iEnemy.animator.SetBool("isWalking", false);
            yield return new WaitForSeconds(1);
        }
        yield return new WaitForSeconds(1);
        iEnemy.detectionLevel = 0;
        onPlayerLeaveVision_Ref = null;
        iEnemy.ChangeCurrentState(iEnemy.enemyRoamingState);
    }
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
}
