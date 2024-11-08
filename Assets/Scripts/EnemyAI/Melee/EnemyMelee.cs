using System.Collections;
using UnityEngine;
using AudioType = SoundGeneralControl.AudioType;
using UnityEngine.VFX;
using static Damage;
using static AIBehaviourEnums;
public class EnemyMelee : IEnemy, IDamageable, ISoundReceiver
{
    //Detection Variables
    #region Detection Variables
    [HideInInspector] public bool heardPlayer;
    [HideInInspector] private float minimalHearValue;
    [Tooltip("Tempo necessario para ir ao estado mais alto de detecção")] readonly protected float timeToMaxDetect = 0.5f;
    #endregion

    //Visual Variables
    #region Visual Variables
    [Foldout("Visual", styled = true)]
    [SerializeField] private SkinnedMeshRenderer meshRenderer;
    #endregion

    //State Machine Variables
    #region StateMachine Variables

    [HideInInspector] public MeleeEnemyState currentState;

    public MeleeEnemyRoamingState enemyRoamingState;
    public MeleeEnemyObservingState enemyObservingState;
    public MeleeEnemySearchingState enemySearchingState;
    public MeleeEnemyAttackingState enemyAttackingState;
    [SerializeField, Tooltip("Nivel de detecção necessario para trocar para o estado de searching")] private readonly float searchingStateBreakPoint = 50;

    #endregion

    //Combat Variables
    #region Combat Variables
    [Foldout("Combat", styled = true)]

    [Header("Shield")]
    public GameObject shieldGameObject;
    [HideInInspector] public bool isShieldActive;

    [Header("Hitbox")]
    public DamageableHitbox[] damageableHitboxes;

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

    [HideInInspector] public bool playHitAnimation;
    #endregion

    //Sfx Variables
    #region SFX Variables
    [Foldout("SFX", styled = true)]
    public SoundEmitter walkingSoundEmitter;
    #endregion

    //Vfx Variables
    #region VFX Variables
    [Foldout("VFX", styled = true)]
    [SerializeField] private ParticleSystem onHitParticleScrewAndBolts;
    [SerializeField] private ParticleSystem onHitParticleSparkDefault;
    [SerializeField] private ParticleSystem onHitParticleSparkCritical;
    [SerializeField] private ParticleSystem onDeathByEletricParticle;
    [SerializeField] private VisualEffect deadLoopParticle;
    #endregion

    //-----

    //Base Functions
    #region Base Functions
    protected override void OnAwake()
    {
        navMeshAgent.speed *= Random.Range(0.5f, 0.75f);
        navMeshAgent.acceleration *= Random.Range(0.5f, 0.75f);
        playHitAnimation = true;
        enemyRoamingState = new MeleeEnemyRoamingState(this);
        enemyObservingState = new MeleeEnemyObservingState(this);
        enemySearchingState = new MeleeEnemySearchingState(this);
        enemyAttackingState = new MeleeEnemyAttackingState(this);
        switch (currentAIBehaviour)
        {
            case AIBehaviour.Static:
                isStatic = true;
                currentState = enemyRoamingState;
                break;
            case AIBehaviour.Roaming:
                currentState = enemyRoamingState;
                break;
            case AIBehaviour.Observing:
                currentState = enemyObservingState;
                break;
            case AIBehaviour.Searching:
                currentState = enemySearchingState;
                break;
            case AIBehaviour.Attacking:
                currentState = enemyAttackingState;
                break;
        }
    }
    protected override void OnStart()
    {
        foreach (DamageableHitbox hitbox in damageableHitboxes)
        {
            hitbox.OnTakeDamage.AddListener(TakeDamage);
        }
        meshRenderer.sharedMaterial = EnemyMasterControl.Instance.defaultMeleeEnemyMat;
        primaryAttackHitbox.aiController = this;
        secondaryAttackHitbox.aiController = this;
        currentState.OnEnterState();
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
        currentState.OnFixedUpdate();
        currentState.OnVisibilityUpdate();
    }
    #endregion

    //Visibility Functions
    #region Visibility Functions
    public override void OnVisibilityUpdate()
    {
        //currentState.OnVisibilityUpdate();
    }
    #endregion

    //Action Functions
    #region Action Functions
    public override void OnActionUpdate()
    {
        currentState.OnActionUpdate();
    }
    #endregion

