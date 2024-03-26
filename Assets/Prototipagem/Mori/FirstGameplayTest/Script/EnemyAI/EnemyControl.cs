using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyControl : MonoBehaviour
{
    public static EnemyControl Instance { get; private set;}

    [HideInInspector]public List<IEnemy> enemiesList;

    private void Awake()
    {
        enemiesList = new List<IEnemy>();
        Instance = this;
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
        int currentEnemyIndex = 0;
        while(true)
        {
            enemiesList[currentEnemyIndex].CheckForLOS(ArmadilloPlayerController.Instance.gameObject);
            yield return null;
            enemiesList[currentEnemyIndex].MoveToNextPosition();
            currentEnemyIndex++;
            if(currentEnemyIndex>=enemiesList.Count)currentEnemyIndex = 0;
            yield return null;
        }
    }
}
