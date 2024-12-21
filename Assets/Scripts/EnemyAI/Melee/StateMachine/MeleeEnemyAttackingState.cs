using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;

public class MeleeEnemyAttackingState : MeleeEnemyState
{
    public MeleeEnemyAttackingState(EnemyMelee enemyCtrl) : base(enemyCtrl)
    {
        iEnemy = enemyCtrl;
    }

    public override void OnEnterState()
    {
        attackCycle = 0;
        iEnemy.enemyBehaviourVisual.ChangeVisualState(AIBehaviourEnums.AIBehaviour.Attacking);
        iEnemy.navMeshAgent.isStopped = false;
        iEnemy.navMeshAgent.autoBraking = false;
        iEnemy.SetVelocity(AIBehaviourEnums.AIBehaviour.Attacking);
        enemyRoutine_Ref = iEnemy.StartCoroutine(EnemyRoutine_Coroutine());
    }
    public override void OnExitState()
    {

    }
    public override void OnFixedUpdate()
    {
        iEnemy.lastKnownPlayerPos = ArmadilloPlayerController.Instance.transform.position;
    }
    public override void OnVisibilityUpdate()
    {

    }
    public override void OnActionUpdate()
    {
    }
    #region Routine
    private Coroutine enemyRoutine_Ref;
    private IEnumerator EnemyRoutine_Coroutine()
    {
        while (true)
        {
            //Move into player
            float desiredDistanceFromPlayer = GetCurrentAttackRange();
            iEnemy.playHitAnimation = true;
            while (GetDistanceFromPlayer() >= desiredDistanceFromPlayer + 0.25f)
            {
                if (!NeedShield())
                {
                    Move(desiredDistanceFromPlayer);
                }
                else
                {
                    iEnemy.animator.SetBool("isWalking", false);
                    iEnemy.navMeshAgent.isStopped = true;
                }
                yield return new WaitForFixedUpdate();
            }

            //Attack
            iEnemy.animator.SetBool("isWalking", false);
            yield return StartAttack();

            //Move away from player
            float timer = 0;
            float duration;
            switch (attackCycle)
            {
                default:
                case 0:
                case 1:
                    duration = iEnemy.primaryAttackCooldown + Random.Range(0f, 3f);
                    break;
                case 2:
                    duration = iEnemy.secondaryAttackCooldown + Random.Range(0f, 3f);
                    break;
            }
            iEnemy.playHitAnimation = true;
            while (timer < duration)
            {
                if (!NeedShield())
                {
                    Move(7.5f);
                    iEnemy.animator.SetBool("isWalking", !iEnemy.CheckForProximityOfPoint());
                    timer += Time.fixedDeltaTime;
                }
                else
                {
                    iEnemy.animator.SetBool("isWalking", false);
                    iEnemy.navMeshAgent.isStopped = true;
                }
                yield return new WaitForFixedUpdate();
            }
            NextAttackCycle();
            iEnemy.animator.SetBool("isWalking", false);
        }

    }

    #endregion

    #region Move
    private void Move(float distanceFromPlayer)
    {
        iEnemy.LerpLookAt(iEnemy.lastKnownPlayerPos, 2f);
        Vector3 target = iEnemy.transform.position - iEnemy.lastKnownPlayerPos;
        target.y = 0;
        target = target.normalized;
        target = target * distanceFromPlayer;
        target += ArmadilloPlayerController.Instance.transform.position;
        NavMeshHit navMeshHit;
        NavMeshPath navMeshPath = new NavMeshPath();
        if (NavMesh.SamplePosition(target, out navMeshHit, 2f, NavMesh.GetAreaFromName("Lobster")))
        {
            iEnemy.navMeshAgent.CalculatePath(navMeshHit.position, navMeshPath);
        }
        if (navMeshPath.status == NavMeshPathStatus.PathComplete)
        {
            iEnemy.navMeshAgent.SetPath(navMeshPath);
            iEnemy.animator.SetBool("isWalking", true);
            iEnemy.navMeshAgent.isStopped = false;
        }
        else
        {
            iEnemy.navMeshAgent.isStopped = true;
            iEnemy.animator.SetBool("isWalking", false);
        }
    }
    private float GetDistanceFromPlayer()
    {
        Vector3 distance = iEnemy.transform.position - iEnemy.lastKnownPlayerPos;
        distance.y = 0;
        return distance.magnitude;
    }
    #endregion

    #region Shield
    private bool NeedShield()
    {
        Vector3 target = ArmadilloPlayerController.Instance.movementControl.rb.position;
        NavMeshHit navMeshHit;
        NavMeshPath navMeshPath = new NavMeshPath();
        if (NavMesh.SamplePosition(target, out navMeshHit, 2f, NavMesh.GetAreaFromName("Lobster")))
        {
            iEnemy.navMeshAgent.CalculatePath(navMeshHit.position, navMeshPath);
        }
        iEnemy.ToggleShield(navMeshPath.status != NavMeshPathStatus.PathComplete);
        return navMeshPath.status != NavMeshPathStatus.PathComplete;
    }
    #endregion

    #region Attacks
    private int attackCycle;
    private Coroutine StartAttack()
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
        return attack_Coroutine;
    }
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
                return 3;
            case 2:
                return 5;
        }
    }

    private Coroutine attack_Coroutine;
    private IEnumerator PrimaryAttack_Coroutine()
    {
        iEnemy.playHitAnimation = false;
        iEnemy.animator.SetTrigger("lightPunch");
        iEnemy.primaryAttackHitbox.EnableHitBox();
        Damage damage = new Damage(iEnemy.primaryAttackDamage, Damage.DamageType.Blunt, false, iEnemy.transform.position);
        yield return new WaitForSeconds(0.4f);
        iEnemy.primaryAttackHitbox.DealDamageToThingsInside(damage);
        yield return new WaitForSeconds(0.5f);
        iEnemy.primaryAttackHitbox.DisableHitBox();

        attack_Coroutine = null;
    }
    private IEnumerator SecondaryAttack_Coroutine()
    {
        float timer = 0;
        iEnemy.playHitAnimation = false;
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
        iEnemy.rb.isKinematic = false;
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
        iEnemy.rb.isKinematic = true;

        attack_Coroutine = null;
    }
    #endregion
}
