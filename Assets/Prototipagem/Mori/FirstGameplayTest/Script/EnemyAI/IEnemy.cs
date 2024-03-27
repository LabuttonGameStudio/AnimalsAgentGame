using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

[Serializable]
public class AIPathPoint
{
    [SerializeField] public Transform transformOfPathPoint;
    [SerializeField, Tooltip("Wait time in seconds")] public float waitTimeOnPoint;
}
public abstract class IEnemy : MonoBehaviour
{
    //----- Path Finding -----
    [Header("Path Finding")]
    [HideInInspector] public NavMeshAgent navMeshAgent;
    [SerializeField, Tooltip("Static = O inimigo não se move")] protected bool isStatic;
    protected enum PathLoopTypes
    {
        DontLoop,
        Loop,
        Boomerang
    }
    [SerializeField, Tooltip(
        "Dont Loop = Para no ponto final | Loop = No ponto final retorna ao ponto incial | Boomerang = No ponto final retorna pelo caminho inverso"
        )]
    protected PathLoopTypes pathLoopType;

    private bool isBoomerangForward;

    protected int currentPathPoint = 0;


    [SerializeField] protected AIPathPoint[] aiPathList;

    //----- Stats -----
    [Header("HP")]
    [SerializeField] protected int currentHp = 50;
    [SerializeField] protected int maxHp = 50;

    //----- Visibility -----
    [Header("Field Of View")]
    [SerializeField] protected Transform eyeTransform;
    [SerializeField] protected float fieldOfView;
    [SerializeField] protected float viewDistance;
    [SerializeField] protected float viewHeight;
    [SerializeField] protected Color colorOfFovMesh;
    protected Mesh fovWedgeMesh;
    [SerializeField] protected LayerMask visionBlockLayers;

    //----- State Machine -----
    public enum AIState
    {
        Roaming,
        Observing,
        Searching,
        Attacking,
    }
    [HideInInspector] public AIState currentAIState { get; private set; }

    protected EnemyState currentEnemyState;

    protected EnemyRoamingState enemyRoamingState;
    protected EnemyObservingState enemyObservingState;
    protected EnemySearchingState enemySearchingState;
    protected EnemyAttackingState enemyAttackingState;

    [SerializeField]protected EnemyCurrentStateVisual enemyStateVisual;

    protected int currentVisibilityOfPlayer;

    private void OnDrawGizmos()
    {
        if (!fovWedgeMesh) return;
        if (ArmadilloPlayerController.Instance != null)
        {
            if (CheckForLOS(ArmadilloPlayerController.Instance.gameObject))
            {
                Color red = Color.red;
                red.a = colorOfFovMesh.a;
                Gizmos.color = red;
            }
            else Gizmos.color = colorOfFovMesh;
        }
        else Gizmos.color = colorOfFovMesh;
        Gizmos.DrawMesh(fovWedgeMesh, eyeTransform.position, eyeTransform.rotation);
    }
    private void Awake()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();

        enemyRoamingState = new EnemyRoamingState(this);
        enemyObservingState = new EnemyObservingState(this);
        enemySearchingState = new EnemySearchingState(this);
        enemyAttackingState = new EnemyAttackingState(this);

        if(isStatic)
        {
            ChangeCurrentAIState(AIState.Observing);
            return;
        }
        ChangeCurrentAIState(AIState.Roaming);

