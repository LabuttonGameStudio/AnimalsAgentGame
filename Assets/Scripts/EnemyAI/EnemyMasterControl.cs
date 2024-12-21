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
    private float m_visibilityTickInverval = 0f;
    public float actionTickInterval = 0.025f;
    private float m_actionTickInterval = 0f;

    private Queue<NextNavmeshPointCallParameters> SetNextNavmeshPointCall_Queue = new Queue<NextNavmeshPointCallParameters>();
    [Header("Melee Enemy")]
    [SerializeField] public Material defaultMeleeEnemyMat;
    [SerializeField] public Material onHitMeleeEnemyMat;

    [Header("Ranged Enemy")]
    [SerializeField] public Material defaultRangedEnemyMat;
    [SerializeField] public Material onHitRangedEnemyMat;

    [Header("Bomber Enemy")]
    [SerializeField] public Material defaultBomberEnemyMat;
    [SerializeField] public Material onHitBomberEnemyMat;

    [Header("Drops")]
    [Header("Eletric Pistol")]
    [SerializeField] public EletricPistol_Collectable eletricPistol_CollectablePrefab;
    [SerializeField] private int minEletricPistolDropCollectable;
    [SerializeField] private int maxEletricPistolDropCollectable;

    [Header("Water Gun")]
    [SerializeField] public WaterGun_Collectable waterGun_CollectablePrefab;

    [SerializeField] private int minWaterGunDropCollectable;
    [SerializeField] private int maxWaterGunDropCollectable;

    [Header("Water Gun")]
    [SerializeField] public HP_Collectable Hp_CollectablePrefab;

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
            float tickInterval = interval - Mathf.Max(m_actionTickInterval - interval, 0);
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

    #region Collectable
    public GameObject SpawnRandomCollectable(Vector3 position)
    {
        GameObject collectable;
        switch (Random.Range(0, 2))
        {
            default:
            case 0:
                eletricPistol_CollectablePrefab.ammoAmount = Random.Range(minEletricPistolDropCollectable, maxEletricPistolDropCollectable + 1);
                collectable = Instantiate(eletricPistol_CollectablePrefab, position, Quaternion.identity).gameObject;
                return collectable;
            case 1:
                waterGun_CollectablePrefab.ammoAmount = Random.Range(minWaterGunDropCollectable, maxWaterGunDropCollectable + 1);
                collectable = Instantiate(waterGun_CollectablePrefab, position, Quaternion.identity).gameObject;
                return collectable;
        }
    }
    #endregion

}
