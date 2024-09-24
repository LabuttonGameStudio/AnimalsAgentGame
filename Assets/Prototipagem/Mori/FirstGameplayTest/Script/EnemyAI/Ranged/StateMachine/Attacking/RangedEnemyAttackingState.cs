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
        attackLoop_Ref = iEnemy.StartCoroutine(AttackLoop_Coroutine());
        Debug.Log("Attacking Enter");
    }

    public override void OnExitState()
    {
        Debug.Log("Attacking Exit");
    }

    public override void OnFixedUpdate()
    {
        Debug.Log("Attacking FixedUpdate");
    }

    public override void OnVisibilityUpdate()
    {
        if (iEnemy.stateEnum != RangedEnemy.CurrentStateEnum.attacking)
        {
            switch (iEnemy.stateEnum)
            {
                case RangedEnemy.CurrentStateEnum.roaming:
                    iEnemy.ChangeCurrentState(iEnemy.enemyRoamingState);
                    break;
                case RangedEnemy.CurrentStateEnum.observing:
                    iEnemy.ChangeCurrentState(iEnemy.enemyObservingState);
                    break;
                case RangedEnemy.CurrentStateEnum.searching:
                    iEnemy.ChangeCurrentState(iEnemy.enemySearchingState);
                    break;
                case RangedEnemy.CurrentStateEnum.attacking:
                    iEnemy.ChangeCurrentState(iEnemy.enemyAttackingState);
                    break;
            }
        }
    }
    private Coroutine attackLoop_Ref;
    private IEnumerator AttackLoop_Coroutine()
    {
        while (true)
        {
            for (int i = 0; i < 2; i++)
            {
                yield return WeakAttack_Coroutine();
                yield return new WaitForSeconds(6 - 2.375f / 2);
            }
            yield return StrongAttack_Coroutine();
            yield return new WaitForSeconds(1f);
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
        iEnemy.weakAttackLaser.gameObject.SetActive(true);
        iEnemy.weakAttackLaser.target = Vector3.zero;
        while (timer < trackDuration)
        {
            iEnemy.weakAttackLaser.target = playerRb.position + playerRb.velocity/10;
            timer += Time.deltaTime;
            yield return null;
        }
        yield return new WaitForSeconds(0.25f);
        iEnemy.weakAttackLaser.gameObject.SetActive(false);
        iEnemy.weakAttackLaser.ResetLaser();
        Vector3 direction = iEnemy.weakAttackLaser.target - iEnemy.firePivot.position;
        direction.Normalize();
        iEnemy.animator.SetTrigger("sniperFire");
        iEnemy.animator.SetBool("isLoadingSniper", false);
        yield return new WaitForSeconds(0.25f+1.2f/2);
        iEnemy.weakAttackProjectile.Fire(iEnemy.firePivot.position, direction*2);
        yield return new WaitForSeconds(0.33f);
    }

    private IEnumerator StrongAttack_Coroutine()
    {
        iEnemy.animator.SetBool("isLoadingCannon", true);
        yield return new WaitForSeconds(2.292f / 2);
        iEnemy.strongAttackProjectile.StartSpawnVFX();
        yield return new WaitForSeconds(0.5f);
        iEnemy.strongAttackProjectile.StopSpawnVFX();
        iEnemy.strongAttackProjectile.ToggleVisual(true);
        yield return new WaitForSeconds(0.5f);
        iEnemy.strongAttackProjectile.ToggleFunctions(true);
        iEnemy.strongAttackProjectile.Launch(iEnemy.firePivot.position,ArmadilloPlayerController.Instance.transform.position);
        iEnemy.animator.SetTrigger("cannonFire");
        iEnemy.animator.SetBool("isLoadingCannon", false);
        yield return new WaitForSeconds(1.833f / 3);
    }
}
