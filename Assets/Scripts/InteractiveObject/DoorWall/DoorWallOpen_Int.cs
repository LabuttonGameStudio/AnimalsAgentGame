using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class DoorWallOpen_Int : MonoBehaviour, InteractiveObject
{
    public bool isEnabled { get; set; }
    [SerializeField] private DoorWallClosed_Int closedDoor;

    public string GetObjectName()
    {
        return "Portão";
    }
    public string GetObjectDescription()
    {
        return "Fechar";
    }
    private void Awake()
    {
        isEnabled = true;
    }
    public void Interact(InputAction.CallbackContext value)
    {
        CloseDoor();
    }
    public void CloseDoor()
    {
        gameObject.SetActive(false);
        closedDoor.gameObject.SetActive(true);
    }
}
