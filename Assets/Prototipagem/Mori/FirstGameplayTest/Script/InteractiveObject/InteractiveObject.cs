using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public interface InteractiveObject
{
    public void Interact(InputAction.CallbackContext value);
    public string GetObjectName();
    public string GetObjectDescription();

}
