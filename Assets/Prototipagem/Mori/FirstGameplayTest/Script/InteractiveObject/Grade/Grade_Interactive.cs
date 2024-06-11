using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Grade_Interactive : MonoBehaviour,INeedRequirements
{

    [SerializeField] private GameObject[] _requirements;
    public IRequirements[] requirements { get; set;}
    [HideInInspector] public bool isOpen=false;
    [SerializeField] private MeshRenderer bodyMeshRenderer;
    [SerializeField] private Transform gateTransform;
    void Awake()
    {
        bodyMeshRenderer.material = new Material(bodyMeshRenderer.material);
    }
    private void Start()
    {
        requirements = new IRequirements[_requirements.Length];
        for (int i = 0; i < _requirements.Length; i++)
        {
            GameObject go = _requirements[i];
            if (go.TryGetComponent(out IRequirements requirement))
            {
                requirements[i] = requirement;
            }
            else
            {
                Debug.LogError("Erro ao conectar ao " + go.name + ", cheque se ele possui os scripts: Antena ou Gerador");
            }
        }
        foreach (IRequirements requirement in requirements)
        {
            requirement.DefineConnectedObject(this);
        }
    }
    private Coroutine toggleGate_Ref;


    private IEnumerator ToggleGate_Coroutine(bool state)
    {
        Vector3 startGatePos = gateTransform.localPosition;
        Vector3 finalGatePos = state ? new Vector3(5.9f, 0.12f, 0.25f) : new Vector3(2.5f, 0.12f, 0.25f);
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

    public void OnRequirementMet()
    {
        if (isOpen) return;
        isOpen = true;
        toggleGate_Ref = StartCoroutine(ToggleGate_Coroutine(true));
    }
}

