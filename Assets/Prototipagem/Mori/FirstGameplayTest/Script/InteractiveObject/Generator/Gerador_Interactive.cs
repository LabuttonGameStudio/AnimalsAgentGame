using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class Gerador_Interactive : MonoBehaviour, IRequirements, InteractiveObject, INeedRequirements
{
    public bool isTurnedOn { get; set; }

    [SerializeField] private GameObject[] _requirements;
    public IRequirements[] requirements { get; set; }
    public INeedRequirements connectedObject { get; set; }

    [HideInInspector] public bool charged;
    [SerializeField] private UnityEvent consequences;

    [Header("Mesh")]
    [SerializeField] private MeshRenderer bodyMeshRenderer;
    [SerializeField] private Transform leverTransform;


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
    public string GetObjectDescription()
    {
        if (isTurnedOn) return "";
        else return charged ? "ligar" : "";
    }

    public string GetObjectName()
    {
        return charged ? "Gerador" : "Gerador inativo";
    }
    public void OnRequirementMet()
    {
        charged = true;
        ArmadilloPlayerController.Instance.interactControl.UpdateInteractionHUD();
    }
    public void Interact(InputAction.CallbackContext value)
    {
        if (requirements != null && requirements.Length > 0)
        {
            if (charged && !isTurnedOn)
            {
                isTurnedOn = true;
                StartCoroutine(LeverSwitchAnimation_Coroutine());
            }
        }
        else
        {
            if (!isTurnedOn)
            {
                isTurnedOn = true;
                StartCoroutine(LeverSwitchAnimation_Coroutine());
            }
        }
    }
    private IEnumerator LeverSwitchAnimation_Coroutine()
    {
        Quaternion startLeverRotation = leverTransform.localRotation;
        Quaternion finalLeverRotation = Quaternion.Euler(175, leverTransform.localRotation.eulerAngles.y, leverTransform.localRotation.eulerAngles.z);

        float timer = 0;
        float duration = 0.35f;
        while (timer < duration)
        {
            leverTransform.localRotation = Quaternion.Slerp(startLeverRotation, finalLeverRotation, timer / duration);
            timer += Time.deltaTime;
            yield return null;
        }
        bodyMeshRenderer.sharedMaterial.SetInt("_Light_on_off", 1);
        leverTransform.localRotation = finalLeverRotation;
        consequences.Invoke();
        if (connectedObject != null)
        {
            connectedObject.OnRequirementChange();
        }
    }
}
