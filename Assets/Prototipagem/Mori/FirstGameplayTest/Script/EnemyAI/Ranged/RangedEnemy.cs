using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static AIBehaviourEnums;

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

    public CurrentStateEnum currentStateEnum;
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

    #region Detection Variables
    [Header("Detection")]
    public float timeToMaxDetect;
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
        currentHp = currentHp- damage.damageAmount;
        if (currentHp <= 0)
        {
            isDead = true;
            animator.transform.parent = animator.transform.parent.parent;
            animator.SetBool("isWalking",false);
            animator.SetTrigger("isDefeated");
            gameObject.SetActive(false);
        }
        else if (damage.wasMadeByPlayer)
        {
            switch (currentStateEnum)
            {
                case CurrentStateEnum.roaming:
                case CurrentStateEnum.observing:
                    lastKnownPlayerPos = damage.originPoint;
                    ChangeCurrentState(enemySearchingState);
                    break;
            }
        }
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
        currentState.OnEnterState();
    }
    protected override void OnStart()
    {

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

    //-----Detection Level-----
    #region Detection

    public void IncreaseDetection()
    {
        float increasePerTick = 100 * EnemyMasterControl.Instance.visibilityTickInterval / timeToMaxDetect;
        if (detectionLevel + increasePerTick >= 100)
        {
            detectionLevel = 100;
            ChangeCurrentState(enemyAttackingState);
            return;
        }
        else detectionLevel += increasePerTick;
        //Se ele nao estiver acima de 100 mas estiver acima do nivel necessario para entrar em estado de procura
        if (detectionLevel > 50)
        {
            ChangeCurrentState(enemySearchingState);
        }
    }
    #endregion
}
