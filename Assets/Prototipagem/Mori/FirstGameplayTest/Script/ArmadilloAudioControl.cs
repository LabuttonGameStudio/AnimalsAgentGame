using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ArmadilloMovementController;
public class ArmadilloAudioControl : MonoBehaviour
{
    public SoundEmitter currentMovingSound { get; private set; }

    public void ChangeCurrentMovingForm(MovementType newMovementType)
    {
        if(currentMovingSound != null)
        {
            currentMovingSound.StopAudio();
        }
        switch(newMovementType)
        {
            case MovementType.Default:
                currentMovingSound = walkingSound;
                break;
            case MovementType.Sprinting:
                currentMovingSound = sprintSound;
                break;
            case MovementType.Lurking:
                currentMovingSound = lurkingSound;
                break;  
        }
    }
    private SoundEmitter walkingSound;
    private SoundEmitter sprintSound;
    private SoundEmitter lurkingSound;
    public SoundEmitter onFallSound;
    public SoundEmitter onTransformSound;
}
