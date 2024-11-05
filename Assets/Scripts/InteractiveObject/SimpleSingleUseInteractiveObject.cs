using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.Localization;

public class SimpleSingleUseInteractiveObject : MonoBehaviour, InteractiveObject
{
    [Foldout("Name|Description",styled =true)]
    [SerializeField] private LocalizedString objectName;
    [SerializeField] private LocalizedString objectDescription;

    [Foldout("Use", styled = true)]
    [SerializeField]private UnityEvent onUseEvent;
    private bool wasUsed;

    public string GetObjectName()
    {
        return objectName.GetLocalizedString();
    }
    public string GetObjectDescription()
    {
        return objectDescription.GetLocalizedString();
    }

    public void Interact(InputAction.CallbackContext value)
    {
        if (wasUsed) return;
        wasUsed = true;
        onUseEvent.Invoke();
    }
}
