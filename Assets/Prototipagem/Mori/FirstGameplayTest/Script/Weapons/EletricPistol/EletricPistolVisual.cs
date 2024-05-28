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
    [SerializeField] private SoundEmitter onFireSoundEmitter;

    public void OnUnchargedFire()
    {
        ZiumOnomatopeiaParticle.Play();
        onFireParticle.Play();
        onFireTrail.Play();
        TweenMunicao.Instance.ShakeAmmunition();
        onFireSoundEmitter.PlayAudio();
    }
    public void OnChargedFire()
    {
        OnUnCharge();
        ZiumOnomatopeiaParticle.Play();
        onFireParticle.Play();
        onFireTrail.Play();
        TweenMunicao.Instance.ShakeAmmunition();
        onFireSoundEmitter.PlayAudio();
    }
    public void OnCharge()
    {
        chargedParticle.Play();
        BzzOnomatopeiaParticle.Play();
    }
    public void OnUnCharge()
    {
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
