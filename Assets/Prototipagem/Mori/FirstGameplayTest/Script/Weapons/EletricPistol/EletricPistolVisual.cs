using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EletricPistolVisual : MonoBehaviour
{
    [SerializeField]private GameObject model;
    [SerializeField]private ParticleSystem chargedParticle;
    [SerializeField]private ParticleSystem onFireParticle;
    [SerializeField]private ParticleSystem onOverheat;
    public void OnUnchargedFire()
    {
        onFireParticle.Play();
    }
    public void OnChargedFire()
    {
        OnUnCharge();
        onFireParticle.Play();
    }
    public void OnCharge()
    {
        chargedParticle.gameObject.SetActive(true);
    }
    public void OnUnCharge()
    {
        chargedParticle.gameObject.SetActive(false);
    }
    public void OnOverheat()
    {

    }
}
