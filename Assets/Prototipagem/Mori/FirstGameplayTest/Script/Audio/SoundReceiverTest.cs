using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static SoundGeneralControl;
using AudioType = SoundGeneralControl.AudioType;

public class SoundReceiverTest : MonoBehaviour, ISoundReceiver
{
    public float minimalSoundNeeded;

    public void OnSoundHear(SoundData soundData)
    {
        if(soundData.audioType == AudioType.Suspicious)
        {
            Debug.Log(minimalSoundNeeded < soundData.audioPercentage);
        }
    }
}
