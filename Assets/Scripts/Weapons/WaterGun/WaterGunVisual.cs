using Pixeye.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.VFX;
using static ArmadilloVisualControl;

public class WaterGunVisual : MonoBehaviour
{
    public static WaterGunVisual Instance;
    [SerializeField] private GameObject model;
    public Transform firePivot;

    [SerializeField] private Animator weaponAnimator;
    [SerializeField] private VisualEffect waterSpray;
    [SerializeField] private ParticleSystem sploshOnomatopeia;
    [SerializeField] private ParticleSystem chkOnomatopeia;
    [SerializeField] private ParticleSystem plopOnomatopeia;
    [SerializeField] private ParticleSystem onFireMuzzleFlash;
    [HideInInspector]public  Vector3 waterSpayVelocity;

    private ShakeStats onFireShake;

    [Foldout("SFX", true)]
    [SerializeField] private AudioSource onEquipAudio; 
    [SerializeField] private AudioSource onReloadAudio; 
    [SerializeField] private AudioSource onFireAudio; 
    private void Awake()
    {
        Instance = this;
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
            onEquipAudio.Play();
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
            if (onFireShake != null)
            {
                FPCameraShake.StopShake(onFireShake);
            }
            onFireShake = FPCameraShake.StartShake(0.15f, 1);
            onFireMuzzleFlash.Play();
            waterSpray.Play();
            sploshOnomatopeia.Play();
            onFireAudio.Play();
        }
        else
        {
            if(onFireShake != null)
            {
                FPCameraShake.StopShake(onFireShake);
                onFireShake = null;
            }
            onFireMuzzleFlash.Stop();
            waterSpray.Stop();
            sploshOnomatopeia.Stop();
            onFireAudio.Stop();
        }
    }
    public void OnReload()
    {
        weaponAnimator.SetTrigger("waterGunReload");
        chkOnomatopeia.Play();
        plopOnomatopeia.Play();
        onReloadAudio.Play();
    }
    public void ToggleOnRun(bool state)
    {
        weaponAnimator.SetBool("waterGunIsRunning",state);
    }

    public void UpdateUI(int currentAmount, int totalAmount, int reserveAmount, int reserveTotal)
    {
        if(ArmadilloPlayerController.Instance.weaponControl.currentWeaponID == 1)
        {
            Slider ammoSlider = ArmadilloPlayerController.Instance.weaponControl.Weaponammoslider;
            ammoSlider.value = (float)currentAmount / (float)totalAmount;
            ammoSlider.fillRect.GetComponent<Image>().color = Color.Lerp(Color.red, Color.white, ammoSlider.value);
            ArmadilloPlayerController.Instance.weaponControl.currentWeaponUI.text = " x" + Mathf.Round(ammoSlider.value * 100).ToString() + "%";
        }
        ChangeWeaponUI.Instance.weaponUIElements[1].ammoReserveSlider.value = (float)reserveAmount / (float)reserveTotal;
        ChangeWeaponUI.Instance.weaponUIElements[1].ammoReserveText.text = Mathf.Round((float)reserveAmount / (float)reserveTotal * 100).ToString() + "%";
    }

    public void OnBodyShot()
    {
        Debug.Log("BodyShot");
    }
    public void OnHeadshotShot()
    {
        Debug.Log("HeadShot");
    }
    public void OnLetalShot()
    {
        Debug.Log("LetalShot");
    }
}
