using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyAttackingState : MeleeEnemyState
{
    public EnemyAttackingState(EnemyMelee enemyCtrl) : base(enemyCtrl)
    {
        iEnemy = enemyCtrl;
    }

    public override void OnVisibilityUpdate()
    {

    }

    public override void OnActionUpdate()
    {
        Vector3 playerPosRef = ArmadilloPlayerController.Instance.transform.position;
        if (!isAttackOnCooldown)
        {
            if (Vector3.Distance(iEnemy.transform.position, playerPosRef) <= attackRange+0.1f)
            {
                Attack();
            }
            else if (isMoving) GoToPlayer();
        }
        else
        {

            if (isMoving)
            {
                GoAwayFromPlayer();
            }
        }
    }
    private float attackRange = 5;
    private float minimalDistanceFromPlayer = 5;
    private void GoToPlayer()
    {
        Vector3 target = iEnemy.transform.position - ArmadilloPlayerController.Instance.transform.position;
        target.y = 0;
        target = Vector3.Normalize(target);
        target = target * minimalDistanceFromPlayer;
        target += ArmadilloPlayerController.Instance.transform.position;
        iEnemy.TrySetNextDestination(target);
    }
    private void GoAwayFromPlayer()
    {
        Vector3 target = iEnemy.transform.position - ArmadilloPlayerController.Instance.transform.position;
        target.y = 0;
        target += iEnemy.transform.right * Mathf.Sin(Time.time/2);
        target = Vector3.Normalize(target);
        target = target * minimalDistanceFromPlayer*1.5f;
        target += ArmadilloPlayerController.Instance.transform.position;
        iEnemy.TrySetNextDestination(target);
    }

    private bool isAttackOnCooldown;
    private bool isMoving = true;
    private bool isLookingAtPlayer = true;
    private void Attack()
    {
        isAttackOnCooldown = true;
        iEnemy.navMeshAgent.isStopped = true;
        isMoving = false;
        isLookingAtPlayer = true;
        iEnemy.StartCoroutine(Attack_Coroutine());
    }
    private IEnumerator Attack_Coroutine()
    {
        Vector3 attackDirection;
        iEnemy.navMeshAgent.velocity = Vector3.zero;
        yield return new WaitForSeconds(0.125f);
        attackDirection = ArmadilloPlayerController.Instance.transform.position - iEnemy.transform.position;
        attackDirection.y = 0;
        attackDirection.Normalize();
        isLookingAtPlayer = false;
        yield return new WaitForSeconds(0.125f);
        iEnemy.rb.AddForce(attackDirection * 25f, ForceMode.Impulse);
        float timer = 0;
        Vector3 bottomCapsulePoint;
        Vector3 topCapsulePoint;
        while (timer < 1)
        {
            bottomCapsulePoint = iEnemy.transform.position - new Vector3(0, iEnemy.navMeshAgent.height / 2, 0);
            topCapsulePoint = iEnemy.transform.position + new Vector3(0, iEnemy.navMeshAgent.height / 2, 0);
            RaycastHit[] raycastHits = Physics.CapsuleCastAll(bottomCapsulePoint, topCapsulePoint, iEnemy.navMeshAgent.radius + 0.5f, Vector3.down);
            foreach (RaycastHit hit in raycastHits)
            {
                if (hit.collider.CompareTag("Player"))
                {
                    ArmadilloPlayerController.Instance.hpControl.TakeDamage(Mathf.RoundToInt(iEnemy.primaryAttackDamage));
                    iEnemy.rb.velocity = iEnemy.rb.velocity * -1;
                    timer = 1;
                    break;
                }
            }
            timer += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }
        isLookingAtPlayer = true;
        isMoving = true;
        iEnemy.navMeshAgent.isStopped = false;
        yield return new WaitForSeconds(4f);
        isAttackOnCooldown = false;
    }
    public override void OnEnterState()
    {
        //iEnemy.navMeshAgent.speed = iEnemy.navMeshAgent.speed * 1.5f;
        iEnemy.navMeshAgent.stoppingDistance = 0;
        iEnemy.navMeshAgent.isStopped = false;
        iEnemy.navMeshAgent.updateRotation = false;
    }

    public override void OnExitState()
    {
        iEnemy.navMeshAgent.isStopped = true;
        iEnemy.navMeshAgent.updateRotation = true;
    }

    public override void OnFixedUpdate()
    {
        if (isLookingAtPlayer)
        {
            Vector3 direction = ArmadilloPlayerController.Instance.transform.position;
            direction.y = 0;
            direction -= iEnemy.transform.position;
            Quaternion lookAtRotation = Quaternion.LookRotation(direction);
            iEnemy.transform.rotation = Quaternion.Lerp(iEnemy.transform.rotation, lookAtRotation, 6 * Time.fixedDeltaTime);
        }
    }
}
