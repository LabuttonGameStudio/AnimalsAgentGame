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
        playerCamera.thirdPersonCinemachine.Priority = 10;
        playerCamera.firstPersonCinemachine.Priority = 11;
    }

    public override void ExitState()
    {
       
    }

    public override void UpdateState()
    {

        //playerCamera.firstPersonCamera.transform.position = playerCamera.cameraFollowPoint.position;

        //Vector2 mouseDelta = playerCamera.GetMouseDelta();
        //playerCamera.xRotation -= mouseDelta.y * playerCamera.firstPersonSensibilityY / 100;
        //playerCamera.yRotation += mouseDelta.x * playerCamera.firstPersonSensibilityX / 100;

        //playerCamera.xRotation = Mathf.Clamp(playerCamera.xRotation, -90, 90);
        //playerCamera.firstPersonCamera.transform.rotation = Quaternion.Euler(playerCamera.xRotation, playerCamera.yRotation, 0);
        ArmadilloPlayerController.Instance.movementControl.transform.rotation = Quaternion.Euler(0, playerCamera.mainCamera.transform.rotation.eulerAngles.y, 0);
        ArmadilloPlayerController.Instance.movementControl.rb.rotation = Quaternion.Euler(0, playerCamera.mainCamera.transform.rotation.eulerAngles.y, 0);
    }
}
