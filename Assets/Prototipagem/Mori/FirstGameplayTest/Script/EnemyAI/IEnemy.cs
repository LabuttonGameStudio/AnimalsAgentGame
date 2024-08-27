using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;
using static AIBehaviourEnums;
using Random = UnityEngine.Random;
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

public abstract class IEnemy : MonoBehaviour , IRaycastableInLOS
{
    #region Path Finding|NavMesh Variables
    [HideInInspector] public NavMeshAgent navMeshAgent;

    [Header("Path Finding | Navmesh")]
    [SerializeField, Tooltip("Static = O inimigo não se move")] public bool isStatic;
    protected enum PathLoopTypes
    {
        DontLoop,
        Loop,
        Boomerang
    }
    [SerializeField, Tooltip(
        "Dont Loop = Para no ponto final |" +
        " Loop = No ponto final retorna ao ponto incial |" +
        " Boomerang = No ponto final retorna pelo caminho inverso"
        )]
    protected PathLoopTypes pathLoopType;

    //Apenas usado se o looping eh do tipo boomerang
    private bool isBoomerangForward;

    //Etapa do caminho onde o inimigo esta atualmente
    public int currentPathPoint = 0;

    //Na proximo update do Enemy Master Control ele ira calcular a proxima rota desse inimigo
    [HideInInspector] public bool enqueued;

    //Rota que o inimigo ira percorrer
    [SerializeField] public AIPathPoint[] aiPathList;
    #endregion

    #region EnemyStats Variables
    //HP Control
    [Header("HP")]
    [SerializeField] protected float currentHp = 50;
    [SerializeField] protected float maxHp = 50;

    //Damage Reduction(DR)
    protected float currentDamageReduction;

    //Is Dead
    public bool isDead;

    //Is on Alert
    public bool isOnAlert;
    #endregion

    #region EnemyComponents
    [HideInInspector] public Rigidbody rb;
    [Header("Components")]
    public Animator animator;
    #endregion

    #region Visibility Variables
    //Lista statica de cones de visibilidade para apenas instanciar se caso existir 2 iguais
    private static List<VisibilityCone> visibilityCones;

    //Ultima posicao conhecida do jogador
    [HideInInspector] public Vector3 lastKnownPlayerPos;

    [Header("Visibility Variables")]
    //Material do cone de visibilidade 
    [SerializeField] protected Material visibilityConeMaterial;
    [SerializeField] protected Material visibilityConeOnViewMaterial;

    //Render de mesh e material do cone de visibilidade
    [SerializeField] protected MeshFilter visibilityMeshFilter;
    [SerializeField] protected MeshRenderer visibilityMeshRenderer;

    [Header("Field Of View")]
    //Ponto do olho do inimigo
    [SerializeField] protected Transform eyeTransform;

    //Campo de visao do inimigo
    [SerializeField] protected float fieldOfView;

    //Distancia do campo de visao do inimigo
    [SerializeField] protected float viewDistance;

    //Altura da checagem de visibilidade do inimigo
    [SerializeField] protected float viewHeight;

    //Camadas que bloqueiam a visibilidade do inimigo 
    [SerializeField] protected LayerMask visionBlockLayers;

    [Header("Testing")]
    //Cor que o gizmos usa para renderizar o campo de visao do inimigo
    [SerializeField] protected Color colorOfFovMesh;
    //Mesh temporaria criada para mostrar nos gizmos
    protected Mesh fovWedgeMesh;

    #endregion

    #region AI Enemy States  
    [Header("AI Behaviour")]
    //Estado principal atual de behaviour
    [SerializeField] public AIBehaviour currentAIBehaviour;
    //Visual Referente ao estado atual do inimigo
    [SerializeField] protected EnemyBehaviourVisual enemyBehaviourVisual;

    //Nivel de deteccao do inimigo(0-100), determina o tipo de behaviour dele 
    //0 Roaming
    //1-50 Observing
    //51-100 Searching
    //100+ Attacking
    [Tooltip("De 0 a 100"), Range(0, 100)] protected float detectionLevel;
    protected float detectionLevelDecreased;
    #endregion


