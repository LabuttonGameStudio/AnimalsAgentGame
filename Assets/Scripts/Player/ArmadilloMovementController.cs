using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.Rendering.DebugUI;


public class SpeedMultipler
{
    public float value;
}

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

    [Header("Slope")]
    public float maxSlopeAngle;

}
public class ArmadilloMovementController : MonoBehaviour
{
    //State Machine
    MovementState currentState;

    public ArmadilloDefaultState defaultState;
    public ArmadilloLadderState ladderState;
    public ArmadilloBallState ballState;

    //Componentes
    [System.NonSerialized] public Rigidbody rb;

    //Input system
    [System.NonSerialized] public ArmadilloPlayerInputController inputController;
    [HideInInspector] public Vector2 movementInputVector;

    //Variables
    public readonly float jumpCooldown = 0.25f;

    public MovementFormStats defaultFormStats;
    public MovementFormStats ballFormStats;


    [Space][Header("Movement Helpers")][SerializeField] public float coyoteTime = 0.25f;
    [SerializeField] public float inputBuffering = 0.25f;

    [HideInInspector] public float sprintLurkSpeedMultiplier;


    [HideInInspector] public float timeSinceTouchedGround;

    public enum MovementType
    {
        Default,
        Sprinting,
        Lurking
    }
    public MovementType currentMovementType;

    [Header("Ground Check")]
    public LayerMask whatIsClimbable;
    public LayerMask whatIsGround;
    public bool readyToJump = true;
    [HideInInspector] public bool grounded;

    [HideInInspector] public bool isPressingJumpButton;
    [HideInInspector] public bool hasUsedLedgeGrab;

    [Header("Testing")]
    public bool requireWInputToLedgeGrab;
    public bool requireJumpInputToLedgeGrab;
    public bool canUseMultiplesLedgeGrabInSingleJump;

    [Space]
    [Header("Ledge Grabing")]
    public float minHeightToLedgeGrab;
    public float maxHeightToLedgeGrab;

    [Header("Slope")]
    [HideInInspector] public bool isOnSlope;
    private RaycastHit slopeHit;

    [Header("Ladder")]
    public float ladderSpeed;

    [Header("Speed Modifiers")]
    [HideInInspector] public float speedMultiplier = 1;

    public List<SpeedMultipler> speedMultiplerList;

    [HideInInspector] public bool isSprintButtonHeld;
    [HideInInspector]public bool isFiring;
    public void OnDrawGizmos()
    {
        //Check for grounded
        Gizmos.color = Color.magenta;
        if (ArmadilloPlayerController.Instance != null)
        {
            Gizmos.DrawSphere(transform.position - new Vector3(0, GetCurrentFormStats().playerHeight / 2 + 0.1f, 0), 0.25f);
        }
    }

    private void Awake()
    {
        speedMultiplerList = new List<SpeedMultipler>();
        sprintLurkSpeedMultiplier = 1;
        speedMultiplier = 1;
        readyToJump = true;
        rb = GetComponent<Rigidbody>();

        defaultState = new ArmadilloDefaultState();
        ballState = new ArmadilloBallState();
        ladderState = new ArmadilloLadderState();
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
        inputController.inputAction.Armadillo.Jump.canceled += OnJump;


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
        MovementType currentMovementType = this.currentMovementType;
        movementInputVector = value.ReadValue<Vector2>();
        SprintCheck();
        CheckOnUpdateOfMovementType(currentMovementType);
    }
    public void OnJump(InputAction.CallbackContext value)
    {
        isPressingJumpButton = value.ReadValueAsButton();
        if (isPressingJumpButton)
        {
            if (readyToJump && timeSinceTouchedGround < coyoteTime)
            {
                currentState.Jump();
            }
            else
            {
                StartJumpBuffer();
            }
        }
    }
    public void StartJumpBuffer()
    {
        if (jumpBuffer_Ref == null) jumpBuffer_Ref = StartCoroutine(JumpBuffer_Coroutine());
        else
        {
            StopCoroutine(jumpBuffer_Ref);
            jumpBuffer_Ref = null;
            jumpBuffer_Ref = StartCoroutine(JumpBuffer_Coroutine());
        }
    }
    private Coroutine jumpBuffer_Ref;
    private IEnumerator JumpBuffer_Coroutine()
    {
        float timer = 0;
        while (timer < inputBuffering)
        {
            if (readyToJump && timeSinceTouchedGround < coyoteTime)
            {
                Debug.Log("Bufferd");
                jumpBuffer_Ref = null;
                currentState.Jump();
            }
            timer += Time.deltaTime;
            yield return null;
        }
    }
    public void OnSprint(InputAction.CallbackContext value)
    {
        MovementType currentMovementType = this.currentMovementType;
        isSprintButtonHeld = value.performed;
        SprintCheck();
        CheckOnUpdateOfMovementType(currentMovementType);
    }
    public void CancelSprint()
    {
        MovementType currentMovementType = this.currentMovementType;
        isSprintButtonHeld = false;
        SprintCheck();
        CheckOnUpdateOfMovementType(currentMovementType);
    }
    public void SprintCheck()
    {
        if (movementInputVector.y >= 0.25 && isSprintButtonHeld && !isFiring)
        {
            ArmadilloPlayerController.Instance.audioControl.ChangeCurrentMovingForm(MovementType.Sprinting);
            sprintLurkSpeedMultiplier = GetCurrentFormStats().sprintSpeedMultiplier;
            currentMovementType = MovementType.Sprinting;
            ArmadilloPlayerController.Instance.weaponControl.OnRun(true);
        }
        else
        {
            if (currentMovementType == MovementType.Sprinting)
            {
                sprintLurkSpeedMultiplier = 1;
                ArmadilloPlayerController.Instance.audioControl.ChangeCurrentMovingForm(MovementType.Default);
                currentMovementType = MovementType.Default;
                ArmadilloPlayerController.Instance.weaponControl.OnRun(false);
            }
        }
    }
    public void OnLurk(InputAction.CallbackContext value)
    {
        MovementType currentMovementType = this.currentMovementType;
        if (value.performed)
        {
            this.currentMovementType = MovementType.Lurking;
            ArmadilloPlayerController.Instance.audioControl.ChangeCurrentMovingForm(MovementType.Lurking);
            sprintLurkSpeedMultiplier = GetCurrentFormStats().lurkSpeedMultiplier;
        }
        else
        {
            if (this.currentMovementType == MovementType.Lurking)
            {
                ArmadilloPlayerController.Instance.audioControl.ChangeCurrentMovingForm(MovementType.Default);
                this.currentMovementType = MovementType.Default;
                sprintLurkSpeedMultiplier = 1;
            }
        }
        CheckOnUpdateOfMovementType(currentMovementType);
    }

