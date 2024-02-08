using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InteractiveButtonOpenDoor : InteractiveObject
{
    [SerializeField] private Transform doorTransform;
    private bool isDoorOpen;
    public void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            ArmadilloInteractController.Instance.AddToInteractButtonAction(OpenDoor);
        }    

    }
    public void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            ArmadilloInteractController.Instance.RemoveToInteractButtonAction(OpenDoor);
        }
    }

    public void OpenDoor(InputAction.CallbackContext value)
    {
        if(isDoorOpen)
        {
            doorTransform.position -= Vector3.up * 6;
        }
        else
        {
            doorTransform.position += Vector3.up * 6;
        }
        isDoorOpen = !isDoorOpen;
    }

}
