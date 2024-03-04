using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraThirdPersonState : CameraBaseState
{
    private PlayerCamera playerCamera;
    public override void EnterState(PlayerCamera playerCmr)
    {
        playerCamera = playerCmr;
        //Tween.MoveTransformLocalPosition(playerCamera, playerCamera.cameraFollowPoint, new Vector3(0, 1, -5f), 0.2f);
        playerCamera.cinemachineFreeLook.enabled = true;
    }

    public override void ExitState()
    {

    }

    public override void UpdateState()
    {

    }
}
