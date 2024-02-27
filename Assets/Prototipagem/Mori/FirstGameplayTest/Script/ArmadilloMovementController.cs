using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ArmadilloMovementController : MonoBehaviour
{
    //State Machine
    ArmadilloBaseState currentState;

    ArmadilloDefaultState defaultState;
    ArmadilloBallState ballState;

    //Componentes
    [System.NonSerialized]public Rigidbody rb;


    //Input system
    [System.NonSerialized] public ArmadilloPlayerInputController inputController;

    //-----Default Form-----
    //Variables
    [System.NonSerialized]public Vector2 movementInputVector;
    [Header(" Default Form Stats")] public float moveSpeed_Default = 7.5f;

    //Jump
    [SerializeField] public float jumpForce_Default = 15;
    [SerializeField] public float jumpCooldown_Default = 0.25f;
    [SerializeField] public float onAirSpeedMultiplier_Default = 0.4f;

    [SerializeField, Space] public float groundDrag_Default = 5;
    [SerializeField] public float airDrag_Default = 3;
    [SerializeField] public float playerHeight_Default;

    [SerializeField] private GameObject playerVisual_Default;
    [SerializeField] private Collider[] playerCollider_Default;

    //----- Ball Form------

    [SerializeField, Header(" Ball Form Stats")] public float moveSpeed_Ball = 1f;
    [SerializeField] public float maxMoveSpeed_Ball = 10f;

    [SerializeField] public float onAirSpeedMultiplier_Ball = 0.6f;

    [SerializeField, Space] public float groundDrag_Ball = 2;
    [SerializeField] public float airDrag_Ball = 1;
    public float playerHeight_Ball;

    [SerializeField] private GameObject playerVisual_Ball;
    [SerializeField] private Collider[] playerCollider_Ball;



    [Header("Ground Check")]
    public LayerMask whatIsGround;
    public bool readyToJump = true;
    [System.NonSerialized]public bool grounded;

    public void OnDrawGizmos()
    {
        //Check for grounded
        Gizmos.DrawRay(transform.position, Vector3.down * (playerHeight_Ball * 0.5f + 0.1f));
    }

    private void Awake()
    {
        readyToJump = true;
        rb = GetComponent<Rigidbody>();

        defaultState = new ArmadilloDefaultState();
        ballState = new ArmadilloBallState();
    }
    private void Start()
    {
        //Salva a classe de controle do input system e liga a deteccao dos inputs
        inputController = GetComponent<ArmadilloPlayerInputController>();
        inputController.inputAction.Armadillo.Movement.Enable();
        inputController.inputAction.Armadillo.Jump.Enable();

        inputController.inputAction.Armadillo.Ability1.Enable();
        inputController.inputAction.Armadillo.Ability1.performed += ChangeToBallForm;

        ChangeState(defaultState);
    }

    public void ChangeState(ArmadilloBaseState state)
    {
        if(currentState != null)currentState.ExitState();
        state.EnterState(this);
        currentState = state;
    }

    public void OnMovement(InputAction.CallbackContext value)
    {
        movementInputVector = value.ReadValue<Vector2>();
    }
    private void Update()
    {
        currentState.UpdateState();
    }
    private void FixedUpdate()
    {
        currentState.FixedUpdateState();
    }
    
    //----- Jump Cooldown -----
    public void StartJumpCooldown()
    {
        if (jumpCooldownRef == null) jumpCooldownRef = StartCoroutine(JumpCooldown_Coroutine());
    }
    public Coroutine jumpCooldownRef;
    public IEnumerator JumpCooldown_Coroutine()
    {
        //Espera o tempo de cooldown apos o pulo pra checar se esta no chao pra evitar que durante o subir do pulo ele detecte o chao
        yield return new WaitForSeconds(jumpCooldown_Default);
        while (!grounded)
        {
            yield return null;
        }
        jumpCooldownRef = null;
        readyToJump = true;
    }

    //------ Change Forms ------
    public void ChangeToBallForm(InputAction.CallbackContext value)
    {
        if (changeToBallFormRef == null) changeToBallFormRef = StartCoroutine(ChangeToBallForm_Coroutine());
    }

    private Coroutine changeToBallFormRef;
    private IEnumerator ChangeToBallForm_Coroutine()
    {
        //Move a camera para terceira pessoa em 0.2 segundos 
        Tween.MoveTransformLocalPosition(this, ArmadilloPlayerController.Instance.cameraControl.cameraFollowPoint, new Vector3(0, 1, -5f), 0.2f);
        //Muda o visual do personagem, futuramente so colocar a opcao de mudar a animacao do model
        playerVisual_Default.SetActive(false);
        playerVisual_Ball.SetActive(true);
        //Retira a funcao do input de transformar em bola e altera a state machine pra interpretar como o modo bola
        inputController.inputAction.Armadillo.Ability1.performed -= ChangeToBallForm;
        ChangeState(ballState);

        foreach (Collider collider in playerCollider_Ball)
        {
            collider.enabled = true;
        }
        foreach (Collider collider in playerCollider_Default)
        {
            collider.enabled = false;
        }
        //Espera 0.5 segundos pra dar tempo pro model do player em modo bola encostar o chao, tempo de travessia da camera e evitar spam de transformacao 
        yield return new WaitForSeconds(0.5f);
        inputController.inputAction.Armadillo.Ability1.performed += ReturnToDefaultForm;

        changeToBallFormRef=null;
    }

    public void ReturnToDefaultForm(InputAction.CallbackContext value)
    {
        if (returnToDefaultFormRef == null) returnToDefaultFormRef = StartCoroutine(ReturnToDefaultForm_Coroutine());
    }

    private Coroutine returnToDefaultFormRef;
    private IEnumerator ReturnToDefaultForm_Coroutine()
    {
        //Move a camera para terceira pessoa em 0.2 segundos 
        Tween.MoveTransformLocalPosition(this, ArmadilloPlayerController.Instance.cameraControl.cameraFollowPoint, new Vector3(0, 0.5f, 0f), 0.2f);
        //Muda o visual do personagem, futuramente so colocar a opcao de mudar a animacao do model
        playerVisual_Ball.SetActive(false);
        playerVisual_Default.SetActive(true);
        //Retira a funcao do input de transformar no modo normal e altera a state machine pra interpretar como modo normal
        inputController.inputAction.Armadillo.Ability1.performed -= ReturnToDefaultForm;
        ChangeState(defaultState);
        //Aplica uma forca para cima para dar espaco pra troca de collider para de capsula 
        rb.AddForce(transform.up * 5f,ForceMode.Impulse);
        //Espera 0.25 segundos para a forca ter tempo de mover o objeto e o tempo de travessia da camera
        yield return new WaitForSeconds(0.25f);
        foreach (Collider collider in playerCollider_Default)
        {
            collider.enabled = true;
        }
        foreach (Collider collider in playerCollider_Ball)
        {
            collider.enabled = false;
        }
        //Espera 0.25 segundos para deixar o objeto cair ate encostar no chao e evitar spam de transformacao 
        yield return new WaitForSeconds(0.25f);
        inputController.inputAction.Armadillo.Ability1.performed += ChangeToBallForm;
        returnToDefaultFormRef = null;
    }
}
