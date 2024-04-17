using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

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
    public Vector3 minPivotOffset;
    public Vector3 maxPivotOffset;
    public float frequency;
    public float duration;
}
public class FPCameraShake : MonoBehaviour
{
    private static FPCameraShake Instance;

    private CinemachineVirtualCamera virtualCamera;
    private CinemachineBasicMultiChannelPerlin channelPerlin;
    private void Awake()
    {
        virtualCamera = GetComponent<CinemachineVirtualCamera>();
        channelPerlin = virtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        Instance = this;
    }
    #region Shake
    public static ShakeStats StartShake(float duration, float amplitudeGain, float frequency)
    {
        ShakeStats stats = new ShakeStats() { duration = duration, amplitudeGain = amplitudeGain, frequency = frequency };
        stats.coroutine = Instance.StartCoroutine(Instance.Shake_Coroutine(stats,true));
        return stats;
    }
    public static ShakeStats StartShake(float amplitudeGain, float frequency)
    {
        ShakeStats stats = new ShakeStats() {amplitudeGain = amplitudeGain, frequency = frequency };
        stats.coroutine = Instance.StartCoroutine(Instance.Shake_Coroutine(stats,false));
        return stats;
    }
    public static void StopShake(ShakeStats stats)
    {
        Instance.channelPerlin.m_AmplitudeGain -= stats.amplitudeGain;
        Instance.channelPerlin.m_FrequencyGain -= stats.frequency;
    }
    private IEnumerator Shake_Coroutine(ShakeStats shake,bool hasDuration)
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

    public static PivotOffsetStats StartPivotOffset(float duration, float frequency, Vector3 minOffset,Vector3 maxOffset)
    {
        PivotOffsetStats stats = new PivotOffsetStats() { duration = duration, minPivotOffset = minOffset, maxPivotOffset = maxOffset,frequency = frequency };
        stats.coroutine = Instance.StartCoroutine(Instance.PivotOffset_Coroutine(stats, true));
        return stats;
    }
    public static PivotOffsetStats StartPivotOffset(float frequency, Vector3 minOffset, Vector3 maxOffset)
    {
        PivotOffsetStats stats = new PivotOffsetStats() {minPivotOffset = minOffset, maxPivotOffset = maxOffset, frequency = frequency };
        stats.coroutine = Instance.StartCoroutine(Instance.PivotOffset_Coroutine(stats, false));
        return stats;
    }
    public static void StopPivotOffset(PivotOffsetStats stats)
    {
        
    }
    private IEnumerator PivotOffset_Coroutine(PivotOffsetStats stats,bool hasDuration)
    {
        float timer = 0;

        while (true)
        {
            stats.currentOffset = Vector3.Lerp(Vector3.up, Vector3.up * (-1), Mathf.Sin(Time.time)) ;
            yield return null;
        }

    }
    #endregion
}
