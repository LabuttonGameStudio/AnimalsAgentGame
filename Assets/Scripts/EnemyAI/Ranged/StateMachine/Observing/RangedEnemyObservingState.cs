using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedEnemyObservingState : RangedEnemyState
{
    public RangedEnemyObservingState(RangedEnemy enemyCtrl) : base(enemyCtrl)
    {
        iEnemy = enemyCtrl;
    }

    public override void OnActionUpdate()
    {

    }

    public override void OnEnterState()
    {
        iEnemy.currentStateEnum = RangedEnemy.CurrentStateEnum.observing;
        iEnemy.enemyBehaviourVisual.ChangeVisualState(AIBehaviourEnums.AIBehaviour.Observing);
        iEnemy.navMeshAgent.ResetPath();
        iEnemy.animator.SetBool("isWalking", false);
        if (onPlayerEnterVision_Ref != null)
        {
            iEnemy.StopCoroutine(onPlayerEnterVision_Ref);
            onPlayerEnterVision_Ref= null;
        }
        onPlayerEnterVision_Ref = iEnemy.StartCoroutine(OnPlayerEnterVision_Coroutine());
        //Debug.Log("Observing Enter");
    }

    public override void OnExitState()
    {
        if (onPlayerEnterVision_Ref != null)
        {
            iEnemy.StopCoroutine(onPlayerEnterVision_Ref);
            //onPlayerEnterVision_Ref = null;
        }
        if (onPlayerLeaveVision_Ref != null)
        {
            iEnemy.StopCoroutine(onPlayerLeaveVision_Ref);
            //onPlayerLeaveVision_Ref = null;
        }
        Debug.Log("Observing Exit");
    }

    public override void OnFixedUpdate()
    {
        Debug.Log("Observing FixedUpdate");
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
        yield return new WaitForSeconds(2);
        if (iEnemy.TryStartRandomLookAround(3, out Coroutine lookAroundCoroutine))
        {
            yield return lookAroundCoroutine;
        }
        onPlayerLeaveVision_Ref = null;
        iEnemy.detectionLevel = 0;
        iEnemy.ChangeCurrentState(iEnemy.enemyRoamingState);
    }
}
