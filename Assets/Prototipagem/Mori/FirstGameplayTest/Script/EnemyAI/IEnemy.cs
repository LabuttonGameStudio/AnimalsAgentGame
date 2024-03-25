using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[Serializable]
public class AIPathPoint
{
    [SerializeField] public Transform transformOfPathPoint;
    [SerializeField, Tooltip("Wait time in seconds")] public float waitTimeOnPoint;
}
public abstract class IEnemy : MonoBehaviour
{
    [SerializeField] protected bool isStatic;
    protected enum PathLoopTypes
    {
        DontLoop,
        Loop,
        Boomerang
    }
    [SerializeField] protected PathLoopTypes pathLoopType;
    private bool isBoomerangForward;
    [SerializeField]protected int currentPathPoint = 0;
    protected bool isMoving;
    protected bool isWaitingOnPoint;
    [SerializeField] protected AIPathPoint[] aiPathList;
    [Header("HP")]
    [SerializeField] protected int currentHp = 50;
    [SerializeField] protected int maxHp = 50;

    protected NavMeshAgent navMeshAgent;

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
        EnemyControl.Instance.enemiesList.Add(this);
        if (!isStatic) MoveToNextPosition();
        OnStart();
    }
    private void FixedUpdate()
    {
        if (!isStatic) MoveToNextPosition();
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

    public void MoveToNextPosition()
    {
        if (isWaitingOnPoint) return;
        if (isMoving)
        {
            CheckForProximityOfPoint();
            return;
        }
        if (navMeshAgent.SetDestination(aiPathList[currentPathPoint].transformOfPathPoint.position))
        {
            isMoving = true;
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
                    isMoving = false;
                    isWaitingOnPoint = false;
                }
                else currentPathPoint++;

                break;
            case PathLoopTypes.Loop:
                if ((currentPathPoint + 1) >= aiPathList.Length)
                {
                    currentPathPoint = 0;
                }
                else currentPathPoint++;
                isMoving = false;
                isWaitingOnPoint = false;
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
                isMoving = false;
                isWaitingOnPoint = false;
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
                isWaitingOnPoint = true;
                isMoving = false;
                navMeshAgent.isStopped = true;
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
            if (hitInfo.rigidbody == null || !hitInfo.rigidbody.CompareTag("Player")) return false;
        }


        return true;
    }

    protected abstract void OnStart();
    protected abstract void OnAwake();

    protected abstract void OnFixedUpdate();
}

