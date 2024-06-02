using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ArmadilloMovementController;
public class ArmadilloAudioControl : MonoBehaviour
{
    [SerializeField] private SoundEmitter walkingSound;
    [SerializeField] private SoundEmitter sprintSound;
    [SerializeField] private SoundEmitter lurkingSound;
    public SoundEmitter onFallSound;
    public SoundEmitter onTransformSound;
    public SoundEmitter currentMovingSound { get; private set; }
    private void Awake()
    {
        currentMovingSound = walkingSound;
    }
    public void ChangeCurrentMovingForm(MovementType newMovementType)
    {
        if (currentMovingSound != null)
        {
            currentMovingSound.StopAudio();
        }
        switch (newMovementType)
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
    public void PlayMovingAudio()
    {
        currentMovingSound.PlayAudio();
    }
    public void StopMovingAudio()
    {
        currentMovingSound.StopAudio();
    }
}
