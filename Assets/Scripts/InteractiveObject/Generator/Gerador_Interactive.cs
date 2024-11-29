using System.Collections;
using Pixeye.Unity;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.Localization;

public class Gerador_Interactive : MonoBehaviour, IRequirements, InteractiveObject, INeedRequirements
{
    public bool isEnabled { get; set; }
    [Foldout("Translations",true)]
    [SerializeField] private LocalizedString objectName;
    [SerializeField] private LocalizedString objectDescriptionTurnedOff;
    [SerializeField] private LocalizedString objectDescriptionTurnedOn;
    public bool isTurnedOn { get; set; }

    //----- Requirements -----
    private bool needRequeriments;
    [Foldout("Functionality", true)]
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
        isEnabled = true;
        bodyMeshRenderer.material = new Material(bodyMeshRenderer.material);
    }
    private void OnValidate()
    {
        for (int i = 0; i < _requirements.Length; i++)
        {
            GameObject go = _requirements[i];
            if (go != null && !go.TryGetComponent(out IRequirements requirement))
            {
                Debug.LogError("Erro ao conectar ao " + go.name + ", cheque se ele possui os scripts: Antena ou Gerador");
                _requirements[i] = null;
            }
        }
    }
    private void Start()
    {
        //Garantee there isnt a empty array slot
        int totalRequirements = 0;
        for (int i = 0; i < _requirements.Length; i++)
        {
            GameObject go = _requirements[i];
            if (go == null) continue;
            if (go.TryGetComponent(out IRequirements requirement))
            {
                totalRequirements += 1;
            }
        }
        requirements = new IRequirements[totalRequirements];
        int currentRequerimentsIndexPos = 0;
        //Transfer from _requeriments to requirements
        for (int i = 0; i < _requirements.Length; i++)
        {
            GameObject go = _requirements[i];
            if (go == null) continue;
            if (go.TryGetComponent(out IRequirements requirement))
            {
                requirements[currentRequerimentsIndexPos] = requirement;
                currentRequerimentsIndexPos++;
            }
            else
            {
                Debug.LogError("Erro ao conectar ao " + go.name + ", cheque se ele possui os scripts: Antena ou Gerador");
            }
        }
        if (totalRequirements > 0) needRequeriments = true;
        foreach (IRequirements requirement in requirements)
        {
            requirement.DefineConnectedObject(this);
        }
    }
    public Vector2 GetCurrentRequirementsMet()
    {
        // X = Required // Y = Total
        int requirementsMet = 0;
        for (int i = 0; i < requirements.Length; i++)
        {
            if (requirements[i].isTurnedOn) requirementsMet += 1;
        }
        return new Vector2(requirementsMet, requirements.Length);
    }
    public string GetObjectDescription()
    {
        if (isTurnedOn) return objectDescriptionTurnedOn.GetLocalizedString();
        else
        {
            if (needRequeriments)
            {
                Vector2 requirementsString = GetCurrentRequirementsMet();
                return charged ? "Ligar" : "(" + requirementsString.x + "|" + requirementsString.y + ")";
            }
            else  return objectDescriptionTurnedOff.GetLocalizedString();
        }
    }
    public string GetObjectName()
    {
        return objectName.GetLocalizedString();
        if (isTurnedOn) return "Gerador Ligado";
        if (needRequeriments)
        {
            return charged ? "Gerador" : "Gerador inativo";
        }
        else
        {
            return "Gerador";
        }
    }
    public void OnRequirementMet()
    {
        charged = true;
        ArmadilloPlayerController.Instance.interactControl.UpdateInteractionHUD();
    }
    public void Interact(InputAction.CallbackContext value)
    {
        TurnOn();
        ArmadilloPlayerController.Instance.interactControl.UpdateInteractionHUD();
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

    public void TurnOn()
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
}
