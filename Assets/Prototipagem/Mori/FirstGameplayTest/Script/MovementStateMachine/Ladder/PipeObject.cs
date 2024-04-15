using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PipeObject : MonoBehaviour, InteractiveObject
{
    [SerializeField] private Transform minTransform;
    [SerializeField] private Transform maxTransform;
    public string GetObjectDescription()
    {
        return "Grab";
    }

    public string GetObjectName()
    {
        return "Pipe";
    }

    public void Interact(InputAction.CallbackContext value)
    {
        ArmadilloMovementController movementController = ArmadilloPlayerController.Instance.movementControl;
        if (!movementController.CheckMatchOfCurrentLadder(transform))
        {
            movementController.EnterLadder(transform, minTransform.position, maxTransform.position);
        }
        else movementController.ExitLadder();
    }

}
