using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using Unity.VisualScripting.Antlr3.Runtime.Misc;

public class ShakeStats
{
    public Coroutine coroutine;
    public float duration;
    public float amplitudeGain;
    public float frequency;
}
public class PivotOffsetStats
{
    public Coroutine coroutine;
    public Vector3 currentOffset;
    public Vector3 offset;
    public float frequency;
    public float duration;
    public float fadeInTime;
    public float fadeOutTime;
}
public class FPCameraShake : MonoBehaviour
{
    public static FPCameraShake Instance;

    private CinemachineVirtualCamera virtualCamera;
    private CinemachineBasicMultiChannelPerlin channelPerlin;

    private CinemachineCameraOffset[] cinemachineCameraOffsetArray;
    private void Awake()
    {
        currentOperatingOffset = new List<PivotOffsetStats>();
        virtualCamera = GetComponent<CinemachineVirtualCamera>();
        channelPerlin = virtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        Instance = this;
    }
    private void Start()
    {
        cinemachineCameraOffsetArray = new CinemachineCameraOffset[2];
        cinemachineCameraOffsetArray[0] = ArmadilloPlayerController.Instance.cameraControl.firstPersonCinemachine.GetComponent<CinemachineCameraOffset>();
        cinemachineCameraOffsetArray[1] = ArmadilloPlayerController.Instance.cameraControl.thirdPersonCinemachine.GetComponent<CinemachineCameraOffset>();
    }
    #region Shake
    public static ShakeStats StartShake(float duration, float amplitudeGain, float frequency)
    {
        ShakeStats stats = new ShakeStats() { duration = duration, amplitudeGain = amplitudeGain, frequency = frequency };
        stats.coroutine = Instance.StartCoroutine(Instance.Shake_Coroutine(stats, true));
        return stats;
    }
    public static ShakeStats StartShake(float amplitudeGain, float frequency)
    {
        ShakeStats stats = new ShakeStats() { amplitudeGain = amplitudeGain, frequency = frequency };
        stats.coroutine = Instance.StartCoroutine(Instance.Shake_Coroutine(stats, false));
        return stats;
    }
    public static void StopShake(ShakeStats stats)
    {
        Instance.channelPerlin.m_AmplitudeGain -= stats.amplitudeGain;
        Instance.channelPerlin.m_FrequencyGain -= stats.frequency;
    }
    private IEnumerator Shake_Coroutine(ShakeStats shake, bool hasDuration)
    {
        channelPerlin.m_AmplitudeGain += shake.amplitudeGain;
        channelPerlin.m_FrequencyGain += shake.frequency;

        if (hasDuration)
        {
            yield return new WaitForSeconds(shake.duration);

            channelPerlin.m_AmplitudeGain -= shake.amplitudeGain;
            channelPerlin.m_FrequencyGain -= shake.frequency;
        }

    }
    #endregion

    #region PivotOffSet
    private List<PivotOffsetStats> currentOperatingOffset;

    //----- Pivot Offset Fixed -----
    #region Pivot Offset Fixed

    #region Start Pivot Offset Fixed Functions
    public PivotOffsetStats StartPivotOffsetFixed(float duration, Vector3 offset, float fadeInTime, float fadeOutTime)
    {
        PivotOffsetStats stats = new PivotOffsetStats() { duration = duration, offset = offset, fadeInTime = fadeInTime, fadeOutTime = fadeOutTime };
        stats.coroutine = StartCoroutine(PivotOffsetFixed_Coroutine(stats));
        currentOperatingOffset.Add(stats);
        if (updateCurrentOffset_Ref == null)
        {
            updateCurrentOffset_Ref = StartCoroutine(UpdateCurrentOffset_Coroutine());
        }
        return stats;
    }
    public PivotOffsetStats StartPivotOffsetFixed(Vector3 offset,float fadeInTime)
    {
        PivotOffsetStats stats = new PivotOffsetStats() { offset = offset, fadeInTime = fadeInTime};
        stats.coroutine = StartCoroutine(FadePivotOffsetFixed_Coroutine(stats, FadeType.FadeIn));
        currentOperatingOffset.Add(stats);
        if (updateCurrentOffset_Ref == null)
        {
            updateCurrentOffset_Ref = StartCoroutine(UpdateCurrentOffset_Coroutine());
        }
        return stats;
    }
    #endregion

    #region  Pivot Offset Fixed Coroutines
    private IEnumerator PivotOffsetFixed_Coroutine(PivotOffsetStats stats)
    {
        yield return StartCoroutine(FadePivotOffsetFixed_Coroutine(stats, FadeType.FadeIn));
        yield return new WaitForSeconds(stats.duration);
        yield return StoptPivotOffsetFixed_Coroutine(stats);
    }
    enum FadeType
    {
        FadeIn,
        FadeOut
    }
    private IEnumerator FadePivotOffsetFixed_Coroutine(PivotOffsetStats stats, FadeType fadeType)
    {
        float lerpDuration;
        Vector3 startValue;
        Vector3 finalValue;
        switch (fadeType)
        {
            default:
            case FadeType.FadeIn:
                lerpDuration = stats.fadeInTime;
                startValue = Vector3.zero;
                finalValue = stats.offset;
                break;
            case FadeType.FadeOut:
                lerpDuration = stats.fadeOutTime;
                startValue = stats.currentOffset;
                finalValue = Vector3.zero;
                break;
        }
        float timer = 0;
        while (timer < lerpDuration)
        {
            stats.currentOffset = Vector3.Lerp(startValue, finalValue, timer / lerpDuration);
            timer += Time.deltaTime;
            yield return null;
        }
        stats.currentOffset = finalValue;
    }

    #endregion

