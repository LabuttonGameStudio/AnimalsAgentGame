using System.Collections;
using Pixeye.Unity;
using System.Collections.Generic;
using UnityEngine;
using static AIBehaviourEnums;
using UnityEngine.VFX;

public class EnemyBombardier : IEnemy, IDamageable
{
    //-----Variables-----
    #region State Machine Variables
    protected BombardierEnemyState currentState;

    public BombardierEnemyRoamingState enemyRoamingState;
    public BombardierEnemyObservingState enemyObservingState;
    public BombardierEnemySearchingState enemySearchingState;
    public BombardierEnemyAttackingState enemyAttackingState;
    #endregion

    #region Combat Variables
    [Foldout("Combat Variables", true)]

    [Header("Attacks")]
    public BombardierBomb bombardierBomb;
    public Transform firePivot;

    [Header("HitBox")]
    public DamageableHitbox[] damageableHitboxes;

    private float defaultSpeed;
    private float defaultAcceleration;
    #endregion

    #region Detection Variables
    [Foldout("Detection", true)]
    public float timeToMaxDetect;

    #endregion

    #region Visual Variables
    [Foldout("Visual", true)]
    [SerializeField] private SkinnedMeshRenderer meshRenderer;

    private Vector3 defaultAnimatorPos;
    private Quaternion defaultAnimatorRot;
    #endregion

    #region VFX Variables
    [Foldout("VFX",true)]
    [SerializeField] private VisualEffect onDeathVFX;
    [SerializeField] private ParticleSystem onDeathByElectricityVFX;
    [SerializeField] private ParticleSystem onHitVFX;
    [SerializeField] private ParticleSystem onHitCriticalVFX;
    #endregion

