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
            //iEnemy.IncreaseDetection();
        }
        else
        {
            OnPlayerOutOfLOS();
            iEnemy.DecreaseDetection();
        }
    }

    public override void OnActionUpdate()
    {

    }
    public override void OnEnterState()
    {
        iEnemy.navMeshAgent.ResetPath();
    }

    public override void OnExitState()
    {
        iEnemy.BreakOnWaitPointCoroutine();
        if (LookAtPlayer_Ref != null)
        {
            iEnemy.StopCoroutine(LookAtPlayer_Ref);
            LookAtPlayer_Ref = null;
        }

        if (LostLOSOfPlayer_Ref != null)
        {
            iEnemy.StopCoroutine(LostLOSOfPlayer_Ref);
            LostLOSOfPlayer_Ref = null;
        }

    }

    public override void OnFixedUpdate()
    {

    }
    private void OnPlayerInLOS()
    {
        if(currentState != ObservingStates.Tracking)
        {
            currentState = ObservingStates.Tracking;
            StopLookAround();
            tracking_Ref = iEnemy.StartCoroutine(Tracking_Coroutine());
        }
    }

    private void OnPlayerOutOfLOS()
    {
        if (LostLOSOfPlayer_Ref == null)
        {
            if (LookAtPlayer_Ref != null)
            {
                iEnemy.StopCoroutine(LookAtPlayer_Ref);
                LookAtPlayer_Ref = null;
            }
            LostLOSOfPlayer_Ref = iEnemy.StartCoroutine(LostLOSOfPlayer_Coroutine());
        }
    }
    #region Look Around
    private Coroutine lookAround_Ref;
    private IEnumerator LookAround_Coroutine()
    {
        iEnemy.waitOnPointTimer_Ref = iEnemy.StartCoroutine(iEnemy.WaitOnPointTimer_Coroutine(5, true));
        yield return iEnemy.waitOnPointTimer_Ref;
        LostLOSOfPlayer_Ref = null;
        iEnemy.ChangeCurrentAIBehaviour(AIBehaviourEnums.AIBehaviour.Roaming);
    }
    private void StopLookAround()
    {

    }
    #endregion

    #region Tracking
    private Coroutine tracking_Ref;
    private IEnumerator Tracking_Coroutine()
    {
        while (true)
        {
            Vector3 direction = iEnemy.lastKnownPlayerPos;
            iEnemy.LerpLookAt(direction);
            yield return new WaitForFixedUpdate();
        }
    }
    private void StopTracking()
    {
        if(tracking_Ref != null)
        {
            iEnemy.StopCoroutine(tracking_Ref);
            tracking_Ref = null;
        }
    }
    #endregion
}
