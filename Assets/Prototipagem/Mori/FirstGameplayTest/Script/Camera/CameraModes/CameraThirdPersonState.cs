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
        playerCamera.activeCamera.gameObject.SetActive(true);

        //Tween.MoveTransformLocalPosition(playerCamera, playerCamera.cameraFollowPoint, new Vector3(0, 1, -5f), 0.2f);
        playerCamera.cinemachineFreeLook.gameObject.SetActive(true);
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
        Vector3 deltaPositionCamera;
        deltaPositionCamera = playerCamera.thirdPersonCamera.transform.position;
        playerCamera.cinemachineFreeLook.gameObject.SetActive(true);
        yield return null;
        deltaPositionCamera -= playerCamera.thirdPersonCamera.transform.position;
        playerCamera.cinemachineCameraOffset.m_Offset = deltaPositionCamera;
        float timer = 0f;
        while (timer<0.25f)
        {
            playerCamera.cinemachineCameraOffset.m_Offset = Vector3.Lerp(deltaPositionCamera, Vector3.zero, timer / 0.25f);
            timer += Time.deltaTime;
            yield return null;
        }
        playerCamera.cinemachineCameraOffset.m_Offset = Vector3.zero;
    }
}
