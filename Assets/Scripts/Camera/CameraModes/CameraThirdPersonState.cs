using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraThirdPersonState : CameraBaseState
{
    private PlayerCamera playerCamera;
    public override void EnterState(PlayerCamera playerCmr)
    {
        playerCamera = playerCmr;
        playerCamera.thirdPersonCinemachine.Follow.rotation = playerCamera.mainCamera.transform.rotation;
        playerCamera.firstPersonCinemachine.Priority = 10;
        playerCamera.thirdPersonCinemachine.Priority = 11;
    }

    public override void ExitState()
    {

    }

    public override void UpdateState()
    {
        Transform followTransform = playerCamera.thirdPersonCinemachine.Follow;
        followTransform.rotation *= Quaternion.AngleAxis(playerCamera.GetMouseDelta().x/8 * playerCamera.thirdPersonSensibility.x, Vector3.up);
        followTransform.rotation *= Quaternion.AngleAxis(-playerCamera.GetMouseDelta().y/8 * playerCamera.thirdPersonSensibility.y, Vector3.right);
        Vector3 angles = followTransform.localEulerAngles;
        angles.z = 0;
        float angle = followTransform.localEulerAngles.x;

        if(angle>180 && angle<340)
        {
            angles.x = 340;
        }
        else if (angle < 180 && angle > 60)
        {
            angles.x = 60;
        }
        followTransform.localEulerAngles = angles;

    }
    public IEnumerator OnEnterLerpCamera_Coroutine()
    {
        ////Vector3 playerCameraDeltaPos = playerCamera.transform.position;

        //float firstPersonRotationX = playerCamera.firstPersonCamera.transform.rotation.eulerAngles.x;

        //////float distanceMultiplier;
        //////float height;

        ////playerCamera.cinemachineFreeLook.gameObject.SetActive(true);
        //if (firstPersonRotationX <= 90)
        //{
        //    float percentage = firstPersonRotationX / 77.5f;
        //    if (percentage > 1) percentage = 1;
        //    playerCamera.cinemachineFreeLook.m_YAxis.Value = Mathf.Lerp(0.5f, 1, percentage);
        //}
        //else if (firstPersonRotationX >= 270 && firstPersonRotationX <= 360)
        //{
        //    float percentage = (firstPersonRotationX - 270) / 78;
        //    if (percentage > 1) percentage = 1;
        //    playerCamera.cinemachineFreeLook.m_YAxis.Value = Mathf.Lerp(0.5f, 0, percentage);
        //}
        ////playerCameraDeltaPos -= playerCamera.transform.forward * distanceMultiplier;
        ////playerCameraDeltaPos.y += height;
        ////GameObject.Find("Roberto").transform.position = playerCameraDeltaPos;
        ////yield return new WaitForSeconds(1);
        ////playerCamera.cinemachineFreeLook.ForceCameraPosition(playerCamera.Roberto.position, Quaternion.identity);
        ////playerCamera.cinemachineFreeLook.OnTargetObjectWarped(playerCamera.transform, playerCamera.transform.position,playerCamera.firstPersonCamera.transform.position);

        yield return null;
    }
}
