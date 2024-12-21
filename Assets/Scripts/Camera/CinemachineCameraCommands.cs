using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CinemachineCameraCommands : MonoBehaviour
{
    private CinemachineVirtualCamera virtualCamera;
    private void Awake()
    {
        virtualCamera = GetComponent<CinemachineVirtualCamera>();
    }
    public void TeleportCamera(Transform newPositionTransform)
    {
        virtualCamera.ForceCameraPosition(newPositionTransform.position, newPositionTransform.rotation);
    }
}
