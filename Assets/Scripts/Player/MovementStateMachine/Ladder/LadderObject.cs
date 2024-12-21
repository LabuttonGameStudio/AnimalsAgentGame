
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Localization;

public class LadderObject : MonoBehaviour, InteractiveObject
{
    public bool isEnabled { get; set; }
    [Header("Localization")]
    [SerializeField]private LocalizedString objectName;
    [SerializeField]private LocalizedString objectDescriptionGrab;
    [SerializeField]private LocalizedString objectDescriptionRelease;

    [SerializeField] private float pipeSize;
    [SerializeField] private Transform minTransform;
    [SerializeField] private Transform maxTransform;

    private void Awake()
    {
        isEnabled = true;
    }
    public string GetObjectDescription()
    {
        ArmadilloMovementController movementController = ArmadilloPlayerController.Instance.movementControl;
        return movementController.CheckMatchOfCurrentLadder(transform) ? objectDescriptionRelease.GetLocalizedString() : objectDescriptionGrab.GetLocalizedString();
    }

    public string GetObjectName()
    {
        return objectName.GetLocalizedString();
    }
    private void OnDrawGizmos()
    {
        GizmosExtra.DrawCylinder(minTransform.position, transform.rotation, maxTransform.position.y - minTransform.position.y, pipeSize, Color.magenta);
    }
    public void Interact(InputAction.CallbackContext value)
    {
        ArmadilloMovementController movementController = ArmadilloPlayerController.Instance.movementControl;
        if (!movementController.CheckMatchOfCurrentLadder(transform))
        {
            movementController.EnterLadder(transform, minTransform.position, maxTransform.position,pipeSize);
        }
        else movementController.ExitLadder();
        ArmadilloInteractController.Instance.UpdateInteractionHUD();
    }

}
