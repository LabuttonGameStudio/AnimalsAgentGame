using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class ArmadilloLadderState : MovementState
{
    ArmadilloMovementController movementCtrl;
    public Transform ladderObject;
    public Vector3 minPosition;
    public Vector3 maxPosition;

    Vector3 targetPosition;
    public override void EnterState(ArmadilloMovementController movementControl)
    {
        movementCtrl = movementControl;
        movementCtrl.rb.velocity = Vector3.zero;
        movementCtrl.rb.angularVelocity = Vector3.zero;
        movementCtrl.rb.useGravity = false;
        if(movementCtrl.rb.position.y< minPosition.y) targetPosition  = minPosition;
        else if (movementCtrl.rb.position.y > maxPosition.y) targetPosition = maxPosition;
        else
        {
            Vector3 movePosition = minPosition;
            movePosition.y = movementCtrl.rb.position.y;
            targetPosition = movePosition;
        }
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
        Vector3 movementPosition = Vector3.Lerp(movementCtrl.rb.position, targetPosition,Time.deltaTime*10);
        movementPosition.y = movementCtrl.rb.position.y;
        movementPosition += movementCtrl.movementInputVector.y * movementCtrl.ladderSpeed * Vector3.up * Time.fixedDeltaTime;

        //if (movementPosition.y > maxPosition.y || movementPosition.y<minPosition.y)return;
        movementCtrl.rb.MovePosition(movementPosition); 
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
    }
}
