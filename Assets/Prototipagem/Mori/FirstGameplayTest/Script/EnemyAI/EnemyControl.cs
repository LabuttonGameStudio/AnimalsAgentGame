using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyControl : MonoBehaviour
{
    public static EnemyControl Instance { get; private set;}


    [HideInInspector]public List<IEnemy> allEnemiesList = new List<IEnemy>();

    public float visibilityTickInterval = 0.025f;
    public float actionTickInterval = 0.025f;

    private void Awake()
    {
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
            if (checkVisibilityLoop_Ref == null) checkVisibilityLoop_Ref = StartCoroutine(CheckVisibilityLoop_Coroutine());

            if (checkActionLoop_Ref == null) checkActionLoop_Ref = StartCoroutine(CheckActionLoop_Coroutine());
        }
        else if(checkVisibilityLoop_Ref != null)
        {
            StopCoroutine(checkVisibilityLoop_Ref);
            checkVisibilityLoop_Ref = null;

            StopCoroutine(checkActionLoop_Ref);
            checkActionLoop_Ref = null;
        }
    }
    private Coroutine checkVisibilityLoop_Ref;
    private IEnumerator CheckVisibilityLoop_Coroutine()
    {
        while(true)
        {
            if (allEnemiesList.Count > 0) break;
            yield return new WaitForSeconds(0.1f);
        }
        int currentEnemyIndex = 0;
        while(true)
        {
            if (allEnemiesList[currentEnemyIndex].isDead) yield return null;
            else
            {
                allEnemiesList[currentEnemyIndex].VisibilityUpdate();
            }

            currentEnemyIndex++;
            if(currentEnemyIndex>=allEnemiesList.Count)currentEnemyIndex = 0;

            float interval = visibilityTickInterval / allEnemiesList.Count;
            yield return new WaitForSecondsRealtime(interval);
        }
    }

    private Coroutine checkActionLoop_Ref;
    private IEnumerator CheckActionLoop_Coroutine()
    {
        while (true)
        {
            if (allEnemiesList.Count > 0) break;
            yield return new WaitForSeconds(0.1f);
        }
        int currentEnemyIndex = 0;
        while (true)
        {
            if (allEnemiesList[currentEnemyIndex].isDead) yield return null;
            else
            {
                allEnemiesList[currentEnemyIndex].ActionUpdate();
            }

            currentEnemyIndex++;
            if (currentEnemyIndex >= allEnemiesList.Count) currentEnemyIndex = 0;

            float interval = actionTickInterval / allEnemiesList.Count;
            yield return new WaitForSecondsRealtime(interval);
        }
    }
}