    //-----Base Functions-----
    #region Base Functions
    protected override void OnAwake()
    {
        defaultAnimatorPos = animator.transform.localPosition;
        defaultAnimatorRot = animator.transform.localRotation;

        defaultSpeed = navMeshAgent.speed;
        defaultAcceleration = navMeshAgent.acceleration;

        navMeshAgent.updateRotation = true;
        enemyRoamingState = new BombardierEnemyRoamingState(this);
        enemyObservingState = new BombardierEnemyObservingState(this);
        enemySearchingState = new BombardierEnemySearchingState(this);
        enemyAttackingState = new BombardierEnemyAttackingState(this);
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
        currentHp = currentHp - damage.damageAmount;
        if (currentHp <= 0)
        {
            isDead = true;
            animator.transform.parent = animator.transform.parent.parent;
            animator.SetBool("isWalking", false);
            animator.SetBool("isDead", true);
            deSpawn_Ref = EnemyMasterControl.Instance.StartCoroutine(DeSpawn_Coroutine());
            foreach (DamageableHitbox hitbox in damageableHitboxes)
            {
                foreach (Collider collider in hitbox._Collider) collider.enabled = false;
                hitbox._IsDead = true;
            }
            switch (damage.damageType)
            {
                case Damage.DamageType.Eletric:
                    onDeathByElectricityVFX.Play();
                    break;
            }
            onDeathVFX.Play();
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
        meshRenderer.sharedMaterial = EnemyMasterControl.Instance.onHitBomberEnemyMat;
        yield return new WaitForSeconds(0.1f);
        meshRenderer.sharedMaterial = EnemyMasterControl.Instance.defaultBomberEnemyMat;
        onTakeDamageVisual_Ref = null;
    }

    private Coroutine deSpawn_Ref;
    private IEnumerator DeSpawn_Coroutine()
    {
        Physics.Raycast(animator.transform.position+Vector3.down*2, Vector3.down*50f, out RaycastHit hitInfo, 50, LayerManager.Instance.activeColliders, QueryTriggerInteraction.Ignore);
        Debug.Log(hitInfo.point);
        float timer = 0;
        yield return new WaitForSeconds(0.2f);
        float duration = 1.0f;

        Vector3 startPosition = animator.transform.position;

        Vector3 startRot = animator.transform.localRotation.eulerAngles;

        Vector3 finalRot = animator.transform.localRotation.eulerAngles;
        finalRot.z += 270*(Random.Range(0,2)*2-1);
        finalRot.y += 270*(Random.Range(0,2)*2-1);

            Vector3 velocity = Vector3.zero;
        while (timer < duration)
        {
            float smoothTime = Mathf.Max(0.0005f, duration - timer);
            animator.transform.position = Vector3.SmoothDamp(animator.transform.position, hitInfo.point + Vector3.up * 0.25f,ref velocity, smoothTime, float.PositiveInfinity,Time.deltaTime);
            //animator.transform.position = Vector3.Slerp(startPosition, hitInfo.point + Vector3.up * 0.25f, timer / duration);

            animator.transform.localRotation = Quaternion.Euler(Vector3.Lerp(startRot, finalRot, timer / duration));
            timer += Time.deltaTime;
            yield return null;
        }
        animator.transform.position = hitInfo.point + Vector3.up *0.25f;
        animator.transform.localRotation = Quaternion.Euler(finalRot);
        yield return new WaitForSeconds(7);
        animator.gameObject.SetActive(false);

    }
    #endregion

    //-----State Machine Functions-----
    #region State Machine
    public void ChangeCurrentState(BombardierEnemyState newState)
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

    public new bool CheckForProximityOfPoint()
    {
        return navMeshAgent.remainingDistance < navMeshAgent.height / 2 + 0.25f;
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

    //----- Check For Los Unique For Flying -----
    #region Check for Los
    public new float CheckForPlayerLOS()
    {
        if (CheckForLOS(ArmadilloPlayerController.Instance.gameObject)) return 1;
        else return 0;
    }
    public new bool CheckForLOS(GameObject objectLooked)
    {
        Vector3 origin = eyeTransform.position;
        Vector3 destination = objectLooked.transform.position;
        Vector3 direction = destination - origin;

        if (Vector3.Distance(origin, destination) > viewDistance) return false;
        float deltaAngle = Vector3.Angle(direction, eyeTransform.forward);
        if (deltaAngle > fieldOfView) return false;

        if (Physics.Linecast(origin, destination, out RaycastHit hitInfo, visionBlockLayers, QueryTriggerInteraction.Ignore))
        {
            if (hitInfo.collider.gameObject != objectLooked) return false;
        }
        return true;
    }

    public new IEnumerator RoamingWaitToReachNextPoint()
    {
        while (true)
        {
            if (CheckForProximityOfPoint()) yield break;
            yield return new WaitForFixedUpdate();
        }
    }
    #endregion

    //Revive Function
    #region Revive Function
    public override void Revive(Vector3 respawnPos, Quaternion respawnRot)
    {
        isDead = false;
        animator.transform.parent = transform;
        animator.transform.localPosition = defaultAnimatorPos;
        animator.transform.localRotation = defaultAnimatorRot;
        transform.position = respawnPos;
        transform.rotation = respawnRot;
        animator.gameObject.SetActive(true);
        animator.SetBool("isWalking", false);
        animator.SetBool("isDead", false);
        if(deSpawn_Ref != null)
        {
            EnemyMasterControl.Instance.StopCoroutine(deSpawn_Ref);
            deSpawn_Ref=null;
        }
        foreach (DamageableHitbox hitbox in damageableHitboxes)
        {
            foreach (Collider collider in hitbox._Collider) collider.enabled = true;
            hitbox._IsDead = false;
        }
        currentHp = maxHp;
        gameObject.SetActive(true);
        fixedUpdate_Ref = StartCoroutine(FixedUpdate_Coroutine());
        currentState.OnEnterState();
    }
    public override void Revive()
    {
        isDead = false;
        animator.transform.parent = transform;
        animator.transform.localPosition = defaultAnimatorPos;
        animator.transform.localRotation = defaultAnimatorRot;
        transform.position = transform.position;
        transform.rotation = transform.rotation;
        animator.gameObject.SetActive(true);
        animator.SetBool("isWalking", false);
        animator.SetBool("isDead", false);
        if (deSpawn_Ref != null)
        {
            EnemyMasterControl.Instance.StopCoroutine(deSpawn_Ref);
            deSpawn_Ref = null;
        }
        foreach (DamageableHitbox hitbox in damageableHitboxes)
        {
            foreach (Collider collider in hitbox._Collider) collider.enabled = true;
            hitbox._IsDead = false;
        }
        currentHp = maxHp;
        gameObject.SetActive(true);
        fixedUpdate_Ref = StartCoroutine(FixedUpdate_Coroutine());
        currentState.OnEnterState();
    }

    public void SetVelocity(AIBehaviour behaviour)
    {
        switch (behaviour)
        {
            case AIBehaviour.Roaming:
            case AIBehaviour.Observing:
            case AIBehaviour.Searching:
                navMeshAgent.speed = defaultSpeed;
                navMeshAgent.acceleration = defaultAcceleration;
                break;
            case AIBehaviour.Attacking:
                navMeshAgent.speed = defaultSpeed * 2f;
                navMeshAgent.acceleration = defaultAcceleration * 2f;
                break;
        }
    }
    #endregion
}
