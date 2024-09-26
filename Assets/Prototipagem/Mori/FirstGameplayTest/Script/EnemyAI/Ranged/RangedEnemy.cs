using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedEnemy : IEnemy, IDamageable, ISoundReceiver
{
    //-----Variables-----
    #region State Machine Variables

    public enum CurrentStateEnum
    {
        roaming,
        observing,
        searching,
        attacking
    }

    public CurrentStateEnum stateEnum;
    protected RangedEnemyState currentState;

    public RangedEnemyRoamingState enemyRoamingState;
    public RangedEnemyObservingState enemyObservingState;
    public RangedEnemySearchingState enemySearchingState;
    public RangedEnemyAttackingState enemyAttackingState;
    #endregion

    #region Combat Variables
    [Header("Combat Variables")]
    public LaserTrackPlayer weakAttackLaser;

    public SandProjectile weakAttackProjectile;

    public SandBomb strongAttackProjectile;

    public Transform firePivot;
    #endregion

    //-----Action Update-----
    #region Action Update Functions
    public override void OnActionUpdate()
    {
        currentState.OnActionUpdate();
    }
    #endregion

    //-----Hear-----
    #region Hear Functions
    public void OnSoundHear(SoundData soundData)
    {

    }
    #endregion

    //-----Visibility Update-----
    #region Visibility Update Functions
    public override void OnVisibilityUpdate()
    {
        currentState.OnVisibilityUpdate();
    }
    #endregion

    //-----Take Damage-----
    #region Take Damage Functions
    public void TakeDamage(Damage damage)
    {

    }
    #endregion

    //-----Base Functions-----
    #region Base Functions
    protected override void OnAwake()
    {
        enemyRoamingState = new RangedEnemyRoamingState(this);
        enemyObservingState = new RangedEnemyObservingState(this);
        enemySearchingState = new RangedEnemySearchingState(this);
        enemyAttackingState = new RangedEnemyAttackingState(this);
        currentState = enemyRoamingState;
    }
    protected override void OnStart()
    {
        animator.SetBool("isWalking", true);
    }
    protected override void OnFixedUpdate()
    {
        currentState.OnFixedUpdate();
    }
    #endregion

    //-----State Machine Functions-----
    #region State Machine
    public void ChangeCurrentState(RangedEnemyState newState)
    {
        if (currentState == newState) return;
        currentState.OnExitState();
        newState.OnEnterState();
        currentState = newState;
    }
    #endregion

    //-----Pathfinding-----
    #region Pathfinding Functions
    protected override void OnRoamingPathEnd()
    {

    }
    #endregion
}
