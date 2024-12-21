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
        iEnemy.SetVelocity(AIBehaviourEnums.AIBehaviour.Attacking);
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
        iEnemy.lastKnownPlayerPos = ArmadilloPlayerController.Instance.transform.position;
        //Debug.Log("Attacking FixedUpdate");
    }

    public override void OnVisibilityUpdate()
    {

    }
    private Coroutine attackRoutine_Ref;
    private IEnumerator AttackRoutine_Coroutine()
    {
        while (true)
        {
            //Move to player 
            iEnemy.animator.SetBool("isWalking", true);
            while (GetDistanceFromPlayer() > 1f)
            {
                Move(ArmadilloPlayerController.Instance.transform.position);
                yield return new WaitForFixedUpdate();
            }

            //Drop bomb
            iEnemy.navMeshAgent.isStopped = true;
            iEnemy.animator.SetBool("isAlerted", true);
            iEnemy.bombardierBomb.transform.position = iEnemy.firePivot.transform.position;
            iEnemy.bombardierBomb.Enable();

            yield return new WaitForSeconds(1);
            iEnemy.animator.SetBool("isAlerted", false);
            iEnemy.navMeshAgent.isStopped = false;

            yield return new WaitForSeconds(4);
        }
    }
    private void Move(Vector3 target)
    {
        target.y = iEnemy.transform.position.y;

        NavMeshPath navMeshPath = new NavMeshPath();
        iEnemy.navMeshAgent.CalculatePath(target, navMeshPath);
        iEnemy.navMeshAgent.SetPath(navMeshPath);
    }
    private float GetDistanceFromPlayer()
    {
        Vector3 distance = iEnemy.transform.position - iEnemy.lastKnownPlayerPos;
        distance.y = 0;
        return distance.magnitude;
    }
}
