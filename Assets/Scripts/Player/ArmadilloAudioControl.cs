using Pixeye.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ArmadilloMovementController;
public class ArmadilloAudioControl : MonoBehaviour
{
    [Foldout("Movement",true)]
    [SerializeField] private SoundEmitter walkingSound;
    [SerializeField] private SoundEmitter sprintSound;
    [SerializeField] private SoundEmitter lurkingSound;
    public SoundEmitter onFallSound;
    public SoundEmitter onTransformSound;
    public AudioSource onLedgeGrab;

    [Header("Ball")]
    public AudioSource onBallJump;
    public AudioSource onBallDealDamage;
    public AudioSource onBallRoll;
    public SoundEmitter currentMovingSound { get; private set; }

    [Foldout("Guns",true)]
    public AudioSource onDamageOnly;
    public AudioSource onCritical;

    [Foldout("Sonar", true)]
    public AudioSource onSonar;

    [Foldout("Melee", true)]
    public AudioSource onWeakMelee;
    public AudioSource onStrongMelee;

    [Foldout("Damage")]
    public AudioSource[] onTakeDamageVariations;
    private int lastPlayedOnTakeDamageAudio;
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
    public void OnTakeDamage()
    {
        int audio = Random.Range(0, onTakeDamageVariations.Length);
        if(audio == lastPlayedOnTakeDamageAudio)
        {
            for(int i = 0;i<50;i++)
            {
                audio = Random.Range(0, onTakeDamageVariations.Length);
                if (audio != lastPlayedOnTakeDamageAudio) break;
            }
        }
        lastPlayedOnTakeDamageAudio = audio;
        onTakeDamageVariations[audio].Play();
    }
}
