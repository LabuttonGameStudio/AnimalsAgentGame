using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class DoorWallClosed_Int : MonoBehaviour,InteractiveObject
{
    [SerializeField]private DoorWallOpen_Int openDoor;

    public string GetObjectName()
    {
        return "Portão";
    }
    public string GetObjectDescription()
    {
        return "Abrir";
    }
    public void Interact(InputAction.CallbackContext value)
    {
        OpenDoor();
    }
    public void OpenDoor()
    {
        gameObject.SetActive(false);
        openDoor.gameObject.SetActive(true);
    }
}