    #region Stop Pivot Offset Fixed Functions
    public void StoptPivotOffsetFixed(PivotOffsetStats stats)
    {
        StopCoroutine(stats.coroutine);
        if (stats.fadeOutTime > 0)
        {
            StoptPivotOffsetFixed_Coroutine(stats);
        }
        else
        {
            stats.currentOffset = Vector3.zero;
            currentOperatingOffset.Remove(stats);
            if (currentOperatingOffset.Count <= 0 && updateCurrentOffset_Ref != null)
            {
                StopCoroutine(updateCurrentOffset_Ref);
                updateCurrentOffset_Ref = null;
            }
        }
    }
    private IEnumerator StoptPivotOffsetFixed_Coroutine(PivotOffsetStats stats)
    {
        yield return StartCoroutine(FadePivotOffsetFixed_Coroutine(stats, FadeType.FadeOut));
        stats.currentOffset = Vector3.zero;
        currentOperatingOffset.Remove(stats);
        if (currentOperatingOffset.Count <= 0 && updateCurrentOffset_Ref != null)
        {
            StopCoroutine(updateCurrentOffset_Ref);
            updateCurrentOffset_Ref = null;
        }
    }

    #endregion

    #endregion

    //----- Pivot Offset Loop ---
    #region Pivot Offset Loop

    #region Start Pivot Offset Functions
    public PivotOffsetStats StartPivotOffsetLoop(float duration, float frequency, Vector3 offset, float fadeOutTime)
    {
        PivotOffsetStats stats = new PivotOffsetStats() { duration = duration, offset = offset, frequency = frequency, fadeOutTime = fadeOutTime };
        stats.coroutine = StartCoroutine(PivotOffsetLoop_Coroutine(stats, true));
        Instance.currentOperatingOffset.Add(stats);
        if (updateCurrentOffset_Ref == null)
        {
            updateCurrentOffset_Ref = StartCoroutine(UpdateCurrentOffset_Coroutine());
        }
        return stats;
    }
    public PivotOffsetStats StartPivotOffsetLoop(float frequency, Vector3 offset)
    {
        PivotOffsetStats stats = new PivotOffsetStats() { offset = offset, frequency = frequency };
        stats.coroutine = StartCoroutine(PivotOffsetLoop_Coroutine(stats, false));
        currentOperatingOffset.Add(stats);
        if (updateCurrentOffset_Ref == null)
        {
            updateCurrentOffset_Ref = StartCoroutine(UpdateCurrentOffset_Coroutine());
        }
        return stats;
    }
    #endregion

    #region Pivot Offset Coroutine
    private IEnumerator PivotOffsetLoop_Coroutine(PivotOffsetStats stats, bool hasDuration)
    {
        if (hasDuration)
        {
            float timer = 0;
            while (timer < stats.duration)
            {
                stats.currentOffset = Vector3.Lerp(stats.offset, stats.offset * (-1), (1 + Mathf.Sin(timer * stats.frequency)) / 2);
                timer += Time.deltaTime;
                yield return null;
            }
            timer = 0;
            while (timer < stats.fadeOutTime)
            {
                stats.currentOffset = Vector3.Lerp(stats.currentOffset, Vector3.zero, timer / stats.fadeOutTime);
                timer += Time.deltaTime;
                yield return null;
            }
            stats.currentOffset = Vector3.zero;
            StopPivotOffset(stats);
        }
        else
        {
            while (true)
            {
                stats.currentOffset = Vector3.Lerp(stats.offset, stats.offset * (-1), (1 + Mathf.Sin(Time.time * stats.frequency)) / 2);
                yield return null;
            }
        }

    }
    #endregion

    #region Stop Pivot Offset Functions
    public void StopPivotOffset(PivotOffsetStats stats)
    {
        StopCoroutine(stats.coroutine);
        if (stats.fadeOutTime > 0)
        {
            StartCoroutine(PivotOffsetFadeOut_Coroutine(stats));
        }
        else
        {
            stats.currentOffset = Vector3.zero;
            currentOperatingOffset.Remove(stats);
            if (currentOperatingOffset.Count <= 0 && updateCurrentOffset_Ref != null)
            {
                StopCoroutine(updateCurrentOffset_Ref);
                updateCurrentOffset_Ref = null;
            }
        }
    }
    private IEnumerator PivotOffsetFadeOut_Coroutine(PivotOffsetStats stats)
    {
        float timer = 0;
        while (timer < stats.fadeOutTime)
        {
            stats.currentOffset = Vector3.Lerp(stats.currentOffset, Vector3.zero, timer / stats.fadeOutTime);
            timer += Time.deltaTime;
            yield return null;
        }
        stats.currentOffset = Vector3.zero;
        currentOperatingOffset.Remove(stats);
        if (currentOperatingOffset.Count <= 0 && updateCurrentOffset_Ref != null)
        {
            StopCoroutine(updateCurrentOffset_Ref);
            updateCurrentOffset_Ref = null;
        }

    }
    #endregion

    #endregion

    //----- Update Current Camera Offset ------
    #region Update Current Camera Offset
    private Coroutine updateCurrentOffset_Ref;
    private IEnumerator UpdateCurrentOffset_Coroutine()
    {
        while (true)
        {
            SetCurrentOffset();
            yield return null;
        }
    }
    private void SetCurrentOffset()
    {
        Vector3 currentOffset = Vector3.zero;
        for (int i = 0; i < currentOperatingOffset.Count; i++)
        {
            currentOffset += currentOperatingOffset[i].currentOffset;
        }
        cinemachineCameraOffsetArray[0].m_Offset = currentOffset;
        cinemachineCameraOffsetArray[1].m_Offset = currentOffset;
    }
    #endregion

    #endregion
}
