using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;
using static ArmadilloVisualControl;

public class WaterGunVisual : MonoBehaviour
{
    public static WaterGunVisual Instance;
    [SerializeField] private GameObject model;
    public Transform firePivot;

    [SerializeField] private Animator weaponAnimator;
    [SerializeField] private VisualEffect waterSpray;
    private float waterSpayVelocityDefault;
    private void Awake()
    {
        Instance = this;
        waterSpayVelocityDefault = waterSpray.GetFloat("FrontForce");
    }
    public void ToggleVisual(bool state)
    {
        model.SetActive(state);
    }
    public void UpdateSprayVelocity(Vector3 adicionalVelocity)
    {
        waterSpray.SetVector3("BodyVelocity", adicionalVelocity);
    }
    public void ResetVisuals()
    {

    }
    public void OnEquip(bool playAnimation)
    {
        if (playAnimation)
        {
            weaponAnimator.CrossFade("WaterGunEquip", crossFadeTime);
        }
        else
        {
            weaponAnimator.CrossFade("WaterGunIdle", crossFadeTime);
        }

    }
    public void OnUnequip()
    {
        weaponAnimator.CrossFade("WaterGunUnequip", crossFadeTime);
    }
    public void ToggleOnFire(bool state)
    {
        weaponAnimator.SetBool("waterGunIsFiring", state);
        if (state)
        {
            waterSpray.Play();
        }
        else
        {
            waterSpray.Stop();
        }
    }
    public void OnReload()
    {
        weaponAnimator.SetTrigger("waterGunReload");
    }
    public void ToggleOnRun(bool state)
    {
        weaponAnimator.SetBool("waterGunIsRunning",state);
    }
}
