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
        ArmadilloPlayerController.Instance.movementControl.transform.rotation = Quaternion.Euler(0, playerCamera.mainCamera.transform.rotation.eulerAngles.y, 0);
        ArmadilloPlayerController.Instance.movementControl.rb.rotation = Quaternion.Euler(0, playerCamera.mainCamera.transform.rotation.eulerAngles.y, 0);
    }
}
