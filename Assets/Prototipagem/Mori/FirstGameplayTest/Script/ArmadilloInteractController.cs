using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class ArmadilloInteractController : MonoBehaviour
{
    //Singleton
    public static ArmadilloInteractController Instance { get; private set; }

    [SerializeField] private float interactionRange = 3f;
    //Input System

    //Interaction Layer
    [SerializeField] private LayerMask whatIsInteractive;

    private InteractiveObject connectedInteractiveObject;

    private void Awake()
    {
        Instance = this;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, interactionRange);
    }

    public void AddToInteractButtonAction(InteractiveObject interactiveObject)
    {
        connectedInteractiveObject = interactiveObject;
        ArmadilloPlayerController.Instance.inputControl.inputAction.Armadillo.Interact.performed += interactiveObject.Interact;

        UpdateInteractionHUD();
        ArmadilloUIControl.Instance.interactHUD.alpha = 1;

    }

    public void UpdateInteractionHUD()
    {
        if (connectedInteractiveObject != null)
        {
            ArmadilloUIControl.Instance.interactItemName.text = connectedInteractiveObject.GetObjectName();
            ArmadilloUIControl.Instance.interactDescription.text = connectedInteractiveObject.GetObjectDescription();
        }
    }

    public void ClearInteractButtonAction()
    {
        if (connectedInteractiveObject != null)
        {
            ArmadilloPlayerController.Instance.inputControl.inputAction.Armadillo.Interact.performed -= connectedInteractiveObject.Interact;
            connectedInteractiveObject = null;
        }

        ArmadilloUIControl.Instance.interactItemName.text = "";
        ArmadilloUIControl.Instance.interactDescription.text = "";
        ArmadilloUIControl.Instance.interactHUD.alpha = 0;
    }

}
