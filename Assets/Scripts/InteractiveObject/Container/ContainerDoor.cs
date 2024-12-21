using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ContainerDoor : MonoBehaviour, InteractiveObject
{
    public bool isEnabled { get; set; }
    [SerializeField] private ContainerDoor twinDoor;
    private bool isOpen;
    [SerializeField] private float openAngle;
    [SerializeField] private float closedAngle;
    public string GetObjectDescription()
    {
        return isOpen ? "Fechar" : "Abrir";
    }

    public string GetObjectName()
    {
        return "Porta Container";
    }

    private void Awake()
    {
        isEnabled = true;
    }

    public void Interact(InputAction.CallbackContext value)
    {
        if(!isOpen)
        {
            twinDoor.Open();
            Open();
        }
        else
        {
            twinDoor.Close();
            Close();
        }
        ArmadilloPlayerController.Instance.interactControl.UpdateInteractionHUD();
    }
    public void Open()
    {
        if(lerpAngle_Ref !=null) StopCoroutine(lerpAngle_Ref);
        lerpAngle_Ref = StartCoroutine(LerpAngle_Coroutine(transform.localRotation.eulerAngles.y, openAngle));
        isOpen = true;
    }
    public void Close()
    {
        if (lerpAngle_Ref != null) StopCoroutine(lerpAngle_Ref);
        lerpAngle_Ref = StartCoroutine(LerpAngle_Coroutine(transform.localRotation.eulerAngles.y, closedAngle));
        isOpen = false;
    }
    private Coroutine lerpAngle_Ref;
    private IEnumerator LerpAngle_Coroutine(float startValue,float finalValue)
    {
        float timer = 0;
        float duration = 0.25f;
        Quaternion startQuaternion = Quaternion.Euler(transform.localRotation.eulerAngles.x, startValue, transform.localRotation.eulerAngles.z);
        Quaternion finalQuaternion = Quaternion.Euler(transform.localRotation.eulerAngles.x, finalValue, transform.localRotation.eulerAngles.z);
        while (timer < duration)
        {
            transform.localRotation = Quaternion.Lerp(startQuaternion,finalQuaternion,timer/duration);
            timer += Time.deltaTime;
            yield return null;
        }
        transform.localRotation = finalQuaternion;
        lerpAngle_Ref = null;
    }
}
