using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class CameraFirstPersonState : CameraBaseState
{
    private PlayerCamera playerCamera;
    public override void EnterState(PlayerCamera playerCmr)
    {
        playerCamera = playerCmr;
        //Tween.MoveTransformLocalPosition(playerCamera, playerCamera.cameraFollowPoint, new Vector3(0, 0.5f, 0f), 0.2f);
        playerCamera.cinemachineFreeLook.enabled = false;
    }

    public override void ExitState()
    {
       
    }

    public override void UpdateState()
    {
        playerCamera.mainCamera.transform.position = playerCamera.cameraFollowPoint.position;

        Vector2 mouseDelta = playerCamera.GetMouseDelta();
        playerCamera.xRotation -= mouseDelta.y * playerCamera.firstPersonSensibilityY / 100;
        playerCamera.yRotation += mouseDelta.x * playerCamera.firstPersonSensibilityX / 100;

        playerCamera.xRotation = Mathf.Clamp(playerCamera.xRotation, -90, 90);
        playerCamera.mainCamera.transform.rotation = Quaternion.Euler(playerCamera.xRotation, playerCamera.yRotation, 0);
        playerCamera.transform.rotation = Quaternion.Euler(0, playerCamera.yRotation, 0);
    }
}
