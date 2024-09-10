using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ContainerDoor : MonoBehaviour, InteractiveObject
{
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
        float lerpAngle = startValue;
        while (timer < duration)
        {
            lerpAngle = Mathf.Lerp(lerpAngle, finalValue, timer/duration);
            transform.localRotation = Quaternion.Euler(transform.localRotation.eulerAngles.x, lerpAngle, transform.localRotation.eulerAngles.z);
            timer += Time.deltaTime;
            yield return null;
        }
        lerpAngle_Ref = null;
    }
}
