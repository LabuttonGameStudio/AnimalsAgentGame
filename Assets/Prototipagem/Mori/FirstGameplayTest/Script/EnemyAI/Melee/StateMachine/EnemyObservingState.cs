using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using static IEnemy;

public class EnemyObservingState : MeleeEnemyState
{
    private enum ObservingStates
    {
        Null,
        Tracking,
        LookingAround
    }
    private ObservingStates currentState;
    public EnemyObservingState(EnemyMelee enemyCtrl) : base(enemyCtrl)
    {
        iEnemy = enemyCtrl;
    }

    public override void OnVisibilityUpdate()
    {
        if (iEnemy.CheckForPlayerLOS() > 0)
        {
            iEnemy.lastKnownPlayerPos = ArmadilloPlayerController.Instance.transform.position;
            OnPlayerInLOS();
            iEnemy.IncreaseDetection();
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
        iEnemy.navMeshAgent.isStopped = false;
        iEnemy.navMeshAgent.ResetPath();
    }

    public override void OnExitState()
    {
        StopLookAround();
        StopTracking();
        
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
            lookAround_Ref = iEnemy.StartCoroutine(LookAround_Coroutine());
        }
    }
    #region Look Around
    private Coroutine lookAround_Ref;
    private IEnumerator LookAround_Coroutine()
    {
        iEnemy.animator.SetBool("isWalking", false);
        tracking_Ref = iEnemy.StartCoroutine(Tracking_Coroutine());
        yield return new WaitForSeconds(2);
        StopTracking();
        if (iEnemy.TryStartRandomLookAround(5, out Coroutine coroutine))
        {
            yield return coroutine;
        }
        iEnemy.SetDetectionLevel(0);
        iEnemy.ChangeCurrentAIBehaviour(AIBehaviourEnums.AIBehaviour.Roaming);
        lookAround_Ref = null;
    }
    private void StopLookAround()
    {
        iEnemy.StopLookAround();
        if(lookAround_Ref != null)
        {
            iEnemy.StopCoroutine(lookAround_Ref);
            lookAround_Ref = null;
        }
    }
    #endregion

    #region Tracking
    private Coroutine tracking_Ref;
    private IEnumerator Tracking_Coroutine()
    {
        iEnemy.animator.SetBool("isWalking", false);
        while (true)
        {
            Vector3 position = iEnemy.lastKnownPlayerPos;
            iEnemy.LerpLookAt(position, 2);
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
