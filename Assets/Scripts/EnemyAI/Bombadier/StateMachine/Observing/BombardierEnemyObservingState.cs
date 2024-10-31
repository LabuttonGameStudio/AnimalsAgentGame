using System.Collections;
using UnityEngine;

public class BombardierEnemyObservingState : BombardierEnemyState
{
    public BombardierEnemyObservingState(EnemyBombardier enemyCtrl) : base(enemyCtrl)
    {
        iEnemy = enemyCtrl;
    }

    public override void OnActionUpdate()
    {

    }

    public override void OnEnterState()
    {
        iEnemy.currentAIBehaviour = AIBehaviourEnums.AIBehaviour.Observing;
        iEnemy.enemyBehaviourVisual.ChangeVisualState(AIBehaviourEnums.AIBehaviour.Observing);
        if (iEnemy.navMeshAgent.isActiveAndEnabled) iEnemy.navMeshAgent.ResetPath();
        iEnemy.animator.SetBool("isWalking", true);
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
            iEnemy.TrySetNextDestination(new Vector3(iEnemy.lastKnownPlayerPos.x,iEnemy.transform.position.y, iEnemy.lastKnownPlayerPos.z));
            yield return new WaitForFixedUpdate();
        }
    }
    private Coroutine onPlayerLeaveVision_Ref;
    private IEnumerator OnPlayerLeaveVision_Coroutine()
    {
        if(iEnemy.TrySetNextDestination(new Vector3(iEnemy.lastKnownPlayerPos.x, iEnemy.transform.position.y, iEnemy.lastKnownPlayerPos.z)))
        {
            yield return iEnemy.RoamingWaitToReachNextPoint();
        }
        iEnemy.animator.SetBool("isWalking", false);
        yield return new WaitForSeconds(2);
        onPlayerLeaveVision_Ref = null;
        iEnemy.detectionLevel = 0;
        iEnemy.ChangeCurrentState(iEnemy.enemyRoamingState);
    }
}
