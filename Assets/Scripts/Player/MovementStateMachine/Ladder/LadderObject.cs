using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class LadderObject : MonoBehaviour, InteractiveObject
{
    [SerializeField] private float pipeSize;
    [SerializeField] private Transform minTransform;
    [SerializeField] private Transform maxTransform;
    public string GetObjectDescription()
    {
        ArmadilloMovementController movementController = ArmadilloPlayerController.Instance.movementControl;
        return movementController.CheckMatchOfCurrentLadder(transform) ? "Release" : "Grab";
    }

    public string GetObjectName()
    {
        return "Pipe";
    }
    private void OnDrawGizmos()
    {
        GizmosExtra.DrawCylinder(minTransform.position, transform.rotation, maxTransform.position.y - minTransform.position.y, pipeSize, Color.magenta);
    }
    public void Interact(InputAction.CallbackContext value)
    {
        ArmadilloMovementController movementController = ArmadilloPlayerController.Instance.movementControl;
        if (!movementController.CheckMatchOfCurrentLadder(transform))
        {
            movementController.EnterLadder(transform, minTransform.position, maxTransform.position,pipeSize);
        }
        else movementController.ExitLadder();
        ArmadilloInteractController.Instance.UpdateInteractionHUD();
    }

}