    //Take Damage Functions
    #region Take Damage Functions
    public void TakeDamage(Damage damage)
    {
        if (isShieldActive) return;
        currentHp -= damage.damageAmount;
        if (currentHp <= 0 && isDead == false)
        {
            foreach (DamageableHitbox hitbox in damageableHitboxes)
            {
                foreach (Collider collider in hitbox._Collider) collider.enabled = false;
                hitbox._IsDead = true;
            }
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
            EnemyMasterControl.Instance.StartCoroutine(DeSpawnCoroutine(animator.gameObject, deadLoopParticle));
            shieldGameObject.SetActive(false);
            if (Random.Range(0f, 1f) >= 0.5f)
            {
                EnemyMasterControl.Instance.SpawnRandomCollectable(transform.position - Vector3.up);
            }
            if (onDamageTakenVisual_Ref != null) StopCoroutine(onDamageTakenVisual_Ref);
            onDamageTakenVisual_Ref = EnemyMasterControl.Instance.StartCoroutine(OnDamageTakenVisual_Coroutine());
            onDeathEvent.Invoke();
            gameObject.SetActive(false);
        }
        else
        {
            if (playHitAnimation) animator.SetTrigger("isHit");
            if (onDamageTaken_Ref == null)
            {
                onDamageTaken_Ref = StartCoroutine(OnDamageTaken_Coroutine(damage));
            }
            if (onDamageTakenVisual_Ref != null) StopCoroutine(onDamageTakenVisual_Ref);
            onDamageTakenVisual_Ref = EnemyMasterControl.Instance.StartCoroutine(OnDamageTakenVisual_Coroutine());
        }
        if (damage.wasCritical) onHitParticleSparkDefault.Play();
        onHitParticleScrewAndBolts.Play();
    }
    bool IDamageable.isDead()
    {
        return isDead;
    }
    private Coroutine onDamageTaken_Ref;
    private IEnumerator OnDamageTaken_Coroutine(Damage damage)
    {
        yield return new WaitForSeconds(0.25f);
        if (currentAIBehaviour == AIBehaviour.Attacking || currentAIBehaviour == AIBehaviour.Searching) yield break;
        lastKnownPlayerPos = damage.originPoint;
        ChangeCurrentState(enemySearchingState);
        SetDetectionLevel(searchingStateBreakPoint);
        StopLookAround();
        onDamageTaken_Ref = null;
    }
    private Coroutine onDamageTakenVisual_Ref;
    private IEnumerator OnDamageTakenVisual_Coroutine()
    {
        meshRenderer.sharedMaterial = EnemyMasterControl.Instance.onHitMeleeEnemyMat;
        yield return new WaitForSeconds(0.1f);
        meshRenderer.sharedMaterial = EnemyMasterControl.Instance.defaultMeleeEnemyMat;
    }
    #endregion

    //State Machine Functions
    #region State Machine Functions
    public void ChangeCurrentState(MeleeEnemyState newState)
    {
        if (currentState == newState) return;
        currentState.OnExitState();
        newState.OnEnterState();
        currentState = newState;
    }

    public override void TurnAgressive()
    {
        if (currentState == enemyAttackingState) return;
        ChangeCurrentState(enemyAttackingState);
    }
    #endregion

    //Detection Functions
    #region Detection Functions
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
            ChangeCurrentState(enemyAttackingState);
            return;
        }
        else detectionLevel += increasePerTick;
        //Se ele nao estiver acima de 100 mas estiver acima do nivel necessario para entrar em estado de procura
        if (detectionLevel > searchingStateBreakPoint)
        {
            ChangeCurrentState(enemySearchingState);
        }
    }
    public void ResetTickInterval()
    {
        detectionTickIntervalTime = 0;
    }
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
                            ChangeCurrentState(enemyObservingState);
                            break;
                        case AIBehaviour.Observing:
                            ChangeCurrentState(enemySearchingState);
                            break;
                    }
                }
            }
        }
    }
    #endregion

    //Pathfinding Functions
    #region Pathfinding Functions
    protected override void OnRoamingPathEnd()
    {
        isStatic = true;
        currentState.OnEnterState();
        navMeshAgent.isStopped = true;
    }

    #endregion

    //DeSpawn Functions
    #region DeSpawn Functions
    private IEnumerator DeSpawnCoroutine(GameObject gameObject, VisualEffect visualEffect)
    {
        yield return new WaitForSeconds(5);
        visualEffect.Stop();
        yield return new WaitForSeconds(15);
        gameObject.SetActive(false);
    }
    #endregion

    //Ball Contact Functions
    #region Ball Contact Function
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            ArmadilloPlayerController playerControler = ArmadilloPlayerController.Instance;
            if (playerControler.currentForm == ArmadilloPlayerController.Form.Ball)
            {
                if (collision.impulse.magnitude > 200)
                {
                    TakeDamage(new Damage(10, DamageType.Blunt, true, playerControler.transform.position));
                    Vector3 direction = playerControler.transform.position - collision.GetContact(0).point;
                    direction.Normalize();
                    playerControler.movementControl.rb.AddForce(direction * 15, ForceMode.VelocityChange);

                    ArmadilloPlayerController.Instance.visualControl.OnBallHit(collision.GetContact(0).point, playerControler.transform.position);
                }
            }
        }
    }
    #endregion

    //Shield Functions
    #region Shield Functions
    public void ToggleShield(bool toggle)
    {
        isShieldActive = toggle;
        shieldGameObject.SetActive(toggle);
    }
    #endregion
}
