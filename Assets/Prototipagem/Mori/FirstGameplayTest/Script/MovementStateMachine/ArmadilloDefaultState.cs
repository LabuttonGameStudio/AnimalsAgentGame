using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.XInput;
using UnityEngine.UIElements;

public class ArmadilloDefaultState : MovementState
{
    ArmadilloMovementController movementCtrl;
    MovementFormStats stats;
    public override void EnterState(ArmadilloMovementController movementControl)
    {
        stats = movementControl.defaultFormStats;
        movementCtrl = movementControl;

        movementCtrl.rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
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
        Vector3 moveDirection = movementCtrl.transform.forward * movementCtrl.movementInputVector.y
            + movementCtrl.transform.right * movementCtrl.movementInputVector.x;
        Vector3 movementApplied;
        if (movementCtrl.grounded)
        {
            movementApplied = moveDirection.normalized * stats.moveSpeedMax * movementCtrl.movementTypeMultiplier * Time.fixedDeltaTime * 500;
            movementCtrl.rb.AddForce(movementApplied, ForceMode.Acceleration);
        }
        else
        {
            movementApplied = moveDirection.normalized * stats.moveSpeedMax * stats.onAirSpeedMultiplier * Time.fixedDeltaTime * 500;
            if (movementCtrl.rb.velocity.y < 0)
            {
                movementCtrl.rb.AddForce(Vector3.up * Physics.gravity.y * 2f,ForceMode.Force);
            }
            movementCtrl.rb.AddForce(movementApplied, ForceMode.Acceleration);
        }
    }
    //-----Player Jump-----
    public override void Jump(InputAction.CallbackContext value)
    {
        if (movementCtrl.readyToJump && movementCtrl.grounded)
        {
            movementCtrl.readyToJump = false;
            movementCtrl.rb.velocity = new Vector3(movementCtrl.rb.velocity.x, 0, movementCtrl.rb.velocity.z);
            movementCtrl.rb.AddForce(movementCtrl.transform.up * stats.jumpForce, ForceMode.VelocityChange);
        }
    }
    private void SpeedControl()
    {
        //Vector3 flatVel = new Vector3(movementCtrl.rb.velocity.x, 0, movementCtrl.rb.velocity.z);
        //if (flatVel.magnitude > movementCtrl.moveSpeed_Default)
        //{
        //    Vector3 limitedVel = flatVel.normalized * movementCtrl.moveSpeed_Default;
        //    movementCtrl.rb.velocity = new Vector3(limitedVel.x, movementCtrl.rb.velocity.y, limitedVel.z);
        //}
    }
}
