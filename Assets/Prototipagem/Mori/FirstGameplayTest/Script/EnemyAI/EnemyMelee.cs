using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using static AIBehaviourEnums;
using static SoundGeneralControl;
using AudioType = SoundGeneralControl.AudioType;
public class EnemyMelee : IEnemy, IDamageable, ISoundReceiver
{
    #region Detection
    [SerializeField] private float minimalHearValue;
    [Tooltip("Tempo necessario para ir ao estado mais alto de detecção")] readonly protected float timeToMaxDetect = 0.5f;
    #endregion

    #region StateMachine

    [HideInInspector] public MeleeEnemyState currentEnemyState;

    protected EnemyRoamingState enemyRoamingState;
    protected EnemyObservingState enemyObservingState;
    protected EnemySearchingState enemySearchingState;
    protected EnemyAttackingState enemyAttackingState;
    [SerializeField, Tooltip("Nivel de detecção necessario para trocar para o estado de searching")] private readonly float searchingStateBreakPoint = 50;
    #endregion

    [Header("Combat")]
    [Header("Primary Attack")]
    [SerializeField]
    public float primaryAttackDamage;
    public EnemyMeleeAttackHitBox primaryAttackHitbox;
    public float primaryAttackCooldown;
    [Header("Secondary Attack")]
    [SerializeField]
    public float secondaryAttackDamage;
    public EnemyMeleeAttackHitBox secondaryAttackHitbox;
    public float secondaryAttackCooldown;

    [Header("SFX")]
    public SoundEmitter walkingSoundEmitter;

    protected override void OnAwake()
    {
        enemyRoamingState = new EnemyRoamingState(this);
        enemyObservingState = new EnemyObservingState(this);
        enemySearchingState = new EnemySearchingState(this);
        enemyAttackingState = new EnemyAttackingState(this);

        ChangeCurrentAIBehaviour(AIBehaviour.Roaming);
    }
    protected override void OnStart()
    {
        primaryAttackHitbox.aiController = this;
        secondaryAttackHitbox.aiController = this;
    }
    protected override void OnFixedUpdate()
    {
        if(navMeshAgent.velocity.magnitude > 0.75f)
        {
            walkingSoundEmitter.PlayAudio();
        }
        else
        {
            walkingSoundEmitter.StopAudio();
        }
        currentEnemyState.OnFixedUpdate();
    }
    public override void OnVisibilityUpdate()
    {
        currentEnemyState.OnVisibilityUpdate();
    }
    public override void OnActionUpdate()
    {
        currentEnemyState.OnActionUpdate();
    }
    public void TakeDamage(Damage damage)
    {
        currentHp -= damage.damageAmount;
        Debug.Log("HP=" + currentHp + "| Damage taken=" + damage.damageAmount);
        if (currentHp <= 0)
        {
            isDead = true;
            gameObject.SetActive(false);
            return;
        }
        OnDamageTaken(damage);
    }
    private void OnDamageTaken(Damage damage)
    {
        if (currentAIBehaviour != AIBehaviour.Attacking)
        {
            if (onDamageTaken_Ref == null)
            {
                onDamageTaken_Ref = StartCoroutine(OnDamageTaken_Coroutine(damage));
            }
        }
    }
    private Coroutine onDamageTaken_Ref;
    private IEnumerator OnDamageTaken_Coroutine(Damage damage)
    {
        yield return new WaitForSeconds(0.25f);
        lastKnownPlayerPos = damage.originPoint;
        ChangeCurrentAIBehaviour(AIBehaviour.Searching);
        SetDetectionLevel(searchingStateBreakPoint);
    }
    protected override void OnRoamingPathEnd()
    {
        isStatic = true;
        currentEnemyState.OnEnterState();
        navMeshAgent.isStopped = true;
    }

    public void ChangeCurrentAIBehaviour(AIBehaviour nextAIBehaviour)
    {
        if (currentAIBehaviour == nextAIBehaviour) return;
        if (currentEnemyState != null) currentEnemyState.OnExitState();
        enemyBehaviourVisual.ChangeVisualState(nextAIBehaviour);
        switch (nextAIBehaviour)
        {
            case AIBehaviour.Roaming:
                currentEnemyState = enemyRoamingState;
                currentEnemyState.OnEnterState();
                break;
            case AIBehaviour.Observing:
            case AIBehaviour.Static:
                currentEnemyState = enemyObservingState;
                currentEnemyState.OnEnterState();
                break;
            case AIBehaviour.Searching:
                currentEnemyState = enemySearchingState;
                currentEnemyState.OnEnterState();
                break;
            case AIBehaviour.Attacking:
                currentEnemyState = enemyAttackingState;
                currentEnemyState.OnEnterState();
                break;
        }
        currentAIBehaviour = nextAIBehaviour;
    }

    public void SetDetectionLevel(float detectionLevel)
    {
        this.detectionLevel = detectionLevel;
    }
    float detectionTickIntervalTime = 0f;
    public void IncreaseDetection()
    {
        float increasePerTick;

        if (detectionTickIntervalTime > 0)
        {
            increasePerTick = 100 * (Time.time - detectionTickIntervalTime) / timeToMaxDetect;
        }
        else increasePerTick = 100 / (timeToMaxDetect / EnemyMasterControl.Instance.visibilityTickInterval);
        detectionTickIntervalTime = Time.time;
        //Se ele atingir o limite acima de 100, ele nao sobe mais que isso e altera seu estado para Attacking
        if (detectionLevel + increasePerTick >= 100)
        {
            detectionLevel = 100;
            ChangeCurrentAIBehaviour(AIBehaviour.Attacking);
            return;
        }
        else detectionLevel += increasePerTick;
        //Se ele nao estiver acima de 100 mas estiver acima do nivel necessario para entrar em estado de procura
        if (detectionLevel > searchingStateBreakPoint)
        {
            ChangeCurrentAIBehaviour(AIBehaviour.Searching);
        }
    }
    public void ResetTickInterval()
    {
        detectionTickIntervalTime = 0;
    }

    public bool heardPlayer;
    public void OnSoundHear(SoundData soundData)
    {
        if (soundData.audioType == AudioType.Suspicious)
        {
            if (soundData.audioPercentage >= minimalHearValue)
            {
                lastKnownPlayerPos = soundData.originPoint;
                if (!heardPlayer)
                {
                    heardPlayer = true;
                    switch (currentAIBehaviour)
                    {
                        case AIBehaviour.Roaming:
                            ChangeCurrentAIBehaviour(AIBehaviour.Observing);
                            break;
                        case AIBehaviour.Observing:
                            ChangeCurrentAIBehaviour(AIBehaviour.Searching);
                            break;
                    }
                }
            }
        }
    }
}
