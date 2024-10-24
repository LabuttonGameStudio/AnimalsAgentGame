using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedEnemyAttackingState : RangedEnemyState
{
    public RangedEnemyAttackingState(RangedEnemy enemyCtrl) : base(enemyCtrl)
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
        iEnemy.navMeshAgent.ResetPath();
        iEnemy.animator.SetBool("isTurret", true);
        attackLoop_Ref = iEnemy.StartCoroutine(AttackLoop_Coroutine());
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
    private Coroutine attackLoop_Ref;
    private IEnumerator AttackLoop_Coroutine()
    {
        while (true)
        {
            for (int i = 0; i < 2; i++)
            {
                yield return WeakAttack_Coroutine();
                yield return iEnemy.LerpLookAt_Coroutine(ArmadilloPlayerController.Instance.transform, 2, 6 - 2.375f / 2);
            }
            yield return StrongAttack_Coroutine();
            yield return iEnemy.LerpLookAt_Coroutine(ArmadilloPlayerController.Instance.transform, 2, 1);
        }
    }

    private IEnumerator WeakAttack_Coroutine()
    {
        iEnemy.animator.SetBool("isLoadingSniper", true);
        yield return new WaitForSeconds(2.375f / 2);
        iEnemy.weakAttackLaser.enabled = true;
        float timer = 0;
        float trackDuration = 3f;
        Rigidbody playerRb = ArmadilloPlayerController.Instance.movementControl.rb;
        iEnemy.weakAttackLaser.ResetLaser();
        iEnemy.weakAttackLaser.gameObject.SetActive(true);
        iEnemy.weakAttackLaser.target = Vector3.zero;
        while (timer < trackDuration)
        {
            if (Physics.Raycast(iEnemy.firePivot.position, playerRb.position - iEnemy.firePivot.position, out RaycastHit hitInfo, 50, Physics.AllLayers, QueryTriggerInteraction.Ignore))
            {
                if (hitInfo.collider.CompareTag("Player"))
                {
                    iEnemy.weakAttackLaser.target = playerRb.position + playerRb.velocity / 10;
                }
                else
                {
                    iEnemy.weakAttackLaser.target = hitInfo.point;
                }
                iEnemy.LerpLookAt(iEnemy.weakAttackLaser.target, 20);
            }
            else
            {
                yield return new WaitForFixedUpdate();
            }
            timer += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }
        yield return new WaitForSeconds(0.25f);
        iEnemy.weakAttackLaser.gameObject.SetActive(false);
        iEnemy.weakAttackLaser.ResetLaser();
        Vector3 direction = iEnemy.weakAttackLaser.target - iEnemy.firePivot.position;
        direction.Normalize();
        iEnemy.animator.SetTrigger("sniperFire");
        iEnemy.animator.SetBool("isLoadingSniper", false);
        yield return new WaitForSeconds(0.25f + 1.2f / 2);
        iEnemy.weakAttackProjectile.Fire(iEnemy.firePivot.position, direction * 2);
        yield return new WaitForSeconds(0.33f);
    }

    private IEnumerator StrongAttack_Coroutine()
    {
        float timer = 0;
        iEnemy.animator.SetBool("isLoadingCannon", true);
        yield return iEnemy.LerpLookAt_Coroutine(ArmadilloPlayerController.Instance.transform, 2, 2.292f / 2);
        iEnemy.strongAttackProjectile.StartSpawnVFX();
        while (timer < 0.5f)
        {
            iEnemy.LerpLookAt(ArmadilloPlayerController.Instance.transform.position, 2);
            iEnemy.strongAttackProjectile.transform.position = iEnemy.firePivot.position;
            timer += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }
        timer = 0;
        iEnemy.strongAttackProjectile.StopSpawnVFX();
        iEnemy.strongAttackProjectile.ToggleVisual(true);
        while (timer<0.5f)
        {
            iEnemy.LerpLookAt(ArmadilloPlayerController.Instance.transform.position, 2);
            iEnemy.strongAttackProjectile.transform.position = iEnemy.firePivot.position;
            timer += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }
        timer = 0;
        iEnemy.strongAttackProjectile.ToggleFunctions(true);
        iEnemy.strongAttackProjectile.Launch(iEnemy.firePivot.position, ArmadilloPlayerController.Instance.transform.position);
        iEnemy.animator.SetTrigger("cannonFire");
        iEnemy.animator.SetBool("isLoadingCannon", false);
        yield return new WaitForSeconds(1.833f / 3);
    }
}
