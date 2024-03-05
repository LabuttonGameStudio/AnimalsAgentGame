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
        
        //float firstPersonRotationX = playerCamera.firstPersonCamera.transform.rotation.eulerAngles.x;

        //float distanceMultiplier;
        //float height;

        //if(firstPersonRotationX <= 90 && firstPersonRotationX > 30)
        //{
        //    //Bottom rig
        //    distanceMultiplier = playerCamera.cinemachineFreeLook.m_Orbits[0].m_Radius;
        //    height = playerCamera.cinemachineFreeLook.m_Orbits[0].m_Height;
        //    Debug.Log("Top Rig| Radius="+distanceMultiplier+"| Height="+height);
        //}
        //else if(firstPersonRotationX <= 30 || (firstPersonRotationX <= 360 && firstPersonRotationX > 330))
        //{
        //    //Middle rig
        //    distanceMultiplier = playerCamera.cinemachineFreeLook.m_Orbits[1].m_Radius;
        //    height = playerCamera.cinemachineFreeLook.m_Orbits[1].m_Height;
        //    Debug.Log("Middle Rig| Radius=" + distanceMultiplier + "| Height=" + height);
        //}
        //else
        //{
        //    //Top rig
        //    distanceMultiplier = playerCamera.cinemachineFreeLook.m_Orbits[2].m_Radius;
        //    height = playerCamera.cinemachineFreeLook.m_Orbits[2].m_Height;
        //    Debug.Log("Bottom Rig| Radius=" + distanceMultiplier + "| Height=" + height);
        //}

        //playerCameraDeltaPos -= playerCamera.transform.forward * distanceMultiplier;
        //playerCameraDeltaPos.y += height;
        //GameObject.Find("Roberto").transform.position = playerCameraDeltaPos;
        playerCamera.cinemachineFreeLook.gameObject.SetActive(true);
        yield return new WaitForSeconds(1);
        playerCamera.cinemachineFreeLook.OnTargetObjectWarped(playerCamera.transform, playerCamera.transform.position,playerCamera.firstPersonCamera.transform.position);
        //playerCamera.cinemachineFreeLook.ForceCameraPosition(playerCameraDeltaPos, Quaternion.identity);

        yield return null;
    }
}
