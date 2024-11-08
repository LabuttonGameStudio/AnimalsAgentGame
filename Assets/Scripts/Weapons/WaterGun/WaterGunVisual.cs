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
    [HideInInspector]public  Vector3 waterSpayVelocity;
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

    public void UpdateUI(int currentAmount, int totalAmount, int reserveAmount, int reserveTotal)
    {
        Slider ammoSlider = ArmadilloPlayerController.Instance.weaponControl.Weaponammoslider;
        ammoSlider.value = (float)currentAmount / (float)totalAmount;
        ammoSlider.fillRect.GetComponent<Image>().color = Color.Lerp(Color.red, Color.white, ammoSlider.value);
        ArmadilloPlayerController.Instance.weaponControl.currentWeaponUI.text = " x" + Mathf.Round(ammoSlider.value * 100).ToString() + "%";

        ChangeWeaponUI.Instance.weaponUIElements[1].ammoReserveSlider.value = (float)reserveAmount / (float)reserveTotal;
        ChangeWeaponUI.Instance.weaponUIElements[1].ammoReserveText.text = Mathf.Round((float)reserveAmount / (float)reserveTotal * 100).ToString() + "%";
    }
}
