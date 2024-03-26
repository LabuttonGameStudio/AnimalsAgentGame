using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

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
    protected NavMeshAgent navMeshAgent;
    [SerializeField,Tooltip("Static = O inimigo não se move")] protected bool isStatic;
    protected enum PathLoopTypes
    {
        DontLoop,
        Loop,
        Boomerang
    }
    [SerializeField,Tooltip(
        "Dont Loop = Para no ponto final | Loop = No ponto final retorna ao ponto incial | Boomerang = No ponto final retorna pelo caminho inverso"
        )] protected PathLoopTypes pathLoopType;

    private bool isBoomerangForward;

    protected int currentPathPoint = 0;

    protected enum AIState
    {
        WaitingForNextOrder,
        Moving,
        Waiting,
        Looking,
        FollowingPlayer,
        Attacking
    }
    [HideInInspector]protected AIState aiCurrentState;

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
        OnAwake();
    }
    private void Start()
    {
        EnemyControl.Instance.allEnemiesList.Add(this);
        if (!isStatic)
        { 
            EnemyControl.Instance.allMovableEnemiesList.Add(this);
            ChangeCurrentAIState(AIState.WaitingForNextOrder);
        }
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

    public void MovementUpdate()
    {
        OnMovementUpdateOfControl.Invoke();
    }
    private UnityEvent OnMovementUpdateOfControl = new UnityEvent();
    private void ChangeCurrentAIState(AIState nextAIState)
    {
        OnMovementUpdateOfControl.RemoveAllListeners();
        aiCurrentState = nextAIState;
        switch(nextAIState)
        {
            case AIState.WaitingForNextOrder:
                OnMovementUpdateOfControl.AddListener(SetNextDestinationOfNavmesh);
                break;
            case AIState.Moving:
                OnMovementUpdateOfControl.AddListener(CheckForProximityOfPoint);
                break;
            case AIState.Waiting:
            case AIState.Looking:
                navMeshAgent.isStopped = true;
                break;
            default:
                return;

        }
    }
    private void SetNextDestinationOfNavmesh()
    {
        if (navMeshAgent.SetDestination(aiPathList[currentPathPoint].transformOfPathPoint.position))
        {
            ChangeCurrentAIState(AIState.Moving);
        }
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
                    isStatic = true;
                    ChangeCurrentAIState(AIState.Waiting);
                }
                else
                {
                    currentPathPoint++;
                    ChangeCurrentAIState(AIState.WaitingForNextOrder);
                }
                    break;
            case PathLoopTypes.Loop:
                if ((currentPathPoint + 1) >= aiPathList.Length)
                {
                    currentPathPoint = 0;
                }
                else currentPathPoint++;
                ChangeCurrentAIState(AIState.WaitingForNextOrder);
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
                ChangeCurrentAIState(AIState.WaitingForNextOrder);
                break;

        }
    }
    private void CheckForProximityOfPoint()
    {
        if (navMeshAgent.remainingDistance < 0.1f)
        {
            if (aiPathList[currentPathPoint].waitTimeOnPoint <= 0)
            {
                NextPathPoint();
            }
            else
            {
                ChangeCurrentAIState(AIState.Waiting);
                StartCoroutine(WaitTillEndTimer_Coroutine(aiPathList[currentPathPoint].waitTimeOnPoint));
            }
            Debug.Log("Reached point");
        }
    }
    private IEnumerator WaitTillEndTimer_Coroutine(float duration)
    {
        yield return new WaitForSeconds(duration);
        NextPathPoint();
        navMeshAgent.isStopped = false;
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

    protected abstract void OnAwake();
    protected abstract void OnStart();

    protected abstract void OnFixedUpdate();
}