    private void OnDrawGizmos()
    {
        //Desenha o gizmos da area onde o inimigo ira ver
        if (!Application.isPlaying)
        {
            //Visibility Mesh
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

            //See navmesh point
            if (aiPathList != null && aiPathList.Length > 0)
            {
                navMeshAgent = GetComponent<NavMeshAgent>();

                foreach (AIPathPoint aiPathPoint in aiPathList)
                {
                    GizmosExtra.DrawCylinder(aiPathPoint.transformOfPathPoint.position - new Vector3(0, navMeshAgent.height / 2, 0), Quaternion.identity, navMeshAgent.height, navMeshAgent.radius, Color.red);
                }

            }
        }
    }
    private void Awake()
    {
        //Se for o primeiro inimigo a existir cria a lista de cones de visibilidade para armazenar eles e poder reusar depois
        if (visibilityCones == null) visibilityCones = new List<VisibilityCone>();

        //Procura na lista se existe algum cone com os requerimentos do atual
        if (SearchForMatchingVisibilityCones(out Mesh coneMesh))
        {
            //Se caso existir ele resgata a informacao da lista e aloca a mesh e o material
            visibilityMeshFilter.mesh = coneMesh;
            visibilityMeshRenderer.material = visibilityConeMaterial;

        }
        else
        {
            //Se caso nao existir ele gera uma nova mesh baseado nos requerimentos do campo de visibilidade atual
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

        //Armazena a informacao do NavmeshAgent
        navMeshAgent = GetComponent<NavMeshAgent>();

        //Desliga a rotacao automatica do NavmeshAgent, ja que nessa IA sera usada rotacao manual
        navMeshAgent.updateRotation = false;

        //Armazena a informacao do Rigidbody
        rb = GetComponent<Rigidbody>();

        //Invoca a funcao de Awake baseado no tipo de inimigo
        OnAwake();
    }
    protected abstract void OnAwake();
    private void Start()
    {
        //Para receber os updates do controlador geral de inimigos, se adiciona em uma lista de inimigos
        EnemyMasterControl.Instance.allEnemiesList.Add(this);

        //Baseado no tipo do inimigo roda o StartDele
        OnStart();
    }
    protected abstract void OnStart();
    private void FixedUpdate()
    {
        //Baseado no tipo de inimigo roda o FixedUpdate dele
        OnFixedUpdate();
    }
    protected abstract void OnFixedUpdate();

    #region VisibilityCone
    /// <summary>
    /// Cria a mesh de visibilidade do inimigo
    /// </summary>
    /// <returns></returns>
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
    /// <summary>
    /// Procura por uma mesh de visibilidade compativel com a requerida
    /// </summary>
    /// <param name="visibilityConeMesh"></param>
    /// <returns></returns>
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
        //Ao atualizar as variaveis do inimigo atualiza a mesh para ver se teve alguma mudanca
        fovWedgeMesh = CreateFovWedgeMesh();
    }
    #endregion

    #region Visibility
    public void VisibilityUpdate()
    {
        //Intervalado pelo tick do Controlador geral dos inimigos, roda uma checagem para detectar oq o inimigo ve
        //Baseado no tipo do inimigo
        OnVisibilityUpdate();
    }
    public abstract void OnVisibilityUpdate();
    #endregion

    #region Actions
    public void ActionUpdate()
    {
        //Intervalado pelo tick do Controlador geral dos inimigos, roda uma checagem para determinar oq o inimigo fara nesse momento
        //Baseado no tipo do inimigo
        OnActionUpdate();
    }
    public abstract void OnActionUpdate();
    #endregion

    #region NavMesh
    /// <summary>
    /// Tenta definir um novo destino para o inimigo seguir
    /// </summary>
    /// <param name="position">posicao requesitada</param>
    /// <returns></returns>
    public bool TrySetNextDestination(Vector3 position)
    {
        if (navMeshAgent.SetDestination(position))
        {
            return true;
        }
        else
        {
            Debug.LogError("Error in " + name + " in setting destination of point " + aiPathList[currentPathPoint].transformOfPathPoint.name);
            return false;
        }
    }

