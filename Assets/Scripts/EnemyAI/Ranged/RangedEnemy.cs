using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static AIBehaviourEnums;

public class RangedEnemy : IEnemy, IDamageable, ISoundReceiver
{
    //-----Variables-----
    #region State Machine Variables
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

    public DamageableHitbox[] damageableHitboxes;
    #endregion

    #region Detection Variables
    [Header("Detection")]
    public float timeToMaxDetect;

    #endregion

    #region Visual Variables
    [Header("Visual")]
    [SerializeField] private SkinnedMeshRenderer meshRenderer;
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
        switch (currentAIBehaviour)
        {
            case AIBehaviour.Static:
                isStatic = true;
                ChangeCurrentState(enemyRoamingState);
                break;
            case AIBehaviour.Roaming:
                ChangeCurrentState(enemyRoamingState);
                break;
            case AIBehaviour.Observing:
                ChangeCurrentState(enemyObservingState);
                break;
            case AIBehaviour.Searching:
                ChangeCurrentState(enemySearchingState);
                break;
            case AIBehaviour.Attacking:
                ChangeCurrentState(enemyAttackingState);
                break;
        }
    }
    protected override void OnStart()
    {
        foreach (DamageableHitbox hitbox in damageableHitboxes)
        {
            hitbox.OnTakeDamage.AddListener(TakeDamage);
        }
        currentState.OnEnterState();
    }
    protected override void OnFixedUpdate()
    {
        currentState.OnFixedUpdate();
        currentState.OnVisibilityUpdate();
    }
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
        //currentState.OnVisibilityUpdate();
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
            foreach(DamageableHitbox hitbox in damageableHitboxes)
            {
                hitbox._Collider.enabled = false;
                hitbox._IsDead = true;
            }
            gameObject.SetActive(false);
        }
        else if (damage.wasMadeByPlayer)
        {
            switch (currentAIBehaviour)
            {
                case AIBehaviour.Roaming:
                case AIBehaviour.Observing:
                    lastKnownPlayerPos = damage.originPoint;
                    ChangeCurrentState(enemySearchingState);
                    break;
            }
        }
        if(onTakeDamageVisual_Ref != null)StopCoroutine(onTakeDamageVisual_Ref);
        onTakeDamageVisual_Ref = EnemyMasterControl.Instance.StartCoroutine(OnTakeDamageVisual_Coroutine());
    }

    bool IDamageable.isDead()
    {
        return isDead;
    }

    private Coroutine onTakeDamageVisual_Ref;
    private IEnumerator OnTakeDamageVisual_Coroutine()
    {
        meshRenderer.sharedMaterial = EnemyMasterControl.Instance.onHitRangedEnemyMat;
        yield return new WaitForSeconds(0.1f);
        meshRenderer.sharedMaterial = EnemyMasterControl.Instance.defaultRangedEnemyMat;
        onTakeDamageVisual_Ref = null;
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
