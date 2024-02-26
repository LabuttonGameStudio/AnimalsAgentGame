using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class ArmadilloInteractController : MonoBehaviour
{
    //Singleton
    public static ArmadilloInteractController Instance { get; private set; }

    //Input System
    ArmadilloPlayerInputController inputController;

    //Interaction Layer
    [SerializeField] private LayerMask whatIsInteractive;

    //HUD
    [Space]
    [SerializeField] private CanvasGroup interactHUD;
    [SerializeField] private TextMeshProUGUI interactItemName;
    [SerializeField] private TextMeshProUGUI interactKeybind;
    [SerializeField] private TextMeshProUGUI interactDescription;
    private void Awake()
    {
        Instance = this;
    }


    private void Start()
    {
        inputController = GetComponent<ArmadilloPlayerInputController>();

        inputController.inputAction.Armadillo.Interact.Enable();
    }

    InteractiveObject activeInteractiveObject;
    public void AddToInteractButtonAction(InteractiveObject interactiveObject)
    {
        activeInteractiveObject = interactiveObject;
        inputController.inputAction.Armadillo.Interact.performed += interactiveObject.Interact;

        UpdateInteractionHUD();
        interactHUD.alpha = 1;

    }

    public void UpdateInteractionHUD()
    {
        interactItemName.text = activeInteractiveObject.GetObjectName();
        interactDescription.text = activeInteractiveObject.GetObjectDescription();
    }

    public void ClearInteractButtonAction()
    {
        if (activeInteractiveObject != null)
        {
            inputController.inputAction.Armadillo.Interact.performed -= activeInteractiveObject.Interact;
            activeInteractiveObject = null;
        }

        interactItemName.text = "";
        interactDescription.text = "";
        interactHUD.alpha = 0;
    }

    public int interactiveObjectsInRange;
    public void OnInteractiveObjectEnterRange()
    {
        interactiveObjectsInRange++;
        if (interactiveObjectsInRange.Equals(1))
        {
            ToggleCheckingForInteractiveItems(true);
        }
    }
    public void OnInteractiveObjectLeaveRange()
    {
        interactiveObjectsInRange--;
        if (interactiveObjectsInRange <= 0)
        {
            ToggleCheckingForInteractiveItems(false);
        }
    }

    public void ToggleCheckingForInteractiveItems(bool state)
    {
        if (checkForInteractiveItemInAimRef == null && state)
        {
            checkForInteractiveItemInAimRef = StartCoroutine(CheckForInteractiveItemInAim_Coroutine());
        }
        else if (checkForInteractiveItemInAimRef != null && !state)
        {
            StopCoroutine(checkForInteractiveItemInAimRef);
            checkForInteractiveItemInAimRef = null;
        }
    }

    private Coroutine checkForInteractiveItemInAimRef;
    public IEnumerator CheckForInteractiveItemInAim_Coroutine()
    {
        Transform activeInteractive = null;
        Transform cameraTransform = PlayerCamera.Instance.mainCamera.transform;
        while (true)
        {
            if (Physics.Raycast(cameraTransform.position, cameraTransform.forward, out RaycastHit hitInfo, 10f, whatIsInteractive, QueryTriggerInteraction.Ignore))
            {
                Debug.Log(hitInfo.transform.gameObject.name);
                if (activeInteractive != hitInfo.transform)
                {
                    activeInteractive = hitInfo.transform;
                    if (hitInfo.transform != null && hitInfo.transform.TryGetComponent(out InteractiveObject interactiveObject))
                    {
                        AddToInteractButtonAction(interactiveObject);
                    }
                }
            }
            else
            {
                activeInteractive = null;
                ClearInteractButtonAction();
            }
            yield return new WaitForSeconds(0.05f);
        }
    }

}
