using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InteractiveToggle : MonoBehaviour, InteractiveObject
{
    [SerializeField] private string objectName;

    [SerializeField] private bool isEnabled;

    [SerializeField] private string onEnableDescription;
    [SerializeField] private string onDisabledDescription;
    public string GetObjectDescription()
    {
        return isEnabled ? onEnableDescription : onDisabledDescription;
    }

    public string GetObjectName()
    {
        return objectName;
    }

    public void Interact(InputAction.CallbackContext value)
    {
        isEnabled = !isEnabled;
        ArmadilloInteractController.Instance.UpdateInteractionHUD();
    }
}
