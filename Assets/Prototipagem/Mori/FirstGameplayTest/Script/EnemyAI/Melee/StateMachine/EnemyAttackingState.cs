using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyAttackingState : MeleeEnemyState
{
    public EnemyAttackingState(EnemyMelee enemyCtrl) : base(enemyCtrl)
    {
        enemyControl = enemyCtrl;
    }

    public override void OnVisibilityUpdate()
    {

    }

    public override void OnActionUpdate()
    {
        Vector3 playerPosRef = ArmadilloPlayerController.Instance.transform.position;
        if (!isAttackOnCooldown && Vector3.Distance(enemyControl.transform.position, playerPosRef) < attackRange)
        { 
            Attack();
        }
        else
        {
            if (isMoving) CircleAroundPlayer();
        }
    }
    private float attackRange = 5;
    private float minimalDistanceFromPlayer = 5;
    private void CircleAroundPlayer()
    {
        Vector3 target = ArmadilloPlayerController.Instance.transform.position;
        Vector3 goToPosition = new Vector3
            (
            target.x + minimalDistanceFromPlayer * Mathf.Cos(2 * Mathf.PI),
            target.y,
            target.z + minimalDistanceFromPlayer * Mathf.Sin(2 * Mathf.PI)
            );
        enemyControl.SetNextDestinationOfNavmesh(goToPosition);
    }

    private bool isAttackOnCooldown;
    private bool isMoving = true;
    private bool isLookingAtPlayer = true;
    private void Attack()
    {
        isAttackOnCooldown = true;
        enemyControl.navMeshAgent.isStopped = true;
        isMoving = false;
        isLookingAtPlayer = true;
        enemyControl.StartCoroutine(Attack_Coroutine());
    }
    private IEnumerator Attack_Coroutine()
    {
        Vector3 attackDirection;
        enemyControl.navMeshAgent.velocity = Vector3.zero;
        yield return new WaitForSeconds(0.125f);
        attackDirection = ArmadilloPlayerController.Instance.transform.position - enemyControl.transform.position;
        attackDirection.y = 0;
        attackDirection.Normalize();
        isLookingAtPlayer = false;
        yield return new WaitForSeconds(0.125f);
        enemyControl.rb.AddForce(attackDirection * 25f, ForceMode.Impulse);
        float timer = 0;
        Vector3 bottomCapsulePoint;
        Vector3 topCapsulePoint;
        while (timer < 0.75f)
        {
            bottomCapsulePoint = enemyControl.transform.position - new Vector3(0, enemyControl.navMeshAgent.height / 2, 0);
            topCapsulePoint = enemyControl.transform.position + new Vector3(0, enemyControl.navMeshAgent.height / 2, 0);
            RaycastHit[] raycastHits = Physics.CapsuleCastAll(bottomCapsulePoint, topCapsulePoint, enemyControl.navMeshAgent.radius + 0.5f, Vector3.down);
            foreach (RaycastHit hit in raycastHits)
            {
                if (hit.collider.CompareTag("Player"))
                {
                    ArmadilloPlayerController.Instance.hpControl.TakeDamage(enemyControl.hitDamage);
                    enemyControl.rb.velocity = enemyControl.rb.velocity * -1;
                    timer = 1;
                    break;
                }
            }
            timer += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }
        yield return new WaitForSeconds(1f);
        isLookingAtPlayer = true;
        isMoving = true;
        enemyControl.navMeshAgent.isStopped = false;
        yield return new WaitForSeconds(3f);
        isAttackOnCooldown = false;
    }
    public override void OnEnterState()
    {
        //enemyControl.navMeshAgent.speed = enemyControl.navMeshAgent.speed * 1.5f;
        enemyControl.navMeshAgent.stoppingDistance = 0;
        enemyControl.navMeshAgent.isStopped = false;
        enemyControl.navMeshAgent.updateRotation = false;
    }

    public override void OnExitState()
    {
        enemyControl.navMeshAgent.isStopped = true;
        enemyControl.navMeshAgent.updateRotation = true;
    }

    public override void OnFixedUpdate()
    {
        if (isLookingAtPlayer)
        {
            Vector3 direction = ArmadilloPlayerController.Instance.transform.position;
            direction.y = 0;
            direction -= enemyControl.transform.position;
            Quaternion lookAtRotation = Quaternion.LookRotation(direction);
            enemyControl.transform.rotation = Quaternion.Lerp(enemyControl.transform.rotation, lookAtRotation, 6 * Time.fixedDeltaTime);
        }
    }
}
