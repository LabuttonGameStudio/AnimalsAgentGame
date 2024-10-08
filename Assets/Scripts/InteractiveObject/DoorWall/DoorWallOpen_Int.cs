using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class DoorWallOpen_Int : MonoBehaviour, InteractiveObject
{
    [SerializeField] private DoorWallClosed_Int closedDoor;

    public string GetObjectName()
    {
        return "Port�o";
    }
    public string GetObjectDescription()
    {
        return "Fechar";
    }
    public void Interact(InputAction.CallbackContext value)
    {
        gameObject.SetActive(false);
        closedDoor.gameObject.SetActive(true);
    }
}
