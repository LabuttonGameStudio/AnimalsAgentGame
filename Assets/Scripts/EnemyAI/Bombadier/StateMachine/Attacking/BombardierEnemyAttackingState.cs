using System.Collections;
using UnityEngine;
using UnityEngine.AI;

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
        if (attackRoutine_Ref != null) iEnemy.StopCoroutine(attackRoutine_Ref);
        attackRoutine_Ref = iEnemy.StartCoroutine(AttackRoutine_Coroutine());
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
    private Coroutine attackRoutine_Ref;
    private IEnumerator AttackRoutine_Coroutine()
    {
        //Move to player 
        iEnemy.animator.SetBool("isWalking", true);
        if (iEnemy.TrySetNextDestination(new Vector3(iEnemy.lastKnownPlayerPos.x, iEnemy.transform.position.y, iEnemy.lastKnownPlayerPos.z)))
        {
            yield return iEnemy.CheckForProximityOfPoint();
            //Drop bomb

        }
        yield return new WaitForFixedUpdate();
    }
}
