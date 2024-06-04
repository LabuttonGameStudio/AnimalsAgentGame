using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static IPickUpObject;

public class ArmadilloPickUpControl : MonoBehaviour
{
    private Rigidbody objectRb;
    private IPickUpObject objectInLOS;
    [HideInInspector] public IPickUpObject connectedObject;
    float objectDefaultDrag;

    private Action<InputAction.CallbackContext> actionInput;

    public bool CanJumpOnConnectedObject()
    {
        if (connectedObject != null)
        {
            switch(connectedObject.m_pickUpObjectType)
            {
                default:
                case PickUpObjectType.Small:
                case PickUpObjectType.Medium:
                    return false;
                case PickUpObjectType.Big:
                    return true;
            }
        }
        else return true;
    }
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

    #region Vision
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
    public void InteractWithPickUpObject(InputAction.CallbackContext value, IPickUpObject pickUpObject)
    {
        if (connectedObject != null)
        {
            Drop();
        }
        else Grab(pickUpObject);
    }
    public void Grab(IPickUpObject grabbedObject)
    {
        Debug.Log(grabbedObject.m_pickUpObjectType);
        switch (grabbedObject.m_pickUpObjectType)
        {
            case PickUpObjectType.Small:

                break;
            case PickUpObjectType.Medium:
            case PickUpObjectType.Big:
                grabbedObject.isBeeingHeld = true;
                UpdateInteractionHUD(grabbedObject);
                connectedObject = grabbedObject;
                objectRb = ((MonoBehaviour)grabbedObject).transform.GetComponent<Rigidbody>();
                objectRb.freezeRotation = true;
                objectDefaultDrag = objectRb.drag;
                objectRb.drag = 10;
                objectRb.useGravity = false;
                ToggleHoldObjectCoroutine(true, grabbedObject.m_pickUpObjectType);
                break;
        }
    }
    public void Drop()
    {
        connectedObject.isBeeingHeld = false;
        objectRb.freezeRotation = false;
        objectRb.useGravity = true;
        objectRb.drag = objectDefaultDrag;
        objectRb = null;
        ToggleHoldObjectCoroutine(false, connectedObject.m_pickUpObjectType);
        ClearInteractButtonAction();
        connectedObject = null;
        if (objectInLOS != null)
        {
            AddToInteractButtonAction(objectInLOS);
            UpdateInteractionHUD(objectInLOS);
        }
        else UpdateInteractionHUD(null);
    }
    private void ToggleHoldObjectCoroutine(bool state, PickUpObjectType pickUpObjectType)
    {
        if (state)
        {
            if (holdObject_Ref == null)
            {
                switch (pickUpObjectType)
                {
                    case PickUpObjectType.Small:
                        break;
                    case PickUpObjectType.Medium:
                        holdObject_Ref = StartCoroutine(HoldMediumObject_Coroutine());
                        break;
                    case PickUpObjectType.Big:
                        holdObject_Ref = StartCoroutine(HoldBigObject_Coroutine());
                        break;
                }
            }
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
    public IEnumerator HoldMediumObject_Coroutine()
    {
        Transform cameraTransform = ArmadilloPlayerController.Instance.cameraControl.mainCamera.transform;
        while (true)
        {
            Vector3 holdArea = cameraTransform.position + cameraTransform.forward * 3f;
            if (Vector3.Distance(holdArea, objectRb.position) > 0.1f)
            {
                Vector3 moveDirection = holdArea - objectRb.position;
                objectRb.AddForce(moveDirection * 3f, ForceMode.VelocityChange);
            }
            yield return new WaitForFixedUpdate();
        }
    }
    public IEnumerator HoldBigObject_Coroutine()
    {
        Transform cameraTransform = ArmadilloPlayerController.Instance.cameraControl.mainCamera.transform;
        while (true)
        {
            Vector3 holdArea = cameraTransform.position + cameraTransform.forward * 3f;


            if (Vector2.Distance(new Vector2(holdArea.x, holdArea.z), new Vector2(objectRb.position.x, objectRb.position.z)) > 0.1f)
            {
                Vector3 moveDirection = holdArea - objectRb.position;
                moveDirection.y = 0;
                moveDirection.Normalize();
                moveDirection = moveDirection * 0.25f;
                objectRb.AddForce(moveDirection *3f, ForceMode.VelocityChange);
            }
            yield return new WaitForFixedUpdate();
        }
    }
}
