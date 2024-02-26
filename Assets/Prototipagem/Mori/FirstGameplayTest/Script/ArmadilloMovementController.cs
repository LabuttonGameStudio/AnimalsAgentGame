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
    [SerializeField, Space] public float groundDrag_Ball = 2;
    [SerializeField] public float airDrag_Ball = 1;

    [SerializeField] private GameObject playerVisual_Ball;
    [SerializeField] private Collider[] playerCollider_Ball;



    [Header("Ground Check")]
    public LayerMask whatIsGround;
    public bool readyToJump = true;
    [System.NonSerialized]public bool grounded;

    public void OnDrawGizmos()
    {
        //Check for grounded
        Gizmos.DrawRay(transform.position, Vector3.down * (playerHeight_Default * 0.5f + 0.1f));
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
        Tween.MoveTransformLocalPosition(this, ArmadilloPlayerController.Instance.cameraControl.cameraFollowPoint, new Vector3(0, 1, -5f), 0.1f);
        playerVisual_Default.SetActive(false);
        playerVisual_Ball.SetActive(true);
        inputController.inputAction.Armadillo.Ability1.performed -= ChangeToBallForm;
        ChangeState(ballState);

        foreach(Collider collider in playerCollider_Ball)
        {
            collider.enabled = true;
        }
        foreach(Collider collider in playerCollider_Default)
        {
            collider.enabled = false;
        }

        inputController.inputAction.Armadillo.Ability1.performed += ReturnToDefaultForm;
    }

    private Coroutine ChangeToBallFormRef;
    private IEnumerator ChangeToBallForm_Coroutine()
    {
        return null;
    }

    public void ReturnToDefaultForm(InputAction.CallbackContext value)
    {
        Tween.MoveTransformLocalPosition(this, ArmadilloPlayerController.Instance.cameraControl.cameraFollowPoint, new Vector3(0, 0.5f, 0f), 0.1f);
        playerVisual_Ball.SetActive(false);
        playerVisual_Default.SetActive(true);
        inputController.inputAction.Armadillo.Ability1.performed -= ReturnToDefaultForm;
        ChangeState(defaultState);
        foreach (Collider collider in playerCollider_Default)
        {
            collider.enabled = true;
        }
        foreach (Collider collider in playerCollider_Ball)
        {
            collider.enabled = false;
        }
        inputController.inputAction.Armadillo.Ability1.performed += ChangeToBallForm;
    }

}
