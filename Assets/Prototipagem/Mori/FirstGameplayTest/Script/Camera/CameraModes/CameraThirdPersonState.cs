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
        //Vector3 playerCameraDeltaPos = playerCamera.transform.position;
        
        float firstPersonRotationX = playerCamera.firstPersonCamera.transform.rotation.eulerAngles.x;

        //float distanceMultiplier;
        //float height;

        playerCamera.cinemachineFreeLook.gameObject.SetActive(true);
        if (firstPersonRotationX <= 90)
        {
            float percentage = firstPersonRotationX / 77.5f;
            if(percentage>1) percentage = 1;
            playerCamera.cinemachineFreeLook.m_YAxis.Value = Mathf.Lerp(0.5f, 1, percentage);
        }
        else if (firstPersonRotationX >= 270 && firstPersonRotationX <= 360)
        {
            float percentage = (firstPersonRotationX - 270) / 78;
            if (percentage > 1) percentage = 1;
            playerCamera.cinemachineFreeLook.m_YAxis.Value = Mathf.Lerp(0.5f, 0, percentage);
        }
        //playerCameraDeltaPos -= playerCamera.transform.forward * distanceMultiplier;
        //playerCameraDeltaPos.y += height;
        //GameObject.Find("Roberto").transform.position = playerCameraDeltaPos;
        //yield return new WaitForSeconds(1);
        //playerCamera.cinemachineFreeLook.ForceCameraPosition(playerCamera.Roberto.position, Quaternion.identity);
        //playerCamera.cinemachineFreeLook.OnTargetObjectWarped(playerCamera.transform, playerCamera.transform.position,playerCamera.firstPersonCamera.transform.position);

        yield return null;
    }
}
