using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ArmadilloInteractController : MonoBehaviour
{
    //Singleton
    public static ArmadilloInteractController Instance { get; private set; }

    //Input System
    ArmadilloPlayerInputController inputController;
    private void Awake()
    {
        Instance = this;
    }


    private void Start()
    {
        inputController = GetComponent<ArmadilloPlayerInputController>();

        inputController.inputAction.Armadillo.Interact.Enable();
    }

    public void AddToInteractButtonAction(System.Action<InputAction.CallbackContext> function)
    {
        inputController.inputAction.Armadillo.Interact.performed += function;
    }
    public void RemoveToInteractButtonAction(System.Action<InputAction.CallbackContext> function)
    {
        inputController.inputAction.Armadillo.Interact.performed -= function;
    }
}
