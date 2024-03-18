using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PickUpBox : MonoBehaviour, InteractiveObject
{
    [SerializeField] private string objectName;
    [SerializeField] private string onGroundDescription;
    [SerializeField] private string pickedUpDescription;

    private bool isBeeingHeld;

    public void Interact(InputAction.CallbackContext value)
    {
        if (!isBeeingHeld)
        {
            isBeeingHeld = true;
            ArmadilloPlayerController.Instance.pickUpControl.Grab(transform);
            ArmadilloInteractController.Instance.UpdateInteractionHUD();
            return;
        }
        isBeeingHeld = false;
        ArmadilloPlayerController.Instance.pickUpControl.Drop();
        ArmadilloInteractController.Instance.UpdateInteractionHUD();
    }

    public string GetObjectName() { return objectName; }

    public string GetObjectDescription()
    {
        return isBeeingHeld ? pickedUpDescription : onGroundDescription;
    }
}
