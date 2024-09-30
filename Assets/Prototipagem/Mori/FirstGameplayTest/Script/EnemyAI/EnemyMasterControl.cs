using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NextNavmeshPointCallParameters
{
    public Vector3 position;
    public IEnemy enemyRef;
}
public class EnemyMasterControl : MonoBehaviour
{
    public static EnemyMasterControl Instance { get; private set; }


    [HideInInspector] public List<IEnemy> allEnemiesList = new List<IEnemy>();

    public float visibilityTickInterval = 0.025f;
    private float m_visibilityTickInverval=0f;
    public float actionTickInterval = 0.025f;
    private float m_actionTickInterval = 0f;

    private Queue<NextNavmeshPointCallParameters> SetNextNavmeshPointCall_Queue = new Queue<NextNavmeshPointCallParameters>();

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
        if (state)
        {
            if (checkVisibilityLoop_Ref == null) checkVisibilityLoop_Ref = StartCoroutine(CheckVisibilityLoop_Coroutine());

            if (checkActionLoop_Ref == null) checkActionLoop_Ref = StartCoroutine(CheckActionLoop_Coroutine());
        }
        else if (checkVisibilityLoop_Ref != null)
        {
            StopCoroutine(checkVisibilityLoop_Ref);
            checkVisibilityLoop_Ref = null;

            StopCoroutine(checkActionLoop_Ref);
            checkActionLoop_Ref = null;
        }
    }

    #region Visibility Check
    private Coroutine checkVisibilityLoop_Ref;
    private IEnumerator CheckVisibilityLoop_Coroutine()
    {
        while (true)
        {
            if (allEnemiesList.Count > 0) break;
            yield return new WaitForSeconds(0.1f);
        }
        int currentEnemyIndex = 0;
        float lastTickTime = 0;
        while (true)
        {
            if (allEnemiesList[currentEnemyIndex].isDead) yield return null;
            else
            {
                allEnemiesList[currentEnemyIndex].VisibilityUpdate();
            }
            currentEnemyIndex++;
            if (currentEnemyIndex >= allEnemiesList.Count) currentEnemyIndex = 0;

            float interval = visibilityTickInterval / allEnemiesList.Count;
            m_actionTickInterval = Time.time - lastTickTime;
            float tickInterval = Mathf.Max(interval - m_actionTickInterval, 0);
            lastTickTime = Time.time;
            yield return new WaitForSeconds(tickInterval);
        }
    }
    #endregion

    #region Action Check
    private Coroutine checkActionLoop_Ref;
    private IEnumerator CheckActionLoop_Coroutine()
    {
        while (true)
        {
            if (allEnemiesList.Count > 0) break;
            yield return new WaitForSeconds(0.1f);
        }
        int currentEnemyIndex = 0;
        float lastTickTime = 0;
        while (true)
        {
            if (allEnemiesList[currentEnemyIndex].isDead) yield return null;
            else
            {
                OnNavmeshQueueCall();
                allEnemiesList[currentEnemyIndex].ActionUpdate();
            }

            currentEnemyIndex++;
            if (currentEnemyIndex >= allEnemiesList.Count) currentEnemyIndex = 0;

            float interval = actionTickInterval / allEnemiesList.Count;
            m_actionTickInterval = Time.time - lastTickTime;
            float tickInterval = Mathf.Max(interval - m_actionTickInterval, 0);
            lastTickTime = Time.time;
            yield return new WaitForSeconds(tickInterval);
        }
    }
    #endregion

    #region NavMeshCallQueue

    public void AddAIToNavmeshQueue(IEnemy enemy, Vector3 desiredPosition)
    {
        enemy.enqueued = true;
        SetNextNavmeshPointCall_Queue.Enqueue(new NextNavmeshPointCallParameters()
        {
            position = desiredPosition,
            enemyRef = enemy,
        });
    }
    public void OnNavmeshQueueCall()
    {
        if (SetNextNavmeshPointCall_Queue.Count > 0)
        {
            NextNavmeshPointCallParameters parameters = SetNextNavmeshPointCall_Queue.Peek();
            if (parameters.enemyRef.TrySetNextDestination(parameters.position))
            {
                parameters.enemyRef.enqueued = false;
            }
            SetNextNavmeshPointCall_Queue.Dequeue();
        }
    }

    #endregion
}
