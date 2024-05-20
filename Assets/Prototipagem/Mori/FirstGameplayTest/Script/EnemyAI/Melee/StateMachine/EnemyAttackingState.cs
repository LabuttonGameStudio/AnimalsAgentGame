using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;

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
        iEnemy.lastKnownPlayerPos = ArmadilloPlayerController.Instance.transform.position;
    }
    private float GetDistanceFromPlayer()
    {
        Vector3 distance = iEnemy.transform.position - iEnemy.lastKnownPlayerPos;
        distance.y = 0;
        return distance.magnitude;
    }
    private void StartMovingToPlayer()
    {
        movingToPlayer_Ref = iEnemy.StartCoroutine(MovingToPlayer_Coroutine(GetCurrentAttackRange()));
    }
    private Coroutine movingToPlayer_Ref;
    private IEnumerator MovingToPlayer_Coroutine(float desiredDistanceFromPlayer)
    {
        while (true)
        {
            iEnemy.animator.SetBool("isWalking", true);
            Move(desiredDistanceFromPlayer);
            if (GetDistanceFromPlayer() < desiredDistanceFromPlayer+0.25f)
            {
                StopMovingToPlayer();
                StartAttack();
                yield break;
            }
            yield return new WaitForFixedUpdate();
        }
    }
    private void StopMovingToPlayer()
    {
        if (movingToPlayer_Ref != null)
        {
            iEnemy.animator.SetBool("isWalking", false);
            iEnemy.StopCoroutine(movingToPlayer_Ref);
            movingToPlayer_Ref = null;
        }
    }
    private void Move(float distanceFromPlayer)
    {
        iEnemy.LerpLookAt(iEnemy.lastKnownPlayerPos, 2f);
        Vector3 target = iEnemy.transform.position - iEnemy.lastKnownPlayerPos;
        target.y = 0;
        target = target.normalized;
        target = target * distanceFromPlayer;
        target += ArmadilloPlayerController.Instance.transform.position;
        if (NavMesh.SamplePosition(target, out NavMeshHit navMeshHit, 2f, NavMesh.AllAreas))
        {
            iEnemy.TrySetNextDestination(navMeshHit.position);
        }
        else
        {
            if (NavMesh.SamplePosition(target, out NavMeshHit navMeshHit2, 5f, NavMesh.AllAreas))
            {
                iEnemy.TrySetNextDestination(navMeshHit2.position);
            }
        }
    }

    private void StartAttack()
    {
        switch (attackCycle)
        {
            case 0:
            case 1:
                attack_Coroutine = iEnemy.StartCoroutine(PrimaryAttack_Coroutine());
                break;
            case 2:
                attack_Coroutine = iEnemy.StartCoroutine(SecondaryAttack_Coroutine());
                break;
        }
    }
    private int attackCycle;
    private void NextAttackCycle()
    {
        if (attackCycle + 1 > 2)
        {
            attackCycle = 0;
        }
        else attackCycle++;
    }
    private float GetCurrentAttackRange()
    {
        switch (attackCycle)
        {
            default:
            case 0:
            case 1:
                return 2;
            case 2:
                return 5;
        }
    }
    private Coroutine attack_Coroutine;
    private IEnumerator PrimaryAttack_Coroutine()
    {
        iEnemy.animator.SetTrigger("lightPunch");
        iEnemy.primaryAttackHitbox.EnableHitBox();
        Damage damage = new Damage(iEnemy.primaryAttackDamage, Damage.DamageType.Blunt, false, iEnemy.transform.position);
        yield return new WaitForSeconds(0.4f);
        iEnemy.primaryAttackHitbox.DealDamageToThingsInside(damage);
        yield return new WaitForSeconds(0.5f);
        iEnemy.primaryAttackHitbox.DisableHitBox();

        attack_Coroutine = null;
        StartMovingAwayFromPlayer();
    }
    private IEnumerator SecondaryAttack_Coroutine()
    {
        float timer = 0;
        iEnemy.animator.SetTrigger("strongPunch");
        while (timer < 0.875f)
        {
            iEnemy.LerpLookAt(iEnemy.lastKnownPlayerPos, 2f);
            timer += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }
        iEnemy.navMeshAgent.velocity = Vector3.zero;
        iEnemy.navMeshAgent.enabled = false;
        Vector3 attackDirection;
        attackDirection = iEnemy.lastKnownPlayerPos - iEnemy.transform.position;
        attackDirection.y = 0;
        attackDirection.Normalize();
        iEnemy.secondaryAttackHitbox.EnableHitBox();
        yield return new WaitForSeconds(0.125f);

        iEnemy.rb.AddForce(attackDirection * 30f, ForceMode.VelocityChange);
        Damage damage = new Damage(iEnemy.secondaryAttackDamage, Damage.DamageType.Blunt, false, iEnemy.transform.position);
        timer = 0;
        while (timer < 1f)
        {
            iEnemy.secondaryAttackHitbox.DealDamageToThingsInside(damage);
            timer += Time.fixedDeltaTime;   
            yield return new WaitForFixedUpdate();
        }
        iEnemy.secondaryAttackHitbox.DisableHitBox();
        iEnemy.navMeshAgent.enabled = true;

        attack_Coroutine = null;
        StartMovingAwayFromPlayer();
    }

    private void StartMovingAwayFromPlayer()
    {
        movingAwayFromPlayer_Ref = iEnemy.StartCoroutine(MovingAwayFromPlayer_Coroutine());
    }
    private Coroutine movingAwayFromPlayer_Ref;
    private IEnumerator MovingAwayFromPlayer_Coroutine()
    {
        float timer=0;
        float duration=0;
        switch (attackCycle)
        {
            case 0:
            case 1:
                duration = iEnemy.primaryAttackCooldown;
                break;
            case 2:
                duration = iEnemy.secondaryAttackCooldown;
                break;
        }
        while (timer<duration)
        {
            Move(7.5f);
            iEnemy.animator.SetBool("isWalking", !iEnemy.CheckForProximityOfPoint());
            timer += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }
        NextAttackCycle();
        StartMovingToPlayer();
        StopMovingAwayFromPlayer();
    }
    private void StopMovingAwayFromPlayer()
    {
        if (movingAwayFromPlayer_Ref != null)
        {
            iEnemy.animator.SetBool("isWalking", false);
            iEnemy.StopCoroutine(movingAwayFromPlayer_Ref);
            movingAwayFromPlayer_Ref = null;
        }
    }
    public override void OnEnterState()
    {
        iEnemy.navMeshAgent.isStopped = false;
        StartMovingToPlayer();
    }

    public override void OnExitState()
    {

    }

    public override void OnFixedUpdate()
    {

    }
}
