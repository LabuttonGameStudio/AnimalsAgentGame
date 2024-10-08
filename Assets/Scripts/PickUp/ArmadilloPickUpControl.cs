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

    [Header("Throw")]
    [SerializeField] private float smallObjectThrowForce;
    [SerializeField] private float mediumObjectThrowForce;
    [SerializeField] private float bigObjectThrowForce;

    [Header("Connected Object Default Configs")]
    private PhysicMaterial objectDefaultPhysicMaterial;
    private Collider objectDefaultCollider;
    private float objectDefaultDrag;
    private RigidbodyConstraints objectDefaultConstraints;
    private bool objectDefaultFreezeRotation;
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
        ArmadilloPlayerController playerController = ArmadilloPlayerController.Instance;
        playerController.weaponControl.ToggleArms(false);
        playerController.weaponControl.ToggleWeapon(false, false);

        //Ebale throw objects
        playerController.inputControl.inputAction.Armadillo.Fire.Enable();
        playerController.inputControl.inputAction.Armadillo.Fire.performed += ThrowObject;

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
                if (objectRb.transform.TryGetComponent(out Collider collider))
                {
                    objectDefaultCollider = collider;
                    if (collider.material != null)
                    {
                        objectDefaultPhysicMaterial = collider.material;
                    }
                    collider.material = holdObjectPhysicMaterial;
                }

                objectDefaultDrag = objectRb.drag;
                objectDefaultConstraints = objectRb.constraints;
                objectDefaultInterpolation = objectRb.interpolation;
                objectDefaultCollisionDetectionMode = objectRb.collisionDetectionMode;
                objectDefaultFreezeRotation = objectRb.freezeRotation;

                objectRb.freezeRotation = true;
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
        if (connectedObject == null) return;
        ArmadilloPlayerController playerController = ArmadilloPlayerController.Instance;

        playerController.weaponControl.ToggleArms(true);
        playerController.weaponControl.ToggleWeapon(true, false);

        playerController.inputControl.inputAction.Armadillo.Fire.performed -= ThrowObject;

        switch (connectedObject.m_pickUpObjectType)
        {
            case PickUpObjectType.Small:

                break;
            case PickUpObjectType.Medium:
            case PickUpObjectType.Big:
                ArmadilloPlayerController.Instance.movementControl.speedMultiplier = 1f;

                connectedObject.isBeeingHeld = false;
                objectRb.useGravity = true;

                if (objectDefaultPhysicMaterial != null)
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

                objectRb.freezeRotation = objectDefaultFreezeRotation;

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
            Vector3 moveForce = moveDirection * mediumObjectPickUpForce + rigidbodyOfPlayer.velocity * 600;
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

    #region Throw
    public void ThrowObject(InputAction.CallbackContext value)
    {
        Rigidbody connectedRigidbody = objectRb;
        Camera camera = ArmadilloPlayerController.Instance.cameraControl.mainCamera;

        Vector3 throwForce=Vector3.zero;
        switch (connectedObject.m_pickUpObjectType)
        {
            case PickUpObjectType.Small:
                Drop();
                throwForce = camera.transform.forward * smallObjectThrowForce;
                break;
            case PickUpObjectType.Medium:
                Drop();
                throwForce = camera.transform.forward * mediumObjectThrowForce;
                break;
            case PickUpObjectType.Big:
                Drop();
                throwForce = camera.transform.forward * bigObjectThrowForce;
                throwForce.y = 0;
                break;
        }
        connectedRigidbody.AddForce(throwForce, ForceMode.Impulse);
    }
    #endregion
}
