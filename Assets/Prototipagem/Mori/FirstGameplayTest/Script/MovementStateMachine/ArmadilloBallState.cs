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
        previousVelocityInput = Vector3.zero;
        velocity = Vector3.zero;
    }

    //-----Player Movement-----
    private void MovePlayer()
    {
        Camera mainCamera = ArmadilloPlayerController.Instance.cameraControl.mainCamera;
        Vector3 inputDirection = mainCamera.transform.forward * movementCtrl.movementInputVector.y
            + mainCamera.transform.right * movementCtrl.movementInputVector.x;
        Vector3 moveDirection = inputDirection;
        moveDirection.y = 0;

        if (movementCtrl.grounded)
        {
            if(movementCtrl.isOnSlope) moveDirection = movementCtrl.GetSlopeMoveDirection(moveDirection.normalized) * stats.moveSpeedMax * 10;
            else moveDirection = moveDirection.normalized * stats.moveSpeedMax * 10;

            moveDirection = Vector3.SmoothDamp(previousVelocityInput, moveDirection, ref velocity, 1 / stats.moveSpeedAcceleration);
            movementCtrl.rb.AddForce(moveDirection, ForceMode.Acceleration);
            previousVelocityInput = moveDirection;

            if(moveDirection.magnitude>0.1f) movementCtrl.rb.MoveRotation(Quaternion.LookRotation(moveDirection));
        }
        else
        {
            Vector3 movementInAir = moveDirection.normalized * stats.moveSpeedMax * stats.onAirSpeedMultiplier * 10;
            if (movementCtrl.rb.velocity.y < 0)
            {
                movementCtrl.rb.AddForce((Vector3.up * Physics.gravity.y * 2.0f * movementCtrl.timeSinceTouchedGround * 3 / 1.5f) * (movementCtrl.rb.mass / 50), ForceMode.Acceleration);
            }
            movementInAir = Vector3.SmoothDamp(previousVelocityInput, movementInAir, ref velocity, 1 / stats.moveSpeedAcceleration);
            movementCtrl.rb.AddForce(movementInAir, ForceMode.Acceleration);
            previousVelocityInput = movementInAir;
            if (movementInAir.magnitude > 0.1f)
            {
                movementCtrl.rb.MoveRotation(Quaternion.LookRotation(movementInAir));
            }
        }
    }
    //-----Player Jump-----
    public override void Jump()
    {
        movementCtrl.readyToJump = false;
        movementCtrl.rb.velocity = new Vector3(movementCtrl.rb.velocity.x, 0, movementCtrl.rb.velocity.z);
        movementCtrl.rb.AddForce(Vector3.up * stats.jumpForce, ForceMode.VelocityChange);
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
