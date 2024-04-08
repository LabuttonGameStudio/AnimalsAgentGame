using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyControl : MonoBehaviour
{
    public static EnemyControl Instance { get; private set;}


    [HideInInspector]public List<IEnemy> allEnemiesList = new List<IEnemy>();
    [HideInInspector]public List<IEnemy> allAttackingEnemiesList = new List<IEnemy>();
    [HideInInspector]public List<IEnemy> allMeleeAttackingEnemiesList = new List<IEnemy>();

    [HideInInspector]public float tickInterval;

    private void Awake()
    {
        Instance = this;
        tickInterval = 0.025f;

    }
    private void Start()
    {
        ToggleCheckLoop(true);
    }
    private void ToggleCheckLoop(bool state)
    {
        if(state)
        {
            if (enemyControlLoop_Ref == null) enemyControlLoop_Ref = StartCoroutine(EnemyControlLoop_Coroutine());
        }
        else if(enemyControlLoop_Ref != null)
        {
            StopCoroutine(enemyControlLoop_Ref);
            enemyControlLoop_Ref = null;
        }
    }
    private Coroutine enemyControlLoop_Ref;
    private IEnumerator EnemyControlLoop_Coroutine()
    {
        while(true)
        {
            if (allEnemiesList.Count > 0) break;
            yield return new WaitForSeconds(0.1f);
        }
        int currentEnemyIndex = 0;
        float interval = tickInterval / allEnemiesList.Count;
        while(true)
        {
            if (allEnemiesList[currentEnemyIndex].isDead) yield return null;
            else
            {
                allEnemiesList[currentEnemyIndex].VisibilityUpdate();
                yield return null;
                allEnemiesList[currentEnemyIndex].ActionUpdate();
            }

            currentEnemyIndex++;
            if(currentEnemyIndex>=allEnemiesList.Count)currentEnemyIndex = 0;
            yield return new WaitForSecondsRealtime(interval);
        }
    }
}
