using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    [SerializeField]private float sensibilityX= 400,sensibilityY = 400;

    private ArmadilloPlayerInputController inputController;
    private Camera mainCamera;

    float xRotation, yRotation;

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
