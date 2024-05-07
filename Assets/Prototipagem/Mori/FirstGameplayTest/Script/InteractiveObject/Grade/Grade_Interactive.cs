using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Grade_Interactive : MonoBehaviour, InteractiveObject
{
    [SerializeField] private Gerador_Interactive[] requirements;
    [HideInInspector] public bool isOpen;
    [HideInInspector] public bool charged;
    [SerializeField] private MeshRenderer bodyMeshRenderer;
    [SerializeField] private Transform gateTransform;
    void Awake()
    {
        bodyMeshRenderer.material = new Material(bodyMeshRenderer.material);
    }
    private void Start()
    {
        foreach (Gerador_Interactive gerador in requirements)
        {
            gerador.DefineConnectedGate(this);
        }
        if (requirements == null || requirements.Length <= 0) charged = true;
    }
    public string GetObjectDescription()
    {
        if (charged) return isOpen ? "Fechar" : "Abrir";
        else return "Desligado";
    }

    public string GetObjectName()
    {
        return charged ? "Portao" : "Portao inacessível";
    }

    public void OnRequirementChange()
    {
        bool charged = true;
        foreach (Gerador_Interactive gerador in requirements)
        {
            if (!gerador.isEnabled)
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

        if (charged)
        {
            if (toggleGate_Ref != null)
            {
                StopCoroutine(toggleGate_Ref);
            }
            isOpen = !isOpen;
            toggleGate_Ref = StartCoroutine(ToggleGate_Coroutine(!isOpen));
        }
    }
    private Coroutine toggleGate_Ref;
    private IEnumerator ToggleGate_Coroutine(bool state)
    {
        Vector3 startGatePos = gateTransform.localPosition;
        Vector3 finalGatePos = state ? new Vector3(2.5f, 0.12f, 0.25f) : new Vector3(5.9f, 0.12f, 0.25f);
        ArmadilloPlayerController.Instance.interactControl.UpdateInteractionHUD();  
        bodyMeshRenderer.sharedMaterial.SetInt("_Light_on_off", state ? 1 : 0);
        float timer = 0;
        float duration = 1f;
        while (timer < duration)
        {
            gateTransform.localPosition = Vector3.Lerp(startGatePos, finalGatePos, timer / duration);
            timer += Time.deltaTime;
            yield return null;
        }
        gateTransform.localPosition = finalGatePos;
        toggleGate_Ref = null;
    }
}

