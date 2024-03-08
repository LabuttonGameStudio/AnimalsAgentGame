using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ArmadilloBallState : ArmadilloBaseState
{
    ArmadilloMovementController movementCtrl;
    public override void EnterState(ArmadilloMovementController movementControl)
    {
        movementCtrl = movementControl;
        movementCtrl.inputController.inputAction.Armadillo.Movement.performed += movementCtrl.OnMovement;
        movementCtrl.inputController.inputAction.Armadillo.Movement.canceled += movementCtrl.OnMovement;
        movementCtrl.inputController.inputAction.Armadillo.Jump.performed += Jump;
    }


    public override void FixedUpdateState()
    {
        MovePlayer();
    }

    public override void UpdateState()
    {
        CheckForGrounded();
        SpeedControl();
    }
    public override void ExitState()
    {

    }

    //-----Player Movement-----
    private void MovePlayer()
    {
        Camera mainCamera = ArmadilloPlayerController.Instance.cameraControl.mainCamera;
        Vector3 moveDirection = mainCamera.transform.forward * movementCtrl.movementInputVector.y
            + mainCamera.transform.right * movementCtrl.movementInputVector.x;
        moveDirection.y = 0;
        if (movementCtrl.grounded) movementCtrl.rb.AddForce(moveDirection.normalized * movementCtrl.moveSpeed_Ball * 10, ForceMode.Force);
        else
        {
            Vector3 movementInAir = moveDirection.normalized * movementCtrl.moveSpeed_Ball * 10 * movementCtrl.onAirSpeedMultiplier_Ball;
            if (movementCtrl.rb.velocity.y < 0)
            {
                movementInAir += Vector3.up * Physics.gravity.y * 2.0f;
                Debug.Log("Gravity Applied");
            }
            movementCtrl.rb.AddForce(movementInAir, ForceMode.Force);
        }
        movementCtrl.transform.LookAt(movementCtrl.transform.position + moveDirection);
    }
    //-----Player Jump-----
    private void Jump(InputAction.CallbackContext value)
    {
        if (movementCtrl.readyToJump && movementCtrl.grounded)
        {
            movementCtrl.readyToJump = false;
            movementCtrl.rb.velocity = new Vector3(movementCtrl.rb.velocity.x, 0, movementCtrl.rb.velocity.z);
            movementCtrl.rb.AddForce(movementCtrl.transform.up * movementCtrl.jumpForce_Default, ForceMode.Impulse);
        }
    }
    private void SpeedControl()
    {
        //Vector3 flatVel = new Vector3(movementCtrl.rb.velocity.x, 0, movementCtrl.rb.velocity.z);

        //if (flatVel.magnitude > movementCtrl.maxMoveSpeed_Ball)
        //{
        //    Vector3 limitedVel = flatVel.normalized * movementCtrl.maxMoveSpeed_Ball;
        //    movementCtrl.rb.velocity = new Vector3(limitedVel.x, movementCtrl.rb.velocity.y, limitedVel.z);
        //}
    }
    private void CheckForGrounded()
    {
        movementCtrl.grounded = Physics.Raycast(movementCtrl.transform.position, Vector3.down, movementCtrl.playerHeight_Ball * 0.5f + 0.1f, movementCtrl.whatIsGround);
        if (movementCtrl.grounded)
        {
            movementCtrl.rb.drag = movementCtrl.groundDrag_Ball ;

            if (!movementCtrl.readyToJump) movementCtrl.StartJumpCooldown();
        }
        else movementCtrl.rb.drag = movementCtrl.airDrag_Ball;
    }
}
