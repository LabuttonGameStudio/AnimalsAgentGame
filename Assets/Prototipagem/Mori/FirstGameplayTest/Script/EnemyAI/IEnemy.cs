using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;
using static AIBehaviourEnums;

public class VisibilityCone
{
    public float fieldOfView;
    public float viewDistance;
    public float viewHeight;

    public Mesh visibilityMesh;
}

[Serializable]
public class AIPathPoint
{
    [SerializeField] public Transform transformOfPathPoint;
    [SerializeField, Tooltip("Wait time in seconds")] public float waitTimeOnPoint;
    [SerializeField, Tooltip("Se o personagem for parar no ponto ele olha ao redor")] public bool lookAroundOnPoint;

}

public abstract class IEnemy : MonoBehaviour
{
    #region Path Finding|NavMesh Variables
    [HideInInspector] public NavMeshAgent navMeshAgent;
    [Header("Path Finding | Navmesh")]
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

    [HideInInspector] public int currentPathPoint = 0;


    [SerializeField] public AIPathPoint[] aiPathList;
    #endregion

    #region EnemyStats Variables
    //----- Stats -----
    [Header("HP")]
    [SerializeField] protected int currentHp = 50;
    [SerializeField] protected int maxHp = 50;

    [Header("Attack")]
    [SerializeField] public float primaryAttackRangeInMeters = 2;
    [SerializeField] public int hitDamage = 20;

    public bool isDead;
    #endregion

    #region EnemyComponents
    [HideInInspector] public Rigidbody rb;
    #endregion

    #region Visibility Variables
    [HideInInspector] public Vector3 lastKnownPlayerPos;
    private static List<VisibilityCone> visibilityCones;

    [SerializeField] protected Material visibilityConeMaterial;
    [SerializeField] protected Material visibilityConeOnViewMaterial;

    [SerializeField] protected MeshFilter visibilityMeshFilter;
    [SerializeField] protected MeshRenderer visibilityMeshRenderer;

    [Header("Field Of View")]
    [SerializeField] protected Transform eyeTransform;
    [SerializeField] protected float fieldOfView;
    [SerializeField] protected float viewDistance;
    [SerializeField] protected float viewHeight;
    [SerializeField] protected Color colorOfFovMesh;
    protected Mesh fovWedgeMesh;
    [SerializeField] protected LayerMask visionBlockLayers;

    #endregion

    #region AI Enemy States  
    [Header("AI Behaviour")]
    [SerializeField] public AIBehaviour currentAIBehaviour;
    [SerializeField] protected EnemyBehaviourVisual enemyBehaviourVisual;
    //0-100
    //0-10 Roaming
    //10-66 Observing
    //66-99 Searching
    //100+ Attacking
    [Tooltip("De 0 a 100"),SerializeField]protected float detectionLevel;
    #endregion


    private void OnDrawGizmos()
    {
        if (Application.isPlaying)
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

            Gizmos.color = Color.red;
            Gizmos.DrawCube(transform.position + transform.forward, Vector3.one / 3);

            Gizmos.color = Color.blue;
        }
    }
    private void Awake()
    {
        if (visibilityCones == null) visibilityCones = new List<VisibilityCone>();

        if (SearchForMatchingVisibilityCones(out Mesh coneMesh))
        {
            visibilityMeshFilter.mesh = coneMesh;
            visibilityMeshRenderer.material = visibilityConeMaterial;

        }
        else
        {
            Mesh createdConeMesh = CreateFovWedgeMesh();
            visibilityCones.Add(new VisibilityCone()
            {
                fieldOfView = fieldOfView,
                viewDistance = viewDistance,
                viewHeight = viewHeight,
                visibilityMesh = createdConeMesh
            });
            visibilityMeshFilter.mesh = createdConeMesh;
            visibilityMeshRenderer.material = visibilityConeMaterial;
        }

        navMeshAgent = GetComponent<NavMeshAgent>();
        rb = GetComponent<Rigidbody>();



        OnAwake();
    }
    private void Start()
    {
        EnemyControl.Instance.allEnemiesList.Add(this);
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

    private bool SearchForMatchingVisibilityCones(out Mesh visibilityConeMesh)
    {
        foreach (VisibilityCone cone in visibilityCones)
        {
            if (cone.viewDistance == viewDistance &&
                cone.viewHeight == viewHeight &&
                cone.fieldOfView == fieldOfView)
            {
                visibilityConeMesh = cone.visibilityMesh;
                return true;
            }
        }
        visibilityConeMesh = null;
        return false;
    }

    private void OnValidate()
    {
        fovWedgeMesh = CreateFovWedgeMesh();
    }
    public void VisibilityUpdate()
    {
        OnVisibilityUpdate();
    }
    public abstract void OnVisibilityUpdate();
    public void ActionUpdate()
    {
        OnActionUpdate();
    }
    public abstract void OnActionUpdate();
    public void SetNextDestinationOfNavmesh(Vector3 position)
    {
        if (navMeshAgent.SetDestination(position)) { }
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
                    OnRoamingPathEnd();
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
        SetNextDestinationOfNavmesh(aiPathList[currentPathPoint].transformOfPathPoint.position);
    }
    protected abstract void OnRoamingPathEnd();
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
                if (waitOnPointTimer_Ref == null)
                {
                    waitOnPointTimer_Ref = StartCoroutine(WaitOnPointTimer_Coroutine(aiPathList[currentPathPoint].waitTimeOnPoint, aiPathList[currentPathPoint].lookAroundOnPoint));
                }
            }
            Debug.Log("Reached point");
        }
    }
    public void BreakOnWaitPointCoroutine()
    {
        if (waitOnPointTimer_Ref != null)
        {
            StopCoroutine(waitOnPointTimer_Ref);
            navMeshAgent.isStopped = false;
            waitOnPointTimer_Ref = null;
        }
        if (lookAround_Ref != null)
        {
            StopCoroutine(lookAround_Ref);
            navMeshAgent.updateRotation = true;
        }
    }
    private Coroutine waitOnPointTimer_Ref;
    private IEnumerator WaitOnPointTimer_Coroutine(float duration, bool lookAround)
    {
        navMeshAgent.isStopped = true;
        if (lookAround)
        {
            navMeshAgent.updateRotation = false;
            yield return new WaitForSeconds(duration / 5);
            lookAround_Ref = StartCoroutine(LookAround_Coroutine(-45, duration / 5));
            yield return lookAround_Ref;

            lookAround_Ref = StartCoroutine(LookAround_Coroutine(90, duration / 2.5f));
            yield return lookAround_Ref;

            lookAround_Ref = StartCoroutine(LookAround_Coroutine(-45, duration / 5));
            yield return lookAround_Ref;

            navMeshAgent.updateRotation = true;
        }
        else yield return new WaitForSeconds(duration);
        NextPathPoint();
        navMeshAgent.isStopped = false;
        waitOnPointTimer_Ref = null;
    }
    private Coroutine lookAround_Ref;
    public IEnumerator LookAround_Coroutine(float rotation, float duration)
    {
        float timer = 0f;
        Quaternion startLookAtRotation = transform.localRotation;
        Quaternion finalLookAtRotation = Quaternion.Euler(transform.localRotation.eulerAngles + new Vector3(0, rotation, 0));
        while (timer < duration)
        {
            transform.localRotation = Quaternion.Slerp(startLookAtRotation, finalLookAtRotation, timer / duration);
            timer += Time.deltaTime;
            yield return null;
        }
        transform.localRotation = finalLookAtRotation;
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

