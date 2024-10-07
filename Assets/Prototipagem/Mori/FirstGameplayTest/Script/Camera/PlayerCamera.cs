using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.InputSystem;
public class SensibilityModifier
{
    public float sensibilityModifier;
    public SensibilityModifier(float sensibilityModifier)
    {
        this.sensibilityModifier = sensibilityModifier;
    }
}
public class PlayerCamera : MonoBehaviour
{
    //Singleton
    static public PlayerCamera Instance;
    [Header("Sensibility")]
    [Header("First Person")]
    public Vector2 firstPersonSensibility;
    [Header("Third Person")]
    public Vector2 thirdPersonSensibility;

    [Header("Sensibility Modifiers")]
    private float currentSpeedModifier = 1f;

    private ArmadilloPlayerInputController inputController;
    public Camera mainCamera;
    public Camera weaponCamera;
    public Transform cameraFollowPoint;
    [Space]
    [Header("Culling Mask")]
    [SerializeField] private LayerMask firstPersonMask;
    [SerializeField] private LayerMask thirdPersonMask;
    [Space]
    [SerializeField] public LayerMask firstPeronSeeThroughMask;
    [SerializeField] public LayerMask thirdPersonSeeThroughMask;
    [Space]
    [HideInInspector] public float xRotation, yRotation;

    //State Machine
    private CameraBaseState currentCameraState;

    public CameraFirstPersonState firstPersonCameraState;
    public CameraThirdPersonState thirdPersonCameraState;

    [Space, Header("Cinemachine")]
    public CinemachineVirtualCamera firstPersonCinemachine;
    private CinemachinePOV firstPersonCinemachinePOV;
    public CinemachineVirtualCamera thirdPersonCinemachine;

    //----- Zoom -----
    [HideInInspector]public float currentZoom;
    [HideInInspector] public float sprintZoomModifier=1f; //1.1f/1f

    [HideInInspector] public float sniperZoomModifier=1f; //0.33f/1f

    //Sensibility Modifiers
    public List<SensibilityModifier> sensibilityModifiers = new List<SensibilityModifier>();
    public void ChangeCameraState(CameraBaseState nextState)
    {
        if (currentCameraState != null) currentCameraState.ExitState();
        nextState.EnterState(this);
        currentCameraState = nextState;
    }

    private void Awake()
    {
        sensibilityModifiers = new List<SensibilityModifier> ();
        firstPersonSensibility = Vector2.one;
        thirdPersonSensibility = Vector2.one;
        currentSpeedModifier = 1f;
        sprintZoomModifier = 1f;
        sniperZoomModifier = 1f;
        currentZoom = firstPersonCinemachine.m_Lens.FieldOfView;
        Instance = this;
    }
    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        inputController = GetComponent<ArmadilloPlayerInputController>();
        inputController.inputAction.Armadillo.Look.Enable();

        //Instancia os estados da camera
        firstPersonCameraState = new CameraFirstPersonState();
        thirdPersonCameraState = new CameraThirdPersonState();

        //Define o estado padrao da camera para primeira pessoa
        mainCamera.cullingMask = firstPersonMask;
        ChangeCameraState(firstPersonCameraState);
    }
    public Vector2 GetMouseDelta()
    {
        return inputController.inputAction.Armadillo.Look.ReadValue<Vector2>();
    }

    public void Update()
    {
        currentCameraState.UpdateState();
    }
    public void ToggleFPCamera(bool state)
    {
        weaponCamera.enabled = state;
    }
    public void ToggleCullingMask(bool state)
    {
        mainCamera.cullingMask = state ? firstPersonMask : thirdPersonMask;
    }
    public void ChangeSensibility(Vector2 sensibility)
    {
        //Update variables
        firstPersonSensibility = new Vector2(sensibility.x / 40f, sensibility.y / 40f);
        thirdPersonSensibility = sensibility/5f;

        //Apply to first person, dont need for third person because is real time
        UpdateCurrentSensibilitySpeed();
    }
    public void ChangeCurrentSpeedModifier(float newValue)
    {
        currentSpeedModifier = newValue;

        UpdateCurrentSensibilitySpeed();
    }
    public void UpdateCurrentSensibilitySpeed()
    {
        float currentModifier =1;
        if (firstPersonCinemachinePOV == null) firstPersonCinemachinePOV = firstPersonCinemachine.GetCinemachineComponent<CinemachinePOV>();
        foreach (SensibilityModifier sensibilityModifier in sensibilityModifiers)
        {
            currentModifier *= sensibilityModifier.sensibilityModifier;
        }
        firstPersonCinemachinePOV.m_HorizontalAxis.m_MaxSpeed = firstPersonSensibility.x * currentSpeedModifier* currentModifier;
        firstPersonCinemachinePOV.m_VerticalAxis.m_MaxSpeed = firstPersonSensibility.y * currentSpeedModifier* currentModifier;
    }
    public void AddSensibilityModifier(SensibilityModifier sensibilityModifier)
    {
        sensibilityModifiers.Add(sensibilityModifier);
        UpdateCurrentSensibilitySpeed();
    }
    public void RemoveSensibilityModifier(SensibilityModifier sensibilityModifier)
    {
        if(sensibilityModifiers.Contains(sensibilityModifier))
        {
            sensibilityModifiers.Remove(sensibilityModifier);
            UpdateCurrentSensibilitySpeed();
        }
    }
    public void ToggleZoom(bool state)
    {
        sniperZoomModifier = state ? 0.33f : 1f;
        UpdateZoom();
        ChangeCurrentSpeedModifier(state ? 0.33f: 1);
    }
    public void UpdateZoom()
    {
        firstPersonCinemachine.m_Lens.FieldOfView = currentZoom * sniperZoomModifier * sprintZoomModifier;
    }

    public void StartLerpSprintZoom(float finalValue,float duration)
    {
        if(lerpSprintZoom_Ref != null)StopCoroutine(lerpSprintZoom_Ref);
        lerpSprintZoom_Ref = StartCoroutine(LerpSprintZoom_Coroutine(sprintZoomModifier, finalValue,duration));
    }
    private Coroutine lerpSprintZoom_Ref;
    public IEnumerator LerpSprintZoom_Coroutine(float startValue,float finalValue,float duration)
    {
        float timer = 0f;
        while(timer<duration)
        {
            sprintZoomModifier = Mathf.Lerp(startValue,finalValue,timer/duration);
            UpdateZoom();
            timer += Time.deltaTime;
            yield return null;
        }
        sprintZoomModifier = finalValue;
        lerpSprintZoom_Ref = null;
    }
}
