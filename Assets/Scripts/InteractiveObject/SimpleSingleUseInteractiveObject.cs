using Pixeye.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.Localization;

public class SimpleSingleUseInteractiveObject : MonoBehaviour, InteractiveObject
{
    [Foldout("Name|Description",true)]
    [SerializeField] private LocalizedString objectName;
    [SerializeField] private LocalizedString objectDescription;

    [Foldout("Use", true)]
    [SerializeField]private UnityEvent onUseEvent;
    private bool wasUsed;

    public bool isEnabled { get; set; }

    public string GetObjectName()
    {
        return objectName.GetLocalizedString();
    }
    public string GetObjectDescription()
    {
        return objectDescription.GetLocalizedString();
    }

    private void Awake()
    {
        isEnabled = true;
    }
    public void Interact(InputAction.CallbackContext value)
    {
        if (wasUsed) return;
        wasUsed = true;
        onUseEvent.Invoke();
        isEnabled = false;
        ArmadilloInteractController.Instance.ClearInteractButtonAction();
        gameObject.layer = LayerMask.NameToLayer("Default");
    }
}
