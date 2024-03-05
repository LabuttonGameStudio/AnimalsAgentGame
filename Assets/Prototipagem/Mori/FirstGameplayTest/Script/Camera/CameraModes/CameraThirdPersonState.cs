using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraThirdPersonState : CameraBaseState
{
    private PlayerCamera playerCamera;
    public override void EnterState(PlayerCamera playerCmr)
    {
        playerCamera = playerCmr;

        playerCamera.activeCamera.gameObject.SetActive(false);
        playerCamera.activeCamera = playerCamera.thirdPersonCamera;
        playerCamera.StartCoroutine(OnEnterLerpCamera_Coroutine());
        playerCamera.activeCamera.gameObject.SetActive(true);

        //Tween.MoveTransformLocalPosition(playerCamera, playerCamera.cameraFollowPoint, new Vector3(0, 1, -5f), 0.2f);
        //playerCamera.StartCoroutine(OnEnterLerpCamera_Coroutine());
    }

    public override void ExitState()
    {

    }

    public override void UpdateState()
    {

    }
    public IEnumerator OnEnterLerpCamera_Coroutine()
    {
        Vector3 playerCameraDeltaPos = playerCamera.firstPersonCamera.transform.position - playerCamera.transform.forward;
        Quaternion lookAtPoint = Quaternion.LookRotation(playerCamera.firstPersonCamera.transform.forward, playerCamera.firstPersonCamera.transform.up);
        GameObject.Find("Roberto").transform.position = playerCameraDeltaPos;
        playerCamera.cinemachineFreeLook.gameObject.SetActive(true);
        playerCamera.cinemachineFreeLook.ForceCameraPosition(playerCameraDeltaPos, Quaternion.identity);

        yield return null;
    }
}
