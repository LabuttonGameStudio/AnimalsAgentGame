using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using static AIBehaviourEnums;

public class EnemyMelee : IEnemy, IDamageable
{
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

        if (isStatic) ChangeCurrentAIBehaviour(AIBehaviour.Static);
        else ChangeCurrentAIBehaviour(AIBehaviour.Roaming);
    }
    protected override void OnStart()
    {

    }
    protected override void OnFixedUpdate()
    {

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

    public void ToggleIncreaseDetectionCoroutine(bool state)
    {
        //Se for para ligar, precisa q corotina ja exista 
        if (state)
        {
            if (increaseDetectionLevel_Ref == null) increaseDetectionLevel_Ref = StartCoroutine(IncreaseDetectionLevel_Coroutine());
            return;
        }
        //Se for para desligar, precisa q corotina ja exista 
        if (increaseDetectionLevel_Ref != null)
        {
            StopCoroutine(increaseDetectionLevel_Ref);
            increaseDetectionLevel_Ref = null;
        }
    }
    //Corotina de aumentar o nivel de detecao do player
    protected Coroutine increaseDetectionLevel_Ref;
    protected IEnumerator IncreaseDetectionLevel_Coroutine()
    {
        float interval = 0.05f;
        float increasePerTick = 100 / (timeToMaxDetect / interval);
        while (true)
        {
            if (detectionLevel + increasePerTick >= 100)
            {
                detectionLevel = 100;
                ChangeCurrentAIBehaviour(AIBehaviour.Attacking);
                break;
            }
            else detectionLevel += increasePerTick;
            if (detectionLevel < 10) break;
            if (detectionLevel >= 10 && detectionLevel < 66)
            {
                ChangeCurrentAIBehaviour(AIBehaviour.Observing);
            }
            else
            {
                ChangeCurrentAIBehaviour(AIBehaviour.Searching);
            }
            yield return new WaitForSeconds(interval);
        }
        increaseDetectionLevel_Ref = null;
    }


    protected Coroutine decreaseDetectionLevel_Ref;
    protected IEnumerator DecreaseDetectionLevel_Coroutine()
    {
        while (true)
        {

            yield return new WaitForSeconds(0.015f);
        }
    }

}
