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
    [SerializeField, Tooltip("Nivel de detecção necessario para trocar para o estado de searching")] private readonly float searchingStateBreakPoint = 50;
    #endregion

    [Header("Combat")]
    [SerializeField]
    public float primaryAttackDamage;
    
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
        //A fazer se for um inimigo sem loop fazer ele parar

        //ChangeCurrentAIBehaviour(AIBehaviour.Observing);
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
        float increasePerTick = 100 / (timeToMaxDetect / EnemyMasterControl.Instance.visibilityTickInterval);

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
    public void DecreaseDetection()
    {
        timeSincePlayerLastSeen += EnemyMasterControl.Instance.visibilityTickInterval;
        bool doesTimeExceedDelay;

        //Delay baseado no estado atual ate comecar a descer o nivel de deteccao
        switch (currentAIBehaviour)
        {
            case AIBehaviour.Roaming:
                doesTimeExceedDelay = true;
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
            //Desce o nivel de deteccao ate ele voltar ao estado roaming
            float decreasePerTick = 100 / (timeToMaxDetect*1.5f / EnemyMasterControl.Instance.visibilityTickInterval);
            if (detectionLevel - decreasePerTick < 0)
            {
                detectionLevel = 0;
                if(currentAIBehaviour != AIBehaviour.Observing) ChangeCurrentAIBehaviour(AIBehaviour.Roaming);
                return;
            }
            else detectionLevel -= decreasePerTick;
        }
    }
    private float timeSincePlayerLastSeen;
}
