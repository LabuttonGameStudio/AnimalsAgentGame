using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISoundReceiver
{
    public abstract void OnSoundHear(SoundData soundData);
}
