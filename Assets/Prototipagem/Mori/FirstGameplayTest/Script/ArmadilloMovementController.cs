using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


[Serializable]
public class FormMovementStats
{
    [Header("Moving")]
    public float moveSpeedAcceleration;
    public float moveSpeed = 7.5f;
    [Header("Colliders e Rigidbody")]
    public PhysicMaterial physicMaterial;
    public Collider[] playerCollider;
    public float mass;
    [Header("Jump")]
    //Jump
    public float jumpForce_Default = 15;
    public float jumpCooldown_Default = 0.25f;
    public float onAirSpeedMultiplier_Default = 0.4f;
    public float groundDrag_Default = 5;
    public float airDrag_Default = 3;
    public float playerHeight_Default;
    private GameObject playerVisual_Default;

}
public class ArmadilloMovementController : MonoBehaviour
{
    //State Machine
    ArmadilloBaseState currentState;

    public ArmadilloDefaultState defaultState;
    public ArmadilloBallState ballState;

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
        Gizmos.color = Color.magenta;
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
}