    /// <summary>
    /// Baseado no tipo de looping e na progressao da rota, retorna o proximo ponto
    /// </summary>
    /// <returns></returns>
    public Vector3 NextPathPoint()
    {
        //Checka se é o caso de loopar a rota ou somente ir para o proximo ponto 
        switch (pathLoopType)
        {
            case PathLoopTypes.DontLoop:
                if (currentPathPoint + 1 >= aiPathList.Length)
                {
                    OnRoamingPathEnd();
                    return Vector3.zero;
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
        return aiPathList[currentPathPoint].transformOfPathPoint.position;
    }
    protected abstract void OnRoamingPathEnd();

    /// <summary>
    /// Checa se o ponto de destino atual esta proximo
    /// </summary>
    /// <returns></returns>
    public bool CheckForProximityOfPoint()
    {
        Vector3 currentPos = transform.position;
        Vector3 destinationPos = navMeshAgent.destination;
        return Vector3.Distance(currentPos, destinationPos) < navMeshAgent.height / 2 + 0.25f;
    }
    #endregion

    #region WaitOnPoint
    /// <summary>
    /// Comeca o processa de esperar parado em um ponto determinado
    /// </summary>
    /// <param name="duration"></param>
    public void StartWaitOnPoint(float duration)
    {
        //Se caso ele ja estiver esperando, reseta o timer atual
        if (waitOnPoint_Ref != null)
        {
            StopCoroutine(waitOnPoint_Ref);
        }
        //Inicia um novo timer de espera
        waitOnPoint_Ref = StartCoroutine(WaitOnPoint_Coroutine(duration));
    }

    //Coroutina de esperar em um ponto
    public Coroutine waitOnPoint_Ref;
    public IEnumerator WaitOnPoint_Coroutine(float duration)
    {
        navMeshAgent.isStopped = true;
        yield return new WaitForSeconds(duration);
        navMeshAgent.isStopped = false;
        waitOnPoint_Ref = null;
    }


    /// <summary>
    /// Quebra o estado de parado do inimigo
    /// </summary>
    public void StopWaitOnPoint()
    {
        if (waitOnPoint_Ref != null)
        {
            StopCoroutine(waitOnPoint_Ref);
            waitOnPoint_Ref = null;
            navMeshAgent.isStopped = false;
        }
    }
    #endregion

    #region Look
    /// <summary>
    /// Tenta iniciar o processo do inimigo de olhar em volta
    /// </summary>
    /// <param name="duration"></param>
    /// <param name="coroutine"></param>
    /// <returns></returns>
    public bool TryStartRandomLookAround(float duration, out Coroutine coroutine)
    {
        //Se caso ja estiver olhando em volta no momento retorna falso
        if (lookAround_Ref != null)
        {
            coroutine = null;
            return false;
        }

        //Se caso eh possivel, escolhe aleatoriamente entre 2 tipos de olhar em volta
        if (Random.value > 0.5f)
        {
            lookAround_Ref = StartCoroutine(LookAround_A_Coroutine(duration));
        }
        else
        {
            lookAround_Ref = StartCoroutine(LookAround_B_Coroutine(duration));
        }
        //Retorna verdadeiro e mostra a coroutina de olhar em volta
        coroutine = lookAround_Ref;
        return true;
    }

    //Coroutinas de olhar em volta 
    public Coroutine lookAround_Ref;

    //Olha em volta 45 graus para cada lado
    public IEnumerator LookAround_A_Coroutine(float duration)
    {
        navMeshAgent.isStopped = true;
        int randomDirection = Random.Range(0, 2);
        randomDirection = (randomDirection * 2) - 1;

        float durationPerAction = duration / 8;
        yield return new WaitForSeconds(durationPerAction);

        //Look Side 0
        animator.SetBool("isWalking", true);
        lerpRotate_Ref = StartCoroutine(LerpRotate_Coroutine(-45 * randomDirection, durationPerAction));
        yield return lerpRotate_Ref;
        animator.SetBool("isWalking", false);
        yield return new WaitForSeconds(durationPerAction);

        //Look Side 1
        animator.SetBool("isWalking", true);
        lerpRotate_Ref = StartCoroutine(LerpRotate_Coroutine(90 * randomDirection, 2 * durationPerAction));
        yield return lerpRotate_Ref;
        animator.SetBool("isWalking", false);
        yield return new WaitForSeconds(durationPerAction);

        //Return to center
        animator.SetBool("isWalking", true);
        lerpRotate_Ref = StartCoroutine(LerpRotate_Coroutine(-45 * randomDirection, durationPerAction));
        yield return lerpRotate_Ref;
        animator.SetBool("isWalking", false);
        yield return new WaitForSeconds(durationPerAction);

        navMeshAgent.isStopped = false;
        lookAround_Ref = null;
    }

    //Olha em volta 90 graus para cada lado
    public IEnumerator LookAround_B_Coroutine(float duration)
    {
        navMeshAgent.isStopped = true;
        int randomDirection = Random.Range(0, 2);
        randomDirection = (randomDirection * 2) - 1;

        float durationPerAction = duration / 8;
        yield return new WaitForSeconds(durationPerAction);

        //Look Side 0
        animator.SetBool("isWalking", true);
        lerpRotate_Ref = StartCoroutine(LerpRotate_Coroutine(-90 * randomDirection, durationPerAction));
        yield return lerpRotate_Ref;
        animator.SetBool("isWalking", false);
        yield return new WaitForSeconds(durationPerAction);

        //Look Side 1
        animator.SetBool("isWalking", true);
        lerpRotate_Ref = StartCoroutine(LerpRotate_Coroutine(90 * randomDirection,durationPerAction));
        yield return lerpRotate_Ref;

        lerpRotate_Ref = StartCoroutine(LerpRotate_Coroutine(90 * randomDirection,durationPerAction));
        yield return lerpRotate_Ref;
        animator.SetBool("isWalking", false);
        yield return new WaitForSeconds(durationPerAction);

        //Return to center
        animator.SetBool("isWalking", true);
        lerpRotate_Ref = StartCoroutine(LerpRotate_Coroutine(-90 * randomDirection, durationPerAction));
        yield return lerpRotate_Ref;
        animator.SetBool("isWalking", false);
        yield return new WaitForSeconds(durationPerAction);

        navMeshAgent.isStopped = false;
        lookAround_Ref = null;
    }

    /// <summary>
    /// Quebra o estado de olhar em volta 
    /// </summary>
    public void StopLookAround()
    {
        if (lookAround_Ref != null)
        {
            StopCoroutine(lookAround_Ref);
            lookAround_Ref = null;
            if (lerpRotate_Ref != null)
            {
                StopCoroutine(lerpRotate_Ref);
                lerpRotate_Ref = null;
            }
        }
    }
    public void LookAt(Vector3 position)
    {
        Vector3 direction = position - transform.position;
        direction.y = 0;
        Quaternion lookAtRotation = Quaternion.LookRotation(direction);
        transform.rotation = lookAtRotation;
    }
    /// <summary>
    /// Faz lerping em fixed update de olhar na direcao
    /// </summary>
    /// <param name="direction"></param>
    public void LerpLookAt(Vector3 position,float velocity)
    {
        Vector3 direction = position - transform.position;
        direction.y = 0;
        Quaternion lookAtRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Lerp(transform.rotation, lookAtRotation,velocity* 3 * Time.fixedDeltaTime);
    }

    //Corotina de rodar para um lado 
    private Coroutine lerpRotate_Ref;
    public IEnumerator LerpRotate_Coroutine(float rotation, float duration)
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
    #endregion

    #region Line Of Sight
    //A refazer

    /// <summary>
    /// Checa se o inimigo consegue ver o objeto
    /// </summary>
    /// <param name="objectLooked"></param>
    /// <returns></returns>
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

    /// <summary>
    /// Checa se o inimigo consegue ver o player
    /// </summary>
    /// <returns></returns>
    public float CheckForPlayerLOS()
    {
        ArmadilloPlayerController player = ArmadilloPlayerController.Instance;
        Transform playerTransform = player.transform;

        Vector3 origin = eyeTransform.position;
        Vector3 destination = playerTransform.position;
        Vector3 direction = destination - origin;

        direction.y = 0;
        //Check For Distance
        if (Vector3.Distance(origin, destination) > viewDistance) return 0;

        //Check for Angle
        float deltaAngle = Vector3.Angle(direction, eyeTransform.forward);
        if (deltaAngle > fieldOfView / 2) return 0;

        //Check for direct Line of sight
        if (Physics.Linecast(origin, destination, out RaycastHit hitInfo, visionBlockLayers, QueryTriggerInteraction.Ignore))
        {
            if (!hitInfo.collider.CompareTag("Player")) return 0;
        }
        return player.GetCurrentVisibilityOfPlayer();
    }
    #endregion

    #region Alert
    public void ToggleAlert(bool state)
    {
        isOnAlert = state;
        enemyBehaviourVisual.ToggleAlert(state);
    }
    #endregion

    void IRaycastableInLOS.OnEnterLOS()
    {

    }

    void IRaycastableInLOS.OnLeaveLOS()
    {

    }
}

