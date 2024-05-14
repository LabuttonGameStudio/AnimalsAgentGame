using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EletricPistolVisual : MonoBehaviour
{
    [SerializeField]private GameObject model;
    [SerializeField]private ParticleSystem chargedParticle;
    [SerializeField]private ParticleSystem onFireParticle;
    [SerializeField]private ParticleSystem onFireTrail;
    [SerializeField]private ParticleSystem onOverheat;
    [SerializeField]private ParticleSystem BzzOnomatopeiaParticle;
    public void OnUnchargedFire()
    {
        onFireParticle.Play();
        onFireTrail.Play();
        BzzOnomatopeiaParticle.Play();
        TweenMunicao.Instance.ShakeAmmunition();
    }
    public void OnChargedFire()
    {
        OnUnCharge();
        onFireParticle.Play();
        onFireTrail.Play();
        BzzOnomatopeiaParticle.Play();
        TweenMunicao.Instance.ShakeAmmunition();
    }
    public void OnCharge()
    {
        chargedParticle.Play();
    }
    public void OnUnCharge()
    {
        chargedParticle.Stop();
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
