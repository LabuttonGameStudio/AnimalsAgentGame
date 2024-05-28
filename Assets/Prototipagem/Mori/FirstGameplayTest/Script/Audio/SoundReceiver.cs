using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface SoundReceiver 
{
    public abstract void OnSoundHear(SoundData soundData);
}
