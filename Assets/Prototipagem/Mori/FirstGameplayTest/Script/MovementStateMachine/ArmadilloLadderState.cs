using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class ArmadilloLadderState : MovementState
{
    ArmadilloMovementController movementCtrl;
    public Transform ladderObject;
    public Vector3 minPosition;
    public Vector3 maxPosition;
    public float pipeSize;
    Vector3 targetPosition;
    public override void EnterState(ArmadilloMovementController movementControl)
    {
        movementCtrl = movementControl;
        movementCtrl.rb.velocity = Vector3.zero;
        movementCtrl.rb.angularVelocity = Vector3.zero;
        movementCtrl.rb.useGravity = false;
        Vector3 movePosition;
        movePosition = (movementCtrl.rb.position - ladderObject.position);
        movePosition.y = 0;
        movePosition = movePosition.normalized * (0.6f + pipeSize) + ladderObject.position;
        if (movementCtrl.rb.position.y < minPosition.y)
        {
            movePosition.y = minPosition.y;
        }
        else if (movementCtrl.rb.position.y > maxPosition.y)
        {
            movePosition.y = maxPosition.y;
        }
        else
        {
            movePosition.y = movementCtrl.rb.position.y;
        }
        targetPosition = movePosition;
        ArmadilloPlayerController.Instance.inputControl.inputAction.Armadillo.Jump.performed += ExitCurrentPipe;
    }


    public override void FixedUpdateState()
    {
        MovePlayer();
    }

    public override void UpdateState()
    {

    }
    private void MovePlayer()
    {
        float height = targetPosition.y;
        targetPosition = RotatePointAroundPivot(movementCtrl.rb.position, ladderObject.position, new Vector3(0, -movementCtrl.movementInputVector.x * 10, 0));
        targetPosition.y = height;
        Vector3 movementPosition = Vector3.Lerp(movementCtrl.rb.position, targetPosition, Time.deltaTime * 10);
        if (!(movementPosition.y > maxPosition.y || movementPosition.y < minPosition.y))
        {
            movementPosition.y = movementCtrl.rb.position.y;
        }
        Vector3 vectorUpMovement = movementCtrl.movementInputVector.y * movementCtrl.ladderSpeed * Vector3.up * Time.fixedDeltaTime;
        if (!((movementPosition + vectorUpMovement).y > maxPosition.y || (movementPosition + vectorUpMovement).y < minPosition.y))
        {
            movementPosition += movementCtrl.movementInputVector.y * movementCtrl.ladderSpeed * Vector3.up * Time.fixedDeltaTime;
        }
        movementCtrl.rb.MovePosition(movementPosition);
    }
    Vector3 RotatePointAroundPivot(Vector3 point, Vector3 pivot, Vector3 angles)
    {
        point.y = 0;
        pivot.y= 0;
        Vector3 dir = (point - pivot); // get point direction relative to pivot
        dir = Quaternion.Euler(angles) * dir; // rotate it
        point = dir.normalized * (0.6f + pipeSize) + pivot; // calculate rotated point
        return point; // return it
    }
    public override void ExitState()
    {
        ArmadilloPlayerController.Instance.inputControl.inputAction.Armadillo.Jump.performed -= ExitCurrentPipe;
        movementCtrl.timeSinceTouchedGround = 0;
        movementCtrl.rb.useGravity = true;
    }
    //-----Player Jump-----
    public override void Jump()
    {

    }
    public void ExitCurrentPipe(InputAction.CallbackContext value)
    {
        movementCtrl.ExitLadderWithJump();
        ArmadilloInteractController.Instance.UpdateInteractionHUD();
    }
}
