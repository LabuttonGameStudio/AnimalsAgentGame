using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using static IEnemy;

public class MeleeEnemyObservingState : MeleeEnemyState
{
    public MeleeEnemyObservingState(EnemyMelee enemyCtrl) : base(enemyCtrl)
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
        iEnemy.navMeshAgent.ResetPath();
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
