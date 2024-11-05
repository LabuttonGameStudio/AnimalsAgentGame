using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static AIBehaviourEnums;

public class EnemyBombardier  : IEnemy, IDamageable
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
    [Foldout("Combat Variables", styled = true)]

    [Header("Attacks")]
    public BombardierBomb bombardierBomb;
    public Transform firePivot;

    [Header("HitBox")]
    public DamageableHitbox[] damageableHitboxes;
    #endregion

    #region Detection Variables
    [Foldout("Detection", styled = true)]
    public float timeToMaxDetect;

    #endregion

    #region Visual Variables
    [Foldout("Visual", styled = true)]
    [SerializeField] private SkinnedMeshRenderer meshRenderer;
    #endregion

    //-----Base Functions-----
    #region Base Functions
    protected override void OnAwake()
    {
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
            animator.SetTrigger("isDefeated");
            foreach (DamageableHitbox hitbox in damageableHitboxes)
            {
                foreach (Collider collider in hitbox._Collider) collider.enabled = false;
                hitbox._IsDead = true;
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
}
