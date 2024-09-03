using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.InputSystem;

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
    public CinemachineVirtualCamera thirdPersonCinemachine;


    public void ChangeCameraState(CameraBaseState nextState)
    {
        if (currentCameraState != null) currentCameraState.ExitState();
        nextState.EnterState(this);
        currentCameraState = nextState;
    }

    private void Awake()
    {
        firstPersonSensibility = Vector2.one;
        thirdPersonSensibility = Vector2.one;
        currentSpeedModifier = 1f;
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

        ChangeSensibility(Vector2.zero);
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
        return;
        firstPersonSensibility = sensibility;
        thirdPersonSensibility = sensibility;

        InputAction inputAction = ArmadilloPlayerController.Instance.inputControl.inputAction.Armadillo.Look;
        inputAction.ApplyBindingOverride(new InputBinding
        {
            overrideProcessors = "ScaleVector2Processor(x=" + sensibility.x + ", y=" + sensibility.y + ")"
        });
    }
    public void ChangeCurrentSpeedModifier(float newValue)
    {
        currentSpeedModifier = newValue;
        ArmadilloPlayerController.Instance.inputControl.inputAction.Armadillo.Look.ApplyParameterOverride("scaleVector2:x", firstPersonSensibility.x * currentSpeedModifier);
        ArmadilloPlayerController.Instance.inputControl.inputAction.Armadillo.Look.ApplyParameterOverride("scaleVector2:y", firstPersonSensibility.y * currentSpeedModifier);
    }
}
