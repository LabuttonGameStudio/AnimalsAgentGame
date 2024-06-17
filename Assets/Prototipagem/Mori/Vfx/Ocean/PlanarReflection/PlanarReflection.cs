using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanarReflection : MonoBehaviour
{
    private Camera mainCamera;

    [SerializeField] private Camera reflectionCamera;
    [SerializeField] private RenderTexture reflectionRenderTexture;

    [SerializeReference] private int reflectionResolution;
    private Vector2 resolution;
    [SerializeField] private Vector3 offset;
    private void Start()
    {
        mainCamera = PlayerCamera.Instance.mainCamera;
    }
    private void LateUpdate()
    {
        reflectionCamera.fieldOfView=  mainCamera.fieldOfView;

        Vector3 reflectionCameraPos = mainCamera.transform.position;
        reflectionCameraPos.y = -reflectionCameraPos.y + transform.position.y;
        reflectionCameraPos += offset;
        reflectionCamera.transform.position = reflectionCameraPos;

        Vector3 reflectionCameraRot = mainCamera.transform.rotation.eulerAngles;
        reflectionCameraRot.x = -reflectionCameraRot.x;
        //reflectionCameraRot.y = -reflectionCameraRot.y;
        reflectionCameraRot.z = 0;
        reflectionCamera.transform.rotation = Quaternion.Euler(reflectionCameraRot);

        resolution = new Vector2(mainCamera.pixelWidth, mainCamera.pixelHeight);

        reflectionRenderTexture.Release();
        reflectionRenderTexture.width = Mathf.RoundToInt(resolution.x) * reflectionResolution/Mathf.RoundToInt(resolution.y);
        reflectionRenderTexture.height = reflectionResolution;
    }
}
