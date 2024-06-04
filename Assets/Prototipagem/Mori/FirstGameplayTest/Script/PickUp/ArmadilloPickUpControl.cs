using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ArmadilloPickUpControl : MonoBehaviour
{
    private Rigidbody objectRb;
    private IPickUpObject objectInLOS;
    [HideInInspector]public IPickUpObject connectedObject;
    float objectDefaultDrag;

    private Action<InputAction.CallbackContext> actionInput;
    public void AddToInteractButtonAction(IPickUpObject pickUpObject)
    {
        if (connectedObject == null && actionInput == null)
        {
            actionInput = context => InteractWithPickUpObject(context, pickUpObject);
            ArmadilloPlayerController.Instance.inputControl.inputAction.Armadillo.Interact.performed += actionInput;

            UpdateInteractionHUD(pickUpObject);
            ArmadilloUIControl.Instance.interactHUD.alpha = 1;
        }

    }

    public void UpdateInteractionHUD(IPickUpObject pickUpObject)
    {
        if (pickUpObject != null)
        {
            ArmadilloUIControl.Instance.interactItemName.text = pickUpObject.GetObjectName();
            ArmadilloUIControl.Instance.interactDescription.text = pickUpObject.GetObjectDescription();
        }
        else
        {
            ArmadilloUIControl.Instance.interactItemName.text = "";
            ArmadilloUIControl.Instance.interactDescription.text = "";
            ArmadilloUIControl.Instance.interactHUD.alpha = 0;
        }
    }
    public void UpdateInteractionHUD()
    {
        if (connectedObject != null)
        {
            ArmadilloUIControl.Instance.interactItemName.text = connectedObject.GetObjectName();
            ArmadilloUIControl.Instance.interactDescription.text = connectedObject.GetObjectDescription();
        }
        else
        {
            ArmadilloUIControl.Instance.interactItemName.text = "";
            ArmadilloUIControl.Instance.interactDescription.text = "";
            ArmadilloUIControl.Instance.interactHUD.alpha = 0;
        }
    }

    public void ClearInteractButtonAction()
    {
        if (connectedObject == null && actionInput != null)
        {
            Debug.Log("RemoveInput");
            ArmadilloPlayerController.Instance.inputControl.inputAction.Armadillo.Interact.performed -= actionInput;
            actionInput = null;
        }
    }

    #region
    public void OnObjectEnterVision(IPickUpObject pickUpObject)
    {
        objectInLOS = pickUpObject;
        AddToInteractButtonAction(objectInLOS);
    }
    public void OnObjectLeaveVision(IPickUpObject pickUpObject)
    {
        objectInLOS = null;
        UpdateInteractionHUD();
        ClearInteractButtonAction();
    }
    #endregion
    public void InteractWithPickUpObject(InputAction.CallbackContext value,IPickUpObject pickUpObject)
    {
        if(connectedObject != null)
        {
            Drop();
        }else Grab(pickUpObject);
    }
    public void Grab(IPickUpObject grabbedObject)
    {
        grabbedObject.isBeeingHeld = true;
        UpdateInteractionHUD(grabbedObject);
        connectedObject = grabbedObject;
        objectRb = ((MonoBehaviour)grabbedObject).transform.GetComponent<Rigidbody>();
        objectRb.freezeRotation = true;
        objectDefaultDrag = objectRb.drag;
        objectRb.drag = 10;
        objectRb.useGravity = false;
        ToggleHoldObjectCoroutine(true);
    }
    public void Drop()
    {
        connectedObject.isBeeingHeld = false;
        objectRb.freezeRotation = false;
        objectRb.useGravity = true;
        objectRb.drag = objectDefaultDrag;
        objectRb = null;
        ToggleHoldObjectCoroutine(false);
        ClearInteractButtonAction();
        connectedObject = null;
        if(objectInLOS != null)
        {
            AddToInteractButtonAction(objectInLOS);
            UpdateInteractionHUD(objectInLOS);
        }
        else UpdateInteractionHUD(null);
    }
    private void ToggleHoldObjectCoroutine(bool state)
    {
        if (state)
        {
            if (holdObject_Ref == null) holdObject_Ref = StartCoroutine(HoldObject_Coroutine());
        }
        else
        {
            if (holdObject_Ref != null)
            {
                StopCoroutine(holdObject_Ref);
                holdObject_Ref = null;
            }
        }
    }
    private Coroutine holdObject_Ref;
    public IEnumerator HoldObject_Coroutine()
    {
        Transform cameraTransform = ArmadilloPlayerController.Instance.cameraControl.mainCamera.transform;
        while (true)
        {
            Vector3 holdArea = cameraTransform.position + cameraTransform.forward * 3f;
            if (Vector3.Distance(holdArea, objectRb.position) > 0.1f)
            {
                Vector3 moveDirection = holdArea - objectRb.position;
                objectRb.AddForce(moveDirection*3f,ForceMode.VelocityChange);
            }
            yield return new WaitForFixedUpdate();
        }
    }
}
