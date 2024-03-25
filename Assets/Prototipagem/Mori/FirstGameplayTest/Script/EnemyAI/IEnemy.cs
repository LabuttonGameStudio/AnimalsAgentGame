using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public abstract class IEnemy : MonoBehaviour
{
    [SerializeField] protected bool isStatic;
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
                Color red= Color.red;
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
        OnStart();
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

    public bool navmesh_SetDestination(Vector3 destination)
    {
        return navMeshAgent.SetDestination(destination);
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
            if (hitInfo.rigidbody != null && !hitInfo.rigidbody.CompareTag("Player")) return false;
        }


        return true;
    }

    protected abstract void OnStart();
    protected abstract void OnAwake();
}
