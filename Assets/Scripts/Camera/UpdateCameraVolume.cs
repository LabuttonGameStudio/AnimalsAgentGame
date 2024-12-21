using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class UpdateCameraVolume : MonoBehaviour
{
    public static UpdateCameraVolume Instance;
    private enum QualityLevelEnum
    {
        Low,
        Medium,
        High,
        Ultra
    }
    private Dictionary<QualityLevelEnum, float> qualityLevelUpdateInterval;
    Camera mainCamera;
    int CurrentQualitySetting;
    private void Awake()
    {
        Instance = this;
        mainCamera = GetComponent<Camera>();

        qualityLevelUpdateInterval = new Dictionary<QualityLevelEnum, float>();
        qualityLevelUpdateInterval.Add(QualityLevelEnum.Low, 0.1f);
        qualityLevelUpdateInterval.Add(QualityLevelEnum.Medium, Time.fixedDeltaTime);
        qualityLevelUpdateInterval.Add(QualityLevelEnum.High, 0);
        qualityLevelUpdateInterval.Add(QualityLevelEnum.Ultra, 0);
        StartCoroutine(UpdateCameraVolume_Coroutine());
    }
    IEnumerator UpdateCameraVolume_Coroutine()
    {
        while(true)
        {
            if (mainCamera.GetVolumeFrameworkUpdateMode() == VolumeFrameworkUpdateMode.ViaScripting)
            {
                mainCamera.UpdateVolumeStack();
                yield return new WaitForSecondsRealtime(qualityLevelUpdateInterval[(QualityLevelEnum)QualitySettings.GetQualityLevel()]);
            }
        }
    }
}
