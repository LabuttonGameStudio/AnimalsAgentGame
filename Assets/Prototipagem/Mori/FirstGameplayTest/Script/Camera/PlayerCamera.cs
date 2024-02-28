using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    //Singleton
    static public PlayerCamera Instance;

    [SerializeField]private float sensibilityX= 400,sensibilityY = 400;

    private ArmadilloPlayerInputController inputController;
    [System.NonSerialized]public Camera mainCamera;
    public Transform cameraFollowPoint;

    float xRotation, yRotation;
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
    }

    public Vector2 GetMouseDelta()
    {
        return inputController.inputAction.Armadillo.Look.ReadValue<Vector2>();
    }

    public void Update()
    {
        Vector2 mouseDelta = GetMouseDelta();
        xRotation -= mouseDelta.y * Time.fixedDeltaTime * sensibilityY;
        yRotation += mouseDelta.x * Time.fixedDeltaTime * sensibilityX;

        xRotation = Mathf.Clamp(xRotation, -90, 90);
        mainCamera.transform.rotation = Quaternion.Euler(xRotation, yRotation, 0);
        transform.rotation = Quaternion.Euler(0,yRotation, 0);
    }
}
