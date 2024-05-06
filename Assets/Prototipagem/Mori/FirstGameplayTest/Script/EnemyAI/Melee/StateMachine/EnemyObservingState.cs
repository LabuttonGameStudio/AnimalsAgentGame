using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using static IEnemy;

public class EnemyObservingState : MeleeEnemyState
{

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

    }

    public override void OnFixedUpdate()
    {

    }
    private void OnPlayerInLOS()
    {
        if (LookAtPlayer_Ref == null)
        {
            Debug.Log("Start");
            iEnemy.BreakOnWaitPointCoroutine();
            LostLOSOfPlayer_Ref = null;
            LookAtPlayer_Ref = iEnemy.StartCoroutine(LookAtPlayer_Coroutine());
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
    private Coroutine LostLOSOfPlayer_Ref;
    private IEnumerator LostLOSOfPlayer_Coroutine()
    {
        iEnemy.waitOnPointTimer_Ref = iEnemy.StartCoroutine(iEnemy.WaitOnPointTimer_Coroutine(5, true));
        yield return iEnemy.waitOnPointTimer_Ref;
        LostLOSOfPlayer_Ref = null;
        iEnemy.ChangeCurrentAIBehaviour(AIBehaviourEnums.AIBehaviour.Roaming);
    }
    private Coroutine LookAtPlayer_Ref;
    private IEnumerator LookAtPlayer_Coroutine()
    {
        while (true)
        {
            Vector3 direction = iEnemy.lastKnownPlayerPos;
            iEnemy.LerpLookAt(direction);
            yield return new WaitForFixedUpdate();
        }
    }
}
