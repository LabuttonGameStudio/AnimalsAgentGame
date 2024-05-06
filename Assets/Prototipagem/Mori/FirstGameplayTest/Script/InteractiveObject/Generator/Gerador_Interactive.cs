using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Gerador_Interactive : MonoBehaviour, InteractiveObject
{
    [SerializeField] private Antena_Damageable[] requirements;
    [HideInInspector] public bool isEnabled;
    [HideInInspector] public bool charged;
    [SerializeField] private MeshRenderer bodyMeshRenderer;
    [SerializeField] private Transform leverTransform;
    void Awake()
    {
        bodyMeshRenderer.material = new Material(bodyMeshRenderer.material);
    }
    private void Start()
    {
        foreach (Antena_Damageable antena in requirements)
        {
            antena.DefineConnectedGenerator(this);
        }
    }
    public string GetObjectDescription()
    {
        if (isEnabled) return "";
        else return charged ? "ligar" : "";
    }

    public string GetObjectName()
    {
        return charged ? "Gerador" : "Gerador inativo";
    }

    public void OnRequirementChange()
    {
        bool charged = true;
        foreach (Antena_Damageable antena in requirements)
        {
            if (!antena.charged)
            {
                charged = false;
                break;
            }
        }
        if (charged)
        {
            this.charged = charged;
            ArmadilloPlayerController.Instance.interactControl.UpdateInteractionHUD();
        }
    }
    public void Interact(InputAction.CallbackContext value)
    {
        if (requirements != null && requirements.Length > 0)
        {
            if (charged && !isEnabled)
            {
                isEnabled = true;
                StartCoroutine(LeverSwitchAnimation_Coroutine());
            }
        }
        else
        {

        }
    }
    private IEnumerator LeverSwitchAnimation_Coroutine()
    {
        Quaternion startLeverRotation = leverTransform.localRotation;
        Quaternion finalLeverRotation = Quaternion.Euler(175, leverTransform.localRotation.eulerAngles.y, leverTransform.localRotation.eulerAngles.z);

        float timer = 0;
        float duration = 0.25f;
        while (timer < duration)
        {
            leverTransform.localRotation = Quaternion.Slerp(startLeverRotation, finalLeverRotation, timer / duration);
            timer += Time.deltaTime;
            yield return null;
        }
        bodyMeshRenderer.sharedMaterial.SetInt("_Light_on_off", 1);
        leverTransform.localRotation = finalLeverRotation;
    }
}
