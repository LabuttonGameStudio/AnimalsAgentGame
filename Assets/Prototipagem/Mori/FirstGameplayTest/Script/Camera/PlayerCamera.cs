using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class PlayerCamera : MonoBehaviour
{
    //Singleton
    static public PlayerCamera Instance;

    public float firstPersonSensibilityX= 10, firstPersonSensibilityY = 10;

    private ArmadilloPlayerInputController inputController;
    [System.NonSerialized]public Camera mainCamera;
    public Transform cameraFollowPoint;

    [HideInInspector]public float xRotation, yRotation;

    //State Machine
    private CameraBaseState currentCameraState;

    public CameraFirstPersonState firstPersonCameraState;
    public CameraThirdPersonState thirdPersonCameraState;

    public CinemachineFreeLook cinemachineFreeLook;
    public void ChangeCameraState(CameraBaseState nextState)
    {
        if(currentCameraState != null)currentCameraState.ExitState();
        nextState.EnterState(this);
        currentCameraState = nextState;
    }

    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        mainCamera = Camera.main;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        inputController = GetComponent<ArmadilloPlayerInputController>();
        inputController.inputAction.Armadillo.Look.Enable();

        //Instancia os estados da camera
        firstPersonCameraState= new CameraFirstPersonState();
        thirdPersonCameraState= new CameraThirdPersonState();

        //Define o estado padrao da camera para primeira pessoa
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
}
