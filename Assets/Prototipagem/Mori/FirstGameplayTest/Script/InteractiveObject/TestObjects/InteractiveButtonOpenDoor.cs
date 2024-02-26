using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InteractiveButtonOpenDoor : MonoBehaviour, InteractiveObject
{
    [SerializeField] private string objectName;
    [SerializeField] private string interactionDescription;
    [Space]
    [SerializeField] private Transform doorTransform;
    private bool isDoorOpen;

    public void Interact(InputAction.CallbackContext value)
    {
        if (isDoorOpen)
        {
            doorTransform.position -= Vector3.up * 6;
        }
        else
        {
            doorTransform.position += Vector3.up * 6;
        }
        isDoorOpen = !isDoorOpen;
    }

    public string GetObjectName() { return objectName; }

    public string GetObjectDescription() { return interactionDescription; }
}
