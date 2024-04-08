using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using static AIBehaviourEnums;

public class EnemyMelee : IEnemy, IDamageable
{
    #region Detection
    [Tooltip("Tempo necessario para ir ao estado mais alto de detecção")] readonly protected float timeToMaxDetect = 1.5f;
    #endregion
    #region StateMachine

    [HideInInspector] public MeleeEnemyState currentEnemyState;

    protected EnemyRoamingState enemyRoamingState;
    protected EnemyObservingState enemyObservingState;
    protected EnemySearchingState enemySearchingState;
    protected EnemyAttackingState enemyAttackingState;


    [SerializeField,Tooltip("Nivel de detecção necessario para trocar para o estado de observing")]private float observingStateBreakPoint = 21; 
    [SerializeField, Tooltip("Nivel de detecção necessario para trocar para o estado de searching")] private float searchingStateBreakPoint = 67; 
    #endregion
    protected override void OnAwake()
    {
        enemyRoamingState = new EnemyRoamingState(this);
        enemyObservingState = new EnemyObservingState(this);
        enemySearchingState = new EnemySearchingState(this);
        enemyAttackingState = new EnemyAttackingState(this);

        if (isStatic)
        {
            ChangeCurrentAIBehaviour(AIBehaviour.Observing);
            observingStateBreakPoint = -1;
        }
        else ChangeCurrentAIBehaviour(AIBehaviour.Roaming);
    }
    protected override void OnStart()
    {

    }
    protected override void OnFixedUpdate()
    {
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
    public void TakeDamage(int damageAmount)
    {
        currentHp -= damageAmount;
        Debug.Log("HP=" + currentHp + "| Damage taken=" + damageAmount);
        if (currentHp <= 0)
        {
            isDead = true;
            gameObject.SetActive(false);
        }
    }

    protected override void OnRoamingPathEnd()
    {
        ChangeCurrentAIBehaviour(AIBehaviour.Observing);
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

    public void IncreaseDetection()
    {
        timeSincePlayerLastSeen = 0;
        float increasePerTick = 100 / (timeToMaxDetect / EnemyControl.Instance.tickInterval);
        if (detectionLevel + increasePerTick >= 100)
        {
            detectionLevel = 100;
            ChangeCurrentAIBehaviour(AIBehaviour.Attacking);
            return;
        }
        else detectionLevel += increasePerTick;
        if (detectionLevel < observingStateBreakPoint) return;
        else if (detectionLevel >= observingStateBreakPoint && detectionLevel < searchingStateBreakPoint)
        {
            ChangeCurrentAIBehaviour(AIBehaviour.Observing);
        }
        else
        {
            ChangeCurrentAIBehaviour(AIBehaviour.Searching);
        }
    }
    public void DecreaseDetection()
    {
        timeSincePlayerLastSeen += EnemyControl.Instance.tickInterval;
        bool doesTimeExceedDelay;

        //Delay based on current ai behaviour
        switch (currentAIBehaviour)
        {
            case AIBehaviour.Roaming:
                doesTimeExceedDelay = timeSincePlayerLastSeen >= 1;
                break;
            case AIBehaviour.Observing:
                doesTimeExceedDelay = timeSincePlayerLastSeen >= 2;
                break;
            case AIBehaviour.Searching:
            default:
                doesTimeExceedDelay = timeSincePlayerLastSeen >= 5;
                break;
            case AIBehaviour.Attacking:
                doesTimeExceedDelay = false;
                break;
        }
        if (doesTimeExceedDelay)
        {
            float decreasePerTick = 100 / (timeToMaxDetect*1.5f / EnemyControl.Instance.tickInterval);
            if (detectionLevel - decreasePerTick < 0)
            {
                detectionLevel = 0;
                if(!isStatic)ChangeCurrentAIBehaviour(AIBehaviour.Roaming);
                return;
            }
            else detectionLevel -= decreasePerTick;
            if (detectionLevel < observingStateBreakPoint) ChangeCurrentAIBehaviour(AIBehaviour.Roaming);
            else if (detectionLevel >= observingStateBreakPoint && detectionLevel < searchingStateBreakPoint)
            {
                ChangeCurrentAIBehaviour(AIBehaviour.Observing);
            }
            else
            {
                ChangeCurrentAIBehaviour(AIBehaviour.Searching);
            }
        }
    }
    private float timeSincePlayerLastSeen;
}
