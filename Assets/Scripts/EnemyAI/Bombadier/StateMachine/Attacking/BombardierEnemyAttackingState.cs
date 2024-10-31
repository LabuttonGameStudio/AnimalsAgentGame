using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombardierEnemyAttackingState : BombardierEnemyState
{
    public BombardierEnemyAttackingState(EnemyBombardier enemyCtrl) : base(enemyCtrl)
    {
        iEnemy = enemyCtrl;
    }

    public override void OnActionUpdate()
    {

    }

    public override void OnEnterState()
    {
        iEnemy.currentAIBehaviour = AIBehaviourEnums.AIBehaviour.Attacking;
        iEnemy.enemyBehaviourVisual.ChangeVisualState(AIBehaviourEnums.AIBehaviour.Attacking);
        iEnemy.animator.SetBool("isWalking", false);
        if (iEnemy.navMeshAgent.isActiveAndEnabled) iEnemy.navMeshAgent.ResetPath();

        //Debug.Log("Attacking Enter");
    }

    public override void OnExitState()
    {
        //Debug.Log("Attacking Exit");
    }

    public override void OnFixedUpdate()
    {
        //Debug.Log("Attacking FixedUpdate");
    }

    public override void OnVisibilityUpdate()
    {

    }
}
