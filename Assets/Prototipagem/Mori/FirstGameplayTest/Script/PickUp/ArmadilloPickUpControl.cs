using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static IPickUpObject;

public class ArmadilloPickUpControl : MonoBehaviour
{
    [Header("ConnectedObject")]
    [SerializeField] private PhysicMaterial holdObjectPhysicMaterial;
    private Rigidbody objectRb;
    private IPickUpObject objectInLOS;
    [HideInInspector] public IPickUpObject connectedObject;

    [Header("Input")]
    private Action<InputAction.CallbackContext> actionInput;

    [Header("Player Force")]
    [SerializeField] private float mediumObjectPickUpForce;

    [Header("Connected Object Default Configs")]
    private PhysicMaterial objectDefaultPhysicMaterial;
    private Collider objectDefaultCollider;
    private float objectDefaultDrag;
    private RigidbodyConstraints objectDefaultConstraints;
    private RigidbodyInterpolation objectDefaultInterpolation;
    private CollisionDetectionMode objectDefaultCollisionDetectionMode;

    public bool CanJumpOnConnectedObject()
    {
        if (connectedObject != null)
        {
            switch (connectedObject.m_pickUpObjectType)
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
                if(objectRb.transform.TryGetComponent(out Collider collider))
                {
                    objectDefaultCollider = collider;
                    if(collider.material != null)
                    {
                        objectDefaultPhysicMaterial = collider.material;
                    }
                    collider.material = holdObjectPhysicMaterial;
                }

                objectRb.freezeRotation = true;
                objectDefaultDrag = objectRb.drag;
                objectDefaultConstraints = objectRb.constraints;
                objectDefaultInterpolation = objectRb.interpolation;
                objectDefaultCollisionDetectionMode = objectRb.collisionDetectionMode;

                objectRb.drag = 10;
                objectRb.useGravity = false;
                objectRb.interpolation = RigidbodyInterpolation.Extrapolate;
                objectRb.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
                ToggleHoldObjectCoroutine(true, grabbedObject.m_pickUpObjectType);
                break;
        }
    }
    public void Drop()
    {
        if(connectedObject == null) return;
        switch (connectedObject.m_pickUpObjectType)
        {
            case PickUpObjectType.Small:

                break;
            case PickUpObjectType.Medium:
            case PickUpObjectType.Big:
                ArmadilloPlayerController.Instance.movementControl.speedMultiplier = 1f;

                connectedObject.isBeeingHeld = false;
                objectRb.useGravity = true;

                if(objectDefaultPhysicMaterial != null)
                {
                    objectDefaultCollider.material = objectDefaultPhysicMaterial;
                    objectDefaultPhysicMaterial = null;
                }

                objectRb.drag = objectDefaultDrag;
                objectDefaultDrag = 0;

                objectRb.constraints = objectDefaultConstraints;
                objectDefaultConstraints = 0;
                
                objectRb.interpolation = objectDefaultInterpolation;
                objectDefaultInterpolation = 0;

                objectRb.collisionDetectionMode = objectDefaultCollisionDetectionMode;
                objectDefaultCollisionDetectionMode = 0;

                objectRb.freezeRotation = false;

                objectRb = null;
                ToggleHoldObjectCoroutine(false, connectedObject.m_pickUpObjectType);
                connectedObject = null;
                ClearInteractButtonAction();
                if (objectInLOS != null)
                {
                    AddToInteractButtonAction(objectInLOS);
                    UpdateInteractionHUD(objectInLOS);
                }
                else UpdateInteractionHUD(null);
                break;
        }
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
        Rigidbody rigidbodyOfPlayer = ArmadilloPlayerController.Instance.movementControl.rb;

        ArmadilloPlayerController.Instance.movementControl.speedMultiplier = 0.75f;
        while (true)
        {
            Vector3 holdArea = cameraTransform.position + cameraTransform.forward * 3f;

            Vector3 moveDirection = holdArea - objectRb.position;
            Vector3 moveForce = moveDirection * mediumObjectPickUpForce + rigidbodyOfPlayer.velocity*600;
            //HorizontalForce

            objectRb.AddForce(moveForce * Time.deltaTime, ForceMode.Acceleration);
            if (Vector3.Distance(rigidbodyOfPlayer.position, objectRb.position) > 7)
            {
                Drop();
            }
            yield return null;
        }
    }
    public IEnumerator HoldBigObject_Coroutine()
    {
        Transform cameraTransform = ArmadilloPlayerController.Instance.cameraControl.mainCamera.transform;

        ArmadilloPlayerController.Instance.movementControl.speedMultiplier = 0.5f;
        while (true)
        {
            Vector3 holdArea = cameraTransform.position + cameraTransform.forward * 3f;


            if (Vector2.Distance(new Vector2(holdArea.x, holdArea.z), new Vector2(objectRb.position.x, objectRb.position.z)) > 0.1f)
            {
                Vector3 moveDirection = holdArea - objectRb.position;
                moveDirection.y = 0;
                moveDirection.Normalize();
                moveDirection = moveDirection * 0.25f;
                objectRb.AddForce(moveDirection * 3f, ForceMode.VelocityChange);
            }
            yield return new WaitForFixedUpdate();
        }
    }
}
