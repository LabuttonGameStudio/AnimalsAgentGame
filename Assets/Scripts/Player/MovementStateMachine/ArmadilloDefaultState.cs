using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using static ArmadilloPlayerController;
public class ArmadilloDefaultState : MovementState
{
    ArmadilloMovementController movementCtrl;
    MovementFormStats stats;
    Vector3 moveDirection;
    public override void EnterState(ArmadilloMovementController movementControl)
    {
        stats = movementControl.defaultFormStats;
        movementCtrl = movementControl;
        movementCtrl.rb.angularVelocity = Vector3.zero;
        movementCtrl.rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
    }


    public override void FixedUpdateState()
    {
        MovePlayer();
    }

    public override void UpdateState()
    {
        moveDirection = movementCtrl.transform.forward * movementCtrl.movementInputVector.y
            + movementCtrl.transform.right * movementCtrl.movementInputVector.x;
        movementCtrl.rb.useGravity = !movementCtrl.isOnSlope;
        if (movementCtrl.grounded)
        {
            if (new Vector2(movementCtrl.rb.velocity.x, movementCtrl.rb.velocity.z).magnitude > 0.5f) ArmadilloPlayerController.Instance.audioControl.PlayMovingAudio();
            else ArmadilloPlayerController.Instance.audioControl.StopMovingAudio();
        }
        else
        {
            ArmadilloPlayerController.Instance.audioControl.StopMovingAudio();

        }
    }
    public override void ExitState()
    {

    }

    //-----Player Movement-----
    private void MovePlayer()
    {
        Vector3 movementApplied;
        if (movementCtrl.grounded)
        {
            Vector3 direction;
            //Direction of movement
            if (movementCtrl.isOnSlope)
            {
                direction = movementCtrl.GetSlopeMoveDirection(moveDirection.normalized);
            }
            else
            {
                direction = moveDirection.normalized;
            }
            movementApplied = direction * Time.fixedDeltaTime * 500* stats.moveSpeedMax;

            movementApplied *= movementCtrl.sprintLurkSpeedMultiplier;

            movementApplied = movementApplied * movementCtrl.speedMultiplier;

            foreach(SpeedMultipler speedMultipler in movementCtrl.speedMultiplerList)
            {
                movementApplied *= speedMultipler.value;
            }

            //movementApplied = Vector3.Scale(sprintMult,movementApplied);
            movementCtrl.rb.AddForce(movementApplied, ForceMode.Acceleration);
        }
        else
        {
            movementApplied = moveDirection.normalized * stats.moveSpeedMax * stats.onAirSpeedMultiplier * Time.fixedDeltaTime * 500;
            movementApplied *= 1+(movementCtrl.sprintLurkSpeedMultiplier-1)/2;
            movementApplied = movementApplied * movementCtrl.speedMultiplier;
            LedgeGrab();
            if (movementCtrl.rb.velocity.y < 0)
            {
                movementCtrl.rb.AddForce((Vector3.up * Physics.gravity.y * stats.gravityMultiplier * movementCtrl.timeSinceTouchedGround / 15) * (movementCtrl.rb.mass / 50), ForceMode.Acceleration);
            }
            movementCtrl.rb.AddForce(movementApplied, ForceMode.Acceleration);
        }
    }
    //-----Player Jump-----
    public override void Jump()
    {
        movementCtrl.readyToJump = false;
        movementCtrl.rb.velocity = new Vector3(movementCtrl.rb.velocity.x, 0, movementCtrl.rb.velocity.z);
        movementCtrl.rb.AddForce(movementCtrl.transform.up * stats.jumpForce, ForceMode.VelocityChange);
    }
    private void LedgeGrab()
    {
        if (!ArmadilloPlayerController.Instance.CheckIfActionIsPossible(2)) return;
        if (movementCtrl.requireJumpInputToLedgeGrab)
        {
            if (!movementCtrl.isPressingJumpButton) return;
        }
        if (movementCtrl.requireWInputToLedgeGrab)
        {
            if (!(movementCtrl.movementInputVector.y > 0)) return;
        }
        if (!movementCtrl.canUseMultiplesLedgeGrabInSingleJump)
        {
            if (movementCtrl.hasUsedLedgeGrab) return;
        }
        RaycastHit downHit;
        Vector3 lineDownStart = (movementCtrl.transform.position + Vector3.up * (movementCtrl.maxHeightToLedgeGrab - stats.playerHeight)) + movementCtrl.transform.forward;
        Vector3 lineDownEnd = (movementCtrl.transform.position + Vector3.up * (movementCtrl.minHeightToLedgeGrab - stats.playerHeight)) + movementCtrl.transform.forward;
        Physics.Linecast(lineDownStart, lineDownEnd, out downHit, movementCtrl.whatIsClimbable, QueryTriggerInteraction.Ignore);
        Debug.DrawLine(lineDownStart, lineDownEnd, Color.magenta);
        if (downHit.collider != null)
        {
            Collider[] collidersOnPoint = Physics.OverlapSphere(downHit.point + Vector3.up, 0.25f, movementCtrl.whatIsGround, QueryTriggerInteraction.Ignore);
            Debug.DrawLine(downHit.point + Vector3.up + new Vector3(0, 0.125f), downHit.point + Vector3.up - new Vector3(0, 0.125f), Color.green);
            if (collidersOnPoint.Length > 0)
            {
                return;
            }
            movementCtrl.hasUsedLedgeGrab = true;
            movementCtrl.timeSinceTouchedGround = 0;
            Jump();
            ArmadilloPlayerController.Instance.visualControl.OnLedgeGrab();
            //}
        }
    }
}
