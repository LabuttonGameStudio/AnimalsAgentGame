using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using UnityEngine;
using static SoundGeneralControl;
using AudioType = SoundGeneralControl.AudioType;
public class SoundData
{
    public Vector3 originPoint;
    public float audioPercentage;
    public AudioType audioType;
}
public class SoundGeneralControl : MonoBehaviour
{
    public static SoundGeneralControl Instance;
    public enum AudioType
    {
        Ambient,
        Suspicious
    }
    private void Awake()
    {
        Instance = this;
       
    }
    public LayerMask soundOcclusion_LayerMask;
    public LayerMask soundReceivers_LayerMask;
}
