using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public interface InteractiveObject: IRaycastableInLOS
{
    public void Interact(InputAction.CallbackContext value);
    public string GetObjectName();
    public string GetObjectDescription();
    void IRaycastableInLOS.OnEnterLOS()
    {
        ArmadilloInteractController.Instance.AddToInteractButtonAction(this);
    }

    void IRaycastableInLOS.OnLeaveLOS()
    {
        ArmadilloInteractController.Instance.ClearInteractButtonAction();
    }
}
