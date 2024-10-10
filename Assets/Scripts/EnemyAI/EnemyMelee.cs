using System.Collections;
using UnityEngine;
using static AIBehaviourEnums;
using AudioType = SoundGeneralControl.AudioType;
using UnityEngine.VFX;
using static Damage;
public class EnemyMelee : IEnemy, IDamageable, ISoundReceiver
{
    #region Detection
    [SerializeField] private float minimalHearValue;
    [Tooltip("Tempo necessario para ir ao estado mais alto de detecção")] readonly protected float timeToMaxDetect = 0.5f;
    #endregion

    #region StateMachine

    [HideInInspector] public MeleeEnemyState currentEnemyState;

    protected MeleeEnemyRoamingState enemyRoamingState;
    protected MeleeEnemyObservingState enemyObservingState;
    protected MeleeEnemySearchingState enemySearchingState;
    protected MeleeEnemyAttackingState enemyAttackingState;
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

    public enum Actions
    {
        Moving,
        Observing,
        Attacking
    }
    [HideInInspector] public Actions currentAction;

    [SerializeField] private ParticleSystem onDeathByEletricParticle;
    [SerializeField] private VisualEffect deadLoopParticle;
    protected override void OnAwake()
    {
        enemyRoamingState = new MeleeEnemyRoamingState(this);
        enemyObservingState = new MeleeEnemyObservingState(this);
        enemySearchingState = new MeleeEnemySearchingState(this);
        enemyAttackingState = new MeleeEnemyAttackingState(this);

        ChangeCurrentAIBehaviour(AIBehaviour.Roaming);
    }
    protected override void OnStart()
    {
        primaryAttackHitbox.aiController = this;
        secondaryAttackHitbox.aiController = this;
    }
    protected override void OnFixedUpdate()
    {
        if (navMeshAgent.velocity.magnitude > 0.75f)
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
        if (currentHp <= 0 && isDead==false)
        {
            isDead = true;
            animator.transform.parent = animator.transform.parent.parent;
            animator.SetTrigger("isDead");
            switch (damage.damageType)
            {
                case DamageType.Eletric:
                    onDeathByEletricParticle.Play();
                    break;
            }
            deadLoopParticle.Play();
            EnemyMasterControl.Instance.StartCoroutine(DeSpawnCoroutine(animator.gameObject,deadLoopParticle));
            gameObject.SetActive(false);
            return;
        }
        if (currentAction == Actions.Moving || currentAction == Actions.Observing) animator.SetTrigger("isHit");
        OnDamageTaken(damage);
    }
    bool IDamageable.isDead()
    {
        return isDead;
    }
    private IEnumerator DeSpawnCoroutine(GameObject gameObject,VisualEffect visualEffect)
    {
        yield return new WaitForSeconds(5);
        visualEffect.Stop();
        yield return new WaitForSeconds(15);
        gameObject.SetActive(false);
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
        if (currentAIBehaviour == AIBehaviour.Attacking || currentAIBehaviour == AIBehaviour.Searching) yield break;
        lastKnownPlayerPos = damage.originPoint;
        ChangeCurrentAIBehaviour(AIBehaviour.Searching);
        SetDetectionLevel(searchingStateBreakPoint);
        StopLookAround();
        onDamageTaken_Ref = null;
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

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            ArmadilloPlayerController playerControler = ArmadilloPlayerController.Instance;
            if (playerControler.currentForm == ArmadilloPlayerController.Form.Ball)
            {
                if (collision.impulse.magnitude > 200)
                {
                    TakeDamage(new Damage(10,DamageType.Blunt,true,playerControler.transform.position));
                    Vector3 direction = playerControler.transform.position -collision.GetContact(0).point;
                    direction.Normalize();
                    playerControler.movementControl.rb.AddForce(direction * 15, ForceMode.VelocityChange);

                    ArmadilloPlayerController.Instance.visualControl.OnBallHit(collision.GetContact(0).point, playerControler.transform.position);
                }
            }
        }
    }
}
