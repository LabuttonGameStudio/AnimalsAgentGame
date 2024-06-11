using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EletricPistolVisual : MonoBehaviour
{
    [SerializeField]private GameObject model;
    [Header("VFX")]
    [SerializeField]private ParticleSystem chargedParticle;
    [SerializeField]private ParticleSystem onFireParticle;
    [SerializeField]private ParticleSystem onFireTrail;
    [SerializeField]private ParticleSystem onOverheat;
    [SerializeField]private ParticleSystem BzzOnomatopeiaParticle;
    [SerializeField]private ParticleSystem ZiumOnomatopeiaParticle;
    [Header("Audio")]
    [SerializeField] public Transform audioEmitterTransform;
    [SerializeField] private SoundEmitter onUnchargedFireSoundEmitter;
    [SerializeField] private SoundEmitter onChargedFireSoundEmitter;
    [SerializeField] private SoundEmitter onChargingFireSoundEmitter;

    public void OnUnchargedFire()
    {
        ZiumOnomatopeiaParticle.Play();
        onFireParticle.Play();
        onFireTrail.Play();
        //TweenMunicao.Instance.ShakeAmmunition();
        onUnchargedFireSoundEmitter.PlayAudio();
    }
    public void OnChargedFire()
    {
        OnUnCharge();
        ZiumOnomatopeiaParticle.Play();
        onFireParticle.Play();
        onFireTrail.Play();
        //TweenMunicao.Instance.ShakeAmmunition();
        onChargedFireSoundEmitter.PlayAudio();
    }
    public void OnCharge()
    {
        onChargingFireSoundEmitter.PlayAudio();
        chargedParticle.Play();
        BzzOnomatopeiaParticle.Play();
    }
    public void OnUnCharge()
    {
        onChargingFireSoundEmitter.StopAudio();
        chargedParticle.Stop();
        BzzOnomatopeiaParticle.Stop();
    }
    public void OnEnterOverheatMode()
    {
        onOverheat.Play();
    }
    public void OnLeaveOverheatMode()
    {
        onOverheat.Stop();
    }
}
