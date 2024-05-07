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

    }
    public string GetObjectDescription()
    {
        if (isOpen) return "";
        else return charged ? "Abrir" : "";
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
        if (requirements != null && requirements.Length > 0)
        {
            if (charged)
            {
                //Colocar pra abrir e fechar portao
            }
        }
        else
        {
            //Colocar pra abrir e fechar portao
        }
    }
    private IEnumerator ToggleGate()
    {
        Vector3 startGatePos = gateTransform.position;
        Vector3 finalGatePos = gateTransform.position;

        float timer = 0;
        float duration = 0.25f;
        while (timer < duration)
        {
            gateTransform.position = Vector3.Slerp(startGatePos, finalGatePos, timer / duration);
            timer += Time.deltaTime;
            yield return null;
        }
        bodyMeshRenderer.sharedMaterial.SetInt("_Light_on_off", 1);
        gateTransform.position = finalGatePos;
    }
}

