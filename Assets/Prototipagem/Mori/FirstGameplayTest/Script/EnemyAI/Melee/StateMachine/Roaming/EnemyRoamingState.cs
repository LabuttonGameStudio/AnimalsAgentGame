using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static IEnemy;

public class EnemyRoamingState : MeleeEnemyState
{

    private MeleeRoaming_SubState currentSubState;

    private MeleeRoaming_MovingSubState movingSubState;
    private MeleeRoaming_StoppedSubState stoppedSubState;
    public EnemyRoamingState(EnemyMelee enemyCtrl) : base(enemyCtrl)
    {
        movingSubState = new MeleeRoaming_MovingSubState();
        stoppedSubState = new MeleeRoaming_StoppedSubState();
        stoppedSubState.FixedUpdate_Event = new UnityEngine.Events.UnityEvent<EnemyRoamingState>();
        iEnemy = enemyCtrl;
    }
    public override void OnVisibilityUpdate()
    {
        if (iEnemy.CheckForPlayerLOS() > 0)
        {
            iEnemy.lastKnownPlayerPos = ArmadilloPlayerController.Instance.transform.position;
            iEnemy.ChangeCurrentAIBehaviour(AIBehaviourEnums.AIBehaviour.Observing);
        }
    }

    public override void OnActionUpdate()
    {

    }

    public override void OnEnterState()
    {
        currentSubState = movingSubState;
        iEnemy.SetNextDestinationOfNavmesh(iEnemy.aiPathList[iEnemy.currentPathPoint].transformOfPathPoint.position);
    }

    public override void OnExitState()
    {
        iEnemy.BreakOnWaitPointCoroutine();
    }

    public override void OnFixedUpdate()
    {
        currentSubState.OnFixedUpdate(this);
    }
    public void ToggleMovement(bool state)
    {
        currentSubState = state ? movingSubState : stoppedSubState;
    }

    public void StopMovement()
    {
        stoppedSubState.FixedUpdate_Event = new UnityEngine.Events.UnityEvent<EnemyRoamingState>();
        stoppedSubState.FixedUpdate_Event.AddListener(stoppedSubState.CheckForEndOfCoroutine);
        currentSubState = stoppedSubState;
    }
}
