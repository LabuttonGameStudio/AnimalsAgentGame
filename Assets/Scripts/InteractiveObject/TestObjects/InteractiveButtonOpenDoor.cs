using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InteractiveButtonOpenDoor : MonoBehaviour, InteractiveObject
{
    [SerializeField] private string objectName;
    [SerializeField] private string interactionDescription;
    [Space]
    [SerializeField] private Transform doorTransform;
    private bool isDoorOpen;

    public void Interact(InputAction.CallbackContext value)
    {
        ArmadilloPlayerController.Instance.hpControl.TakeDamage(new Damage(100000, Damage.DamageType.Blunt, false, transform.position));
    }

    public string GetObjectName() { return objectName; }

    public string GetObjectDescription() { return interactionDescription; }
}