    public void CheckOnUpdateOfMovementType(MovementType previousMovementType)
    {
        if (currentMovementType != previousMovementType)
        {
            ArmadilloVisualControl visualControl = ArmadilloPlayerController.Instance.visualControl;
            switch (previousMovementType)
            {
                case MovementType.Default:
                    visualControl.OnWalkStop();
                    break;
                case MovementType.Sprinting:
                    visualControl.OnSprintStop();
                    break;
                case MovementType.Lurking:
                    visualControl.OnLurkStop();
                    break;
            }

            switch (currentMovementType)
            {
                case MovementType.Default:
                    visualControl.OnWalkStart();
                    break;
                case MovementType.Sprinting:
                    visualControl.OnSprintStart();
                    break;
                case MovementType.Lurking:
                    visualControl.OnLurkStart();
                    break;
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
        Vector3 groundCheckPos = transform.position - new Vector3(0, stats.playerHeight / 2f, 0);
        Collider[] colliders = Physics.OverlapSphere(groundCheckPos, 0.25f, whatIsGround, QueryTriggerInteraction.Ignore);
        bool wasGrounded = grounded;
        if (colliders.Length > 1)
        {
            grounded = true;
        }
        else if (colliders.Length == 1)
        {
            if (ArmadilloPlayerController.Instance.pickUpControl.connectedObject == null)
            {
                grounded = true;
            }
            else if (((MonoBehaviour)ArmadilloPlayerController.Instance.pickUpControl.connectedObject).gameObject == colliders[0].gameObject)
            {
                ArmadilloPlayerController.Instance.pickUpControl.Drop();
                grounded = true;
            }
            else grounded = true;
        }
        else
        {
            grounded = false;
        }
        if (grounded)
        {
            if (!wasGrounded)
            {
                ArmadilloPlayerController.Instance.audioControl.onFallSound.PlayAudio();
                ArmadilloPlayerController.Instance.visualControl.ExitOnAir();
            }
            hasUsedLedgeGrab = false;
            timeSinceTouchedGround = 0;
            rb.drag = stats.groundDrag;

            if (Physics.Raycast(groundCheckPos, Vector3.down, out slopeHit, 0.5f, whatIsGround, QueryTriggerInteraction.Ignore))
            {
                float angle = Vector3.Angle(Vector3.up, slopeHit.normal);
                isOnSlope = (angle < stats.maxSlopeAngle && angle != 0);
            }
            if (!readyToJump) StartJumpCooldown();

        }
        else
        {
            if (wasGrounded)
            {
                ArmadilloPlayerController.Instance.visualControl.EnterOnAir();
            }
            isOnSlope = false;
            timeSinceTouchedGround += Time.deltaTime;
            rb.drag = stats.airDrag;
        }
    }
    public bool CheckMatchOfCurrentLadder(Transform ladderObject)
    {
        if (ladderState.ladderObject == null || ladderObject == null) return false;
        return ReferenceEquals(ladderObject.gameObject, ladderState.ladderObject.gameObject);
    }
    public void EnterLadder(Transform objectTransform, Vector3 minPosition, Vector3 maxPosition, float pipeSize)
    {
        if (ArmadilloPlayerController.Instance.CheckIfActionIsPossible(2))
        {
            ArmadilloPlayerController.Instance.pickUpControl.Drop();
            ladderState.ladderObject = objectTransform;
            ladderState.minPosition = minPosition;
            ladderState.maxPosition = maxPosition;
            ladderState.pipeSize = pipeSize;
            ChangeState(ladderState);
            ArmadilloPlayerController.Instance.visualControl.OnClimbingStart();
        }
    }
    public void ExitLadder()
    {
        ladderState.ladderObject = null;
        ladderState.minPosition = Vector3.zero;
        ladderState.maxPosition = Vector3.zero;
        ladderState.pipeSize = 0;
        ChangeState(defaultState);
        ArmadilloPlayerController.Instance.visualControl.OnClimbingStop();
    }
    public void ExitLadderWithJump()
    {
        ladderState.ladderObject = null;
        ladderState.minPosition = Vector3.zero;
        ladderState.maxPosition = Vector3.zero;
        ladderState.pipeSize = 0;
        ChangeState(defaultState);
        defaultState.Jump();
        ArmadilloPlayerController.Instance.visualControl.OnClimbingStop();
    }
    public Vector3 GetSlopeMoveDirection(Vector3 moveDirection)
    {
        return Vector3.ProjectOnPlane(moveDirection, slopeHit.normal);
    }
    public void OnBreakObject()
    {
        if (ArmadilloPlayerController.Instance.currentForm == ArmadilloPlayerController.Form.Ball)
        {
            ballState.OnBreakObject();
        }
    }
}
