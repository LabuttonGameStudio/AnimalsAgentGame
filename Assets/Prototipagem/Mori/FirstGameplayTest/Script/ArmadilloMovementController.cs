using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;
using UnityEngine.InputSystem;


[Serializable]
public class MovementFormStats
{
    [Header("Moving")]
    public float moveSpeedAcceleration;
    public float moveSpeedMax = 7.5f;

    [Header("Colliders")]
    public Collider[] playerCollider;
    public float mass;

    [Header("Jump")]
    public float jumpForce = 15;
    public float onAirSpeedMultiplier = 0.4f;
    public float groundDrag = 5;
    public float airDrag = 3;
    public float gravityMultiplier;

    [Header("Height e Visual")]
    public float playerHeight;
    private GameObject playerVisual;

    [Header("Sprint e Lurk")]
    public float sprintSpeedMultiplier;
    public float lurkSpeedMultiplier;
}
public class ArmadilloMovementController : MonoBehaviour
{
    //State Machine
    MovementState currentState;

    public ArmadilloDefaultState defaultState;
    public ArmadilloBallState ballState;

    //Componentes
    [System.NonSerialized] public Rigidbody rb;

    //Input system
    [System.NonSerialized] public ArmadilloPlayerInputController inputController;
    [HideInInspector]public Vector2 movementInputVector;

    //Variables
    public readonly float jumpCooldown = 0.25f;

    public MovementFormStats defaultFormStats;
    public MovementFormStats ballFormStats;

    [HideInInspector] public float movementTypeMultiplier = 1;

    public enum MovementType
    {
        Default,
        Sprinting,
        Lurking
    }
    [HideInInspector]public MovementType currentMovementType;

    [Header("Ground Check")]
    public LayerMask whatIsGround;
    public bool readyToJump = true;
    [HideInInspector]public bool grounded;

    public void OnDrawGizmos()
    {
        //Check for grounded
        Gizmos.color = Color.magenta;
        if (ArmadilloPlayerController.Instance != null)
        {
            Gizmos.DrawSphere(transform.position - new Vector3(0, GetCurrentFormStats().playerHeight / 2 + 0.1f, 0),0.25f);
        }
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
        inputController = ArmadilloPlayerController.Instance.inputControl;
        inputController.inputAction.Armadillo.Movement.Enable();
        inputController.inputAction.Armadillo.Jump.Enable();
        inputController.inputAction.Armadillo.Sprint.Enable();
        inputController.inputAction.Armadillo.Lurk.Enable();

        inputController.inputAction.Armadillo.Movement.performed += OnMovement;
        inputController.inputAction.Armadillo.Movement.canceled += OnMovement;

        inputController.inputAction.Armadillo.Jump.performed += OnJump;


        inputController.inputAction.Armadillo.Sprint.performed += OnSprint;
        inputController.inputAction.Armadillo.Sprint.canceled += OnSprint;

        inputController.inputAction.Armadillo.Lurk.performed += OnLurk;
        inputController.inputAction.Armadillo.Lurk.canceled += OnLurk;

        ChangeState(defaultState);
    }
    public MovementFormStats GetCurrentFormStats()
    {
        switch (ArmadilloPlayerController.Instance.currentForm)
        {
            case ArmadilloPlayerController.Form.Default:
            default:
                return defaultFormStats;
            case ArmadilloPlayerController.Form.Ball:
                return ballFormStats;
        }
    }

    public void ChangeState(MovementState state)
    {
        if (currentState != null) currentState.ExitState();
        state.EnterState(this);
        currentState = state;
    }

    public void OnMovement(InputAction.CallbackContext value)
    {
        movementInputVector = value.ReadValue<Vector2>();
    }
    public void OnJump(InputAction.CallbackContext value)
    {
        currentState.Jump(value);
    }

    public void OnSprint(InputAction.CallbackContext value)
    {
        if (value.performed)
        {
            currentMovementType = MovementType.Sprinting;
            movementTypeMultiplier = GetCurrentFormStats().sprintSpeedMultiplier;
        }
        else
        {
            if (currentMovementType == MovementType.Sprinting)
            {
                currentMovementType = MovementType.Default;
                movementTypeMultiplier = 1;
            }
        }
    }
    public void OnLurk(InputAction.CallbackContext value)
    {
        if (value.performed)
        {
            currentMovementType = MovementType.Lurking;
            movementTypeMultiplier = GetCurrentFormStats().lurkSpeedMultiplier;
        }
        else
        {
            if (currentMovementType == MovementType.Lurking)
            {
                currentMovementType = MovementType.Default;
                movementTypeMultiplier = 1;
            }
        }
    }


    private void Update()
    {
        CheckForGrounded();
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
        yield return new WaitForSeconds(jumpCooldown);
        while (!grounded)
        {
            yield return null;
        }
        jumpCooldownRef = null;
        readyToJump = true;
    }
    private void CheckForGrounded()
    {
        MovementFormStats stats = GetCurrentFormStats();
        Vector3 groundCheckPos = transform.position - new Vector3(0, stats.playerHeight/2f,0);
        Collider[] colliders = Physics.OverlapSphere(groundCheckPos, 0.25f, whatIsGround, QueryTriggerInteraction.Ignore);
        grounded = colliders.Length > 0;
        if (grounded)
        {
            rb.drag = stats.groundDrag;

            if (!readyToJump) StartJumpCooldown();
        }
        else rb.drag = stats.airDrag;
    }
}
