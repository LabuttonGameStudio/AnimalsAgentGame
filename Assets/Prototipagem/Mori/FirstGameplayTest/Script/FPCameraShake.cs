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
}
