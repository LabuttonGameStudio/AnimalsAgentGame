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
        if (!isAttackOnCooldown)
        {
            if (Vector3.Distance(enemyControl.transform.position, playerPosRef) <= attackRange+0.1f)
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
        Debug.Log("Go to player");
        Vector3 target = enemyControl.transform.position - ArmadilloPlayerController.Instance.transform.position;
        target.y = 0;
        target = Vector3.Normalize(target);
        target = target * minimalDistanceFromPlayer;
        target += ArmadilloPlayerController.Instance.transform.position;
        enemyControl.SetNextDestinationOfNavmesh(target);
    }
    private void GoAwayFromPlayer()
    {
        Debug.Log("Go away from player");
        Vector3 target = enemyControl.transform.position - ArmadilloPlayerController.Instance.transform.position;
        target.y = 0;
        target += enemyControl.transform.right * Mathf.Sin(Time.time/2);
        target = Vector3.Normalize(target);
        target = target * minimalDistanceFromPlayer*1.5f;
        target += ArmadilloPlayerController.Instance.transform.position;
        enemyControl.SetNextDestinationOfNavmesh(target);
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
        while (timer < 1)
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
        isLookingAtPlayer = true;
        isMoving = true;
        enemyControl.navMeshAgent.isStopped = false;
        yield return new WaitForSeconds(4f);
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
