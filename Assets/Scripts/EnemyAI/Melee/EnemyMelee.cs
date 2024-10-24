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
    [SerializeField] private float minimalHearValue;
    [Tooltip("Tempo necessario para ir ao estado mais alto de detecção")] readonly protected float timeToMaxDetect = 0.5f;
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

    [HideInInspector]public bool playHitAnimation;
    #endregion

    //Sfx Variables
    #region SFX Variables
    [Header("SFX")]
    public SoundEmitter walkingSoundEmitter;
    #endregion

    //Vfx Variables
    #region VFX Variables
    [SerializeField] private ParticleSystem onDeathByEletricParticle;
    [SerializeField] private VisualEffect deadLoopParticle;
    #endregion

    //-----

    //Base Functions
    #region Base Functions
    protected override void OnAwake()
    {
        playHitAnimation = true;
        enemyRoamingState = new MeleeEnemyRoamingState(this);
        enemyObservingState = new MeleeEnemyObservingState(this);
        enemySearchingState = new MeleeEnemySearchingState(this);
        enemyAttackingState = new MeleeEnemyAttackingState(this);
        switch(currentAIBehaviour)
        {
            case AIBehaviour.Static:
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
        currentHp -= damage.damageAmount;
        if (currentHp <= 0 && isDead == false)
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
            EnemyMasterControl.Instance.StartCoroutine(DeSpawnCoroutine(animator.gameObject, deadLoopParticle));
            gameObject.SetActive(false);
            return;
        }
        if (playHitAnimation) animator.SetTrigger("isHit");
        OnDamageTaken(damage);
    }
    bool IDamageable.isDead()
    {
        return isDead;
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
        ChangeCurrentState(enemySearchingState);
        SetDetectionLevel(searchingStateBreakPoint);
        StopLookAround();
        onDamageTaken_Ref = null;
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
}