        OnAwake();
    }
    private void Start()
    {
        EnemyControl.Instance.allEnemiesList.Add(this);
        enemyStateVisual.ChangeVisualState(currentAIState);
        OnStart();
    }
    private void FixedUpdate()
    {
        OnFixedUpdate();
    }
    private Mesh CreateFovWedgeMesh()
    {
        Mesh mesh = new Mesh();

        int segments = 10;
        int numTriangles = (segments * 4) + 2 + 2;
        int numVertices = numTriangles * 3;

        Vector3[] vertices = new Vector3[numVertices];
        int[] triangles = new int[numVertices];

        Vector3 bottomCenter = Vector3.zero - Vector3.up * viewHeight / 2;
        Vector3 bottomLeft = Quaternion.Euler(0, -(fieldOfView / 2), 0) * Vector3.forward * viewDistance - Vector3.up * viewHeight / 2;
        Vector3 bottomRight = Quaternion.Euler(0, (fieldOfView / 2), 0) * Vector3.forward * viewDistance - Vector3.up * viewHeight / 2;

        Vector3 topCenter = bottomCenter + Vector3.up * viewHeight;
        Vector3 topRight = bottomRight + Vector3.up * viewHeight;
        Vector3 topLeft = bottomLeft + Vector3.up * viewHeight;

        int vert = 0;

        //Left side
        vertices[vert++] = bottomCenter;
        vertices[vert++] = bottomLeft;
        vertices[vert++] = topLeft;

        vertices[vert++] = topLeft;
        vertices[vert++] = topCenter;
        vertices[vert++] = bottomCenter;

        //Right side
        vertices[vert++] = bottomCenter;
        vertices[vert++] = topCenter;
        vertices[vert++] = topRight;

        vertices[vert++] = topRight;
        vertices[vert++] = bottomRight;
        vertices[vert++] = bottomCenter;

        float currentAngle = -(fieldOfView / 2);
        float deltaAngle = fieldOfView / segments;
        for (int i = 0; i < segments; ++i)
        {
            bottomLeft = Quaternion.Euler(0, currentAngle, 0) * Vector3.forward * viewDistance - Vector3.up * viewHeight / 2;
            bottomRight = Quaternion.Euler(0, currentAngle + deltaAngle, 0) * Vector3.forward * viewDistance - Vector3.up * viewHeight / 2;

            topLeft = bottomLeft + Vector3.up * viewHeight;
            topRight = bottomRight + Vector3.up * viewHeight;

            //Far side
            vertices[vert++] = bottomLeft;
            vertices[vert++] = bottomRight;
            vertices[vert++] = topRight;

            vertices[vert++] = topRight;
            vertices[vert++] = topLeft;
            vertices[vert++] = bottomLeft;

            //Top side
            vertices[vert++] = topCenter;
            vertices[vert++] = topLeft;
            vertices[vert++] = topRight;

            //Bottom side
            vertices[vert++] = bottomCenter;
            vertices[vert++] = bottomRight;
            vertices[vert++] = bottomLeft;

            currentAngle += deltaAngle;
        }

        for (int i = 0; i < numVertices; ++i)
        {
            triangles[i] = i;
        }

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();

        return mesh;
    }

    private void OnValidate()
    {
        fovWedgeMesh = CreateFovWedgeMesh();
    }

    public void VisibilityUpdate()
    {
        currentEnemyState.OnVisibilityUpdate();
    }
    public void ActionUpdate()
    {
        currentEnemyState.OnActionUpdate();
    }
    public void ChangeCurrentAIState(AIState nextAIState)
    {
        if(currentEnemyState != null)currentEnemyState.OnExitState();
        switch (nextAIState)
        {
            case AIState.Roaming:
                enemyRoamingState.OnEnterState();
                currentEnemyState = enemyRoamingState;
                break;
            case AIState.Observing:
                enemyObservingState.OnEnterState();
                currentEnemyState = enemyObservingState;
                break;
            case AIState.Searching:
                enemySearchingState.OnEnterState();
                currentEnemyState = enemySearchingState;
                break;
            case AIState.Attacking:
                enemyAttackingState.OnEnterState();
                currentEnemyState = enemyAttackingState;
                break;
        }
        currentAIState = nextAIState;
    }
    public void SetNextDestinationOfNavmesh()
    {
        if (navMeshAgent.SetDestination(aiPathList[currentPathPoint].transformOfPathPoint.position)) { }
        else Debug.LogError("Error in " + name + " in setting destination of point " + aiPathList[currentPathPoint].transformOfPathPoint.name);
    }
    private void NextPathPoint()
    {
        //Checka se é o caso de loopar a rota ou somente ir para o proximo ponto 
        switch (pathLoopType)
        {
            case PathLoopTypes.DontLoop:
                if (currentPathPoint + 1 >= aiPathList.Length)
                {
                    ChangeCurrentAIState(AIState.Observing);
                    return;
                }
                else
                {
                    currentPathPoint++;
                }
                break;
            case PathLoopTypes.Loop:
                if ((currentPathPoint + 1) >= aiPathList.Length)
                {
                    currentPathPoint = 0;
                }
                else currentPathPoint++;
                break;
            case PathLoopTypes.Boomerang:
                if (isBoomerangForward)
                {
                    if (currentPathPoint + 1 >= aiPathList.Length)
                    {
                        isBoomerangForward = false;
                        currentPathPoint--;
                    }
                    else currentPathPoint++;
                }
                else
                {
                    if (currentPathPoint - 1 < 0)
                    {
                        isBoomerangForward = true;
                        currentPathPoint++;
                    }
                    else currentPathPoint--;
                }
                break;

        }
        SetNextDestinationOfNavmesh();
    }
    public void CheckForProximityOfPoint()
    {
        if (navMeshAgent.remainingDistance < 1f)
        {
            if (aiPathList[currentPathPoint].waitTimeOnPoint <= 0)
            {
                NextPathPoint();
            }
            else
            {
                if (waitOnPointTimer_Ref == null) waitOnPointTimer_Ref = StartCoroutine(WaitOnPointTimer_Coroutine(aiPathList[currentPathPoint].waitTimeOnPoint));
            }
            Debug.Log("Reached point");
        }
    }
    private Coroutine waitOnPointTimer_Ref;
    private IEnumerator WaitOnPointTimer_Coroutine(float duration)
    {
        navMeshAgent.isStopped = true;
        yield return new WaitForSeconds(duration);
        NextPathPoint();
        navMeshAgent.isStopped = false;
        waitOnPointTimer_Ref = null;
    }
    public bool CheckForLOS(GameObject objectLooked)
    {
        Vector3 origin = eyeTransform.position;
        Vector3 destination = objectLooked.transform.position;
        Vector3 direction = destination - origin;

        direction.y = 0;

        if (Vector3.Distance(origin, destination) > viewDistance) return false;

        float deltaAngle = Vector3.Angle(direction, eyeTransform.forward);
        if (deltaAngle > fieldOfView / 2) return false;

        if (Physics.Linecast(origin, destination, out RaycastHit hitInfo, visionBlockLayers, QueryTriggerInteraction.Ignore))
        {
            if (hitInfo.collider.gameObject != objectLooked) return false;
        }
        return true;
    }

    public void ToggleIncreaseDetectionCoroutine(bool state)
    {
        if(state)
        {
            if (decreaseDetectionLevel_Ref != null)
            {
                StopCoroutine(decreaseDetectionLevel_Ref);
                decreaseDetectionLevel_Ref = null;
            }
            if (increaseDetectionLevel_Ref == null) increaseDetectionLevel_Ref = StartCoroutine(IncreaseDetectionLevel_Coroutine());
            return;
        }
        if(increaseDetectionLevel_Ref != null)
        {
            StopCoroutine(increaseDetectionLevel_Ref);
            increaseDetectionLevel_Ref = null;
            if (currentVisibilityOfPlayer > 0)
            {
                if (decreaseDetectionLevel_Ref == null) decreaseDetectionLevel_Ref = StartCoroutine(DecreaseDetectionLevel_Coroutine());
            }
        }
    }

    protected Coroutine increaseDetectionLevel_Ref;
    protected IEnumerator IncreaseDetectionLevel_Coroutine()
    {
        while (true)
        {
            if (currentVisibilityOfPlayer+1 > 100) currentVisibilityOfPlayer = 100;
            else currentVisibilityOfPlayer++;

            if (currentVisibilityOfPlayer.Equals(1)) enemyStateVisual.ChangeVisualState(AIState.Observing);
            else if (currentVisibilityOfPlayer.Equals(51)) enemyStateVisual.ChangeVisualState(AIState.Searching);
            else if(currentVisibilityOfPlayer.Equals(100)) enemyStateVisual.ChangeVisualState(AIState.Attacking);

            yield return new WaitForSeconds(0.015f);
        }
    }
    protected Coroutine decreaseDetectionLevel_Ref;
    protected IEnumerator DecreaseDetectionLevel_Coroutine()
    {
        while (true)
        {
            if (currentVisibilityOfPlayer - 1 <0) currentVisibilityOfPlayer = 0;
            else currentVisibilityOfPlayer--;

            if (currentVisibilityOfPlayer.Equals(0)) enemyStateVisual.ChangeVisualState(AIState.Roaming);
            else if (currentVisibilityOfPlayer.Equals(50)) enemyStateVisual.ChangeVisualState(AIState.Observing);
            else if (currentVisibilityOfPlayer.Equals(99)) enemyStateVisual.ChangeVisualState(AIState.Searching);

            yield return new WaitForSeconds(0.015f);
        }
    }

    protected abstract void OnAwake();
    protected abstract void OnStart();

    protected abstract void OnFixedUpdate();
}

