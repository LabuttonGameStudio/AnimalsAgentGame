using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ArmadilloBallState : MovementState
{
    ArmadilloMovementController movementCtrl;
    MovementFormStats stats;
    Vector3 previousVelocityInput = Vector3.zero;
    private Vector3 velocity = Vector3.zero;

    private Transform playerVisual;
    public override void EnterState(ArmadilloMovementController movementControl)
    {
        stats = movementControl.ballFormStats;
        movementCtrl = movementControl;

        movementCtrl.rb.constraints = RigidbodyConstraints.None;
    }


    public override void FixedUpdateState()
    {
        MovePlayer();
    }

    public override void UpdateState()
    {
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

        if (movementCtrl.grounded)
        {
            moveDirection = moveDirection.normalized * stats.moveSpeedMax * 10;
            moveDirection = Vector3.SmoothDamp(previousVelocityInput, moveDirection, ref velocity, 1 / stats.moveSpeedAcceleration);
            movementCtrl.rb.AddForce(moveDirection, ForceMode.Acceleration);
            previousVelocityInput = moveDirection;
            movementCtrl.rb.AddTorque(new Vector3(moveDirection.z, moveDirection.y, -moveDirection.x)/10, ForceMode.VelocityChange);
        }
        else
        {
            Vector3 movementInAir = moveDirection.normalized * stats.moveSpeedMax * stats.onAirSpeedMultiplier * 10;
            if (movementCtrl.rb.velocity.y < 0)
            {
                movementInAir += Vector3.up * Physics.gravity.y * 2.0f;
            }
            movementInAir = Vector3.SmoothDamp(previousVelocityInput, movementInAir, ref velocity, 1 / stats.moveSpeedAcceleration);
            movementCtrl.rb.AddForce(movementInAir, ForceMode.Acceleration);
            previousVelocityInput = movementInAir;
            movementCtrl.rb.AddTorque(new Vector3(movementInAir.z, movementInAir.y, -movementInAir.x) / 20, ForceMode.VelocityChange);
        }
        //movementCtrl.transform.LookAt(movementCtrl.transform.position + moveDirection);
    }
    //-----Player Jump-----
    public override void Jump(InputAction.CallbackContext value)
    {
        if (movementCtrl.readyToJump && movementCtrl.grounded)
        {
            movementCtrl.readyToJump = false;
            movementCtrl.rb.velocity = new Vector3(movementCtrl.rb.velocity.x, 0, movementCtrl.rb.velocity.z);
            movementCtrl.rb.AddForce(Vector3.up * stats.jumpForce, ForceMode.VelocityChange);
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
}
