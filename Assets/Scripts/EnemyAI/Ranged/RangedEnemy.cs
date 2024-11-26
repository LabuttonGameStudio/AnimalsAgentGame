using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;
using Pixeye.Unity;
using static AIBehaviourEnums;
using UnityEngine.UIElements;
using UnityEngine.InputSystem.LowLevel;

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
    [Foldout("Combat Variables", true)]

    [Header("Attacks")]
    public LaserTrackPlayer weakAttackLaser;

    public SandProjectile weakAttackProjectile;

    public SandBomb strongAttackProjectile;

    public Transform firePivot;

    [Header("HitBox")]
    public DamageableHitbox[] damageableHitboxes;
    #endregion

    #region Detection Variables
    [Foldout("Detection", true)]
    public float timeToMaxDetect;

    #endregion

    #region Visual Variables
    [Foldout("Visual", true)]
    [Header("Mesh")]
    [SerializeField] private SkinnedMeshRenderer meshRenderer;
    [Header("VFX")]
    [SerializeField] private VisualEffect onDeathVFX;
    [SerializeField] private ParticleSystem onDeathByElectricityVFX;
    [SerializeField] private ParticleSystem onHitVFX;
    [SerializeField] private ParticleSystem onHitCriticalVFX;
    #endregion

    #region Audios Variables
    [Foldout("Audio Variables",true)]
    [SerializeField] private AudioSource walkingAudio;
    [SerializeField] public AudioSource onFireAudio;
    #endregion

    //-----Base Functions-----
    #region Base Functions
    protected override void OnAwake()
    {
        enemyRoamingState = new RangedEnemyRoamingState(this);
        enemyObservingState = new RangedEnemyObservingState(this);
        enemySearchingState = new RangedEnemySearchingState(this);
        enemyAttackingState = new RangedEnemyAttackingState(this);
        switch (currentAIBehaviour)
        {
            case AIBehaviour.Static:
                isStatic = true;
                enemyRoamingState.OnEnterState();
                currentState = enemyRoamingState;
                break;
            case AIBehaviour.Roaming:
                isStatic = false;
                enemyRoamingState.OnEnterState();
                currentState = enemyRoamingState;
                break;
            case AIBehaviour.Observing:
                enemyObservingState.OnEnterState();
                currentState = enemyObservingState;
                break;
            case AIBehaviour.Searching:
                enemySearchingState.OnEnterState();
                currentState = enemySearchingState;
                break;
            case AIBehaviour.Attacking:
                enemyAttackingState.OnEnterState();
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
        currentState.OnEnterState();
    }
    protected override void OnFixedUpdate()
    {
        currentState.OnFixedUpdate();
        currentState.OnVisibilityUpdate();
        if (navMeshAgent.velocity.magnitude > 0.5f)
        {
            if (!walkingAudio.isPlaying)
            {
                walkingAudio.Play();
            }
        }
        else
        {
            if (walkingAudio.isPlaying)
            {
                walkingAudio.Stop();
            }
        }
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
        currentHp = currentHp - damage.damageAmount;
        if (currentHp <= 0)
        {
            isDead = true;
            animator.transform.parent = animator.transform.parent.parent;
            animator.SetBool("isWalking", false);
            animator.SetBool("isDead", true);
            foreach (DamageableHitbox hitbox in damageableHitboxes)
            {
                foreach (Collider collider in hitbox._Collider) collider.enabled = false;
                hitbox._IsDead = true;
            }
            //VFX
            deSpawn_Ref = EnemyMasterControl.Instance.StartCoroutine(DeSpawnCoroutine(animator.gameObject, onDeathVFX));

            switch (damage.damageType)
            {
                case Damage.DamageType.Eletric:
                    onDeathByElectricityVFX.Play();
                    break;
            }
            onDeathEvent.Invoke();
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

        //VFX
        if (damage.wasCritical) onHitCriticalVFX.Play();
        onHitVFX.Play();

        if (onTakeDamageVisual_Ref != null) StopCoroutine(onTakeDamageVisual_Ref);
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
    private Coroutine deSpawn_Ref;
    private IEnumerator DeSpawnCoroutine(GameObject gameObject, VisualEffect visualEffect)
    {
        yield return new WaitForSeconds(5);
        visualEffect.Stop();
        yield return new WaitForSeconds(15);
        gameObject.SetActive(false);
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

    public override void TurnAgressive()
    {
        if (currentState == enemyAttackingState) return;
        ChangeCurrentState(enemyAttackingState);
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

    //Revive Function
    #region Revive Function
    public override void Revive(Vector3 respawnPos, Quaternion respawnRot)
    {
        isDead = false;
        animator.transform.parent = transform;
        animator.SetBool("isDead", false);

        gameObject.transform.position = respawnPos;
        gameObject.transform.rotation = respawnRot;
        animator.gameObject.SetActive(true);

        foreach (DamageableHitbox hitbox in damageableHitboxes)
        {
            foreach (Collider collider in hitbox._Collider) collider.enabled = true;
            hitbox._IsDead = false;
        }
        if (deSpawn_Ref != null)
        {
            EnemyMasterControl.Instance.StopCoroutine(deSpawn_Ref);
            deSpawn_Ref = null;
        }
        onDeathVFX.Stop();
        currentHp = maxHp;
        gameObject.SetActive(true);
        fixedUpdate_Ref = StartCoroutine(FixedUpdate_Coroutine());
        currentState.OnEnterState();
    }
    public override void Revive()
    {
        isDead = false;
        animator.transform.parent = transform;
        animator.SetBool("isDead", false);
        animator.gameObject.SetActive(true);

        foreach (DamageableHitbox hitbox in damageableHitboxes)
        {
            foreach (Collider collider in hitbox._Collider) collider.enabled = true;
            hitbox._IsDead = false;
        }
        if (deSpawn_Ref != null)
        {
            EnemyMasterControl.Instance.StopCoroutine(deSpawn_Ref);
            deSpawn_Ref = null;
        }
        onDeathVFX.Stop();
        currentHp = maxHp;
        gameObject.SetActive(true);
        fixedUpdate_Ref = StartCoroutine(FixedUpdate_Coroutine());
        currentState.OnEnterState();
    }
    #endregion

}
