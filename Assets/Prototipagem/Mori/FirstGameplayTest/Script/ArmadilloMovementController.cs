using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ArmadilloMovementController : MonoBehaviour
{
    //Componentes
    private Rigidbody rb;


    //Input system
    private ArmadilloPlayerInputController inputController;

    //Variables
    private Vector2 movementInputVector;
    [SerializeField, Header("Stats")] private float moveSpeed = 5f;

    //Jump
    [SerializeField] private float jumpForce;
    [SerializeField] private float jumpCooldown;
    [SerializeField] private float onAirSpeedMultiplier;
    [SerializeField] private bool readyToJump;

    [SerializeField, Space] public float groundDrag;
    [SerializeField] private float airDrag;


    [Header("Ground Check")]
    [SerializeField] private float playerHeight;
    [SerializeField] private LayerMask whatIsGround;
    bool grounded;

    public void OnDrawGizmos()
    {
        //Check for grounded
        Gizmos.DrawRay(transform.position, Vector3.down * (playerHeight * 0.5f + 0.1f));
    }

    private void Awake()
    {
        readyToJump = true;
        rb = GetComponent<Rigidbody>();
    }
    private void Start()
    {
        //Salva a classe de controle do input system e liga a deteccao dos inputs
        inputController = GetComponent<ArmadilloPlayerInputController>();
        inputController.inputAction.Armadillo.Movement.Enable();
        inputController.inputAction.Armadillo.Jump.Enable();

        //Determina para que aos inputs de movimentos serem usados pelo jogador ele chamar a funcao OnMovement
        inputController.inputAction.Armadillo.Movement.performed += OnMovement;
        inputController.inputAction.Armadillo.Movement.canceled += OnMovement;

        inputController.inputAction.Armadillo.Jump.performed += Jump;
    }

    private void OnMovement(InputAction.CallbackContext value)
    {
        movementInputVector = value.ReadValue<Vector2>();
    }
    private void Update()
    {
        CheckForGrounded();
        SpeedControl();
    }
    private void FixedUpdate()
    {
        MovePlayer();
    }
    private void MovePlayer()
    {
        Vector3 moveDirection = transform.forward * movementInputVector.y + transform.right * movementInputVector.x;

        if(grounded)rb.AddForce(moveDirection.normalized * moveSpeed * 10, ForceMode.Force);
        else rb.AddForce(moveDirection.normalized * moveSpeed * 10* onAirSpeedMultiplier, ForceMode.Force);
    }
    private void SpeedControl()
    {
        Vector3 flatVel = new Vector3(rb.velocity.x, 0, rb.velocity.z);
        if (flatVel.magnitude > moveSpeed)
        {
            Vector3 limitedVel = flatVel.normalized * moveSpeed;
            rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
        }
    }

    private void CheckForGrounded()
    {
        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.1f, whatIsGround);
        if (grounded)
        {
            rb.drag = groundDrag;

            if (!readyToJump) StartJumpCooldown();
        }
        else rb.drag = airDrag;
    }

    private void Jump(InputAction.CallbackContext value)
    {
        if (readyToJump && grounded)
        {
            readyToJump = false;
            rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
            rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
        }
    }
    private void StartJumpCooldown()
    {
        if (jumpCooldownRef == null) jumpCooldownRef = StartCoroutine(JumpCooldown_Coroutine());
    }
    private Coroutine jumpCooldownRef;
    private IEnumerator JumpCooldown_Coroutine()
    {
        yield return new WaitForSeconds(jumpCooldown);
        while (!grounded)
        {
            yield return null;
        }
        jumpCooldownRef = null;
        readyToJump = true;
    }
}
