using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using static WeaponData;

public class EletricPistol : Weapon
{
    private ArmadilloWeaponControl weaponControl;
    public EletricPistol(ArmadilloWeaponControl armadilloWeaponControl)
    {
        weaponControl = armadilloWeaponControl;
        ammoType = AmmoType.Ammo;
        maxAmmoAmount = 12;
        currentAmmoAmount = 12;
        maxAmmoAmount = 24;
        maxAmmoReserveAmount = 36;
        name = "Pistola Eletrica";
        description = "Pressione pra atirar, segure para carregar ";
        ammoSlider = armadilloWeaponControl.Weaponammoslider;
        weaponType = WeaponType.Zapgun;
        LoadVisual();
    }

    //----- Stats -----
    //Tiro normal
    readonly private float fireDelay = 0.33f;
    private bool isOnCooldown;

    readonly private int unchargedHitDamage = 15;

    readonly private float damageFallOffStartRange;
    readonly private float damageFallOffEndRange;
    //----- Visual -----
    private EletricPistolVisual visualHandler;

    //Visual
    #region
    private void LoadVisual()
    {
        visualHandler = EletricPistolVisual.Instance;
        visualHandler.ToggleVisual(false);
    }

    public override void ToggleVisual(bool state)
    {
        visualHandler.ToggleVisual(state);
    }
    public override void OnEquip(bool playAnimation)
    {
        if (playAnimation) ArmadilloPlayerController.Instance.visualControl.EquipEletricPistolAnim();
        else ArmadilloPlayerController.Instance.visualControl.EquipEletricPistol();

    }
    public override void OnUnequip()
    {
        ArmadilloPlayerController.Instance.visualControl.UnequipEletricPistol();
        weaponControl.StartCoroutine(OnUnequipAnimEnd());
    }
    public IEnumerator OnUnequipAnimEnd()
    {
        yield return new WaitForSeconds(0.16f);
        visualHandler.ToggleVisual(false);
    }
    #endregion

    //Fire
    #region
    public void StartFireCooldownTimer()
    {
        if (fireCooldown_Ref == null && !isOnCooldown)
        {
            fireCooldown_Ref = weaponControl.StartCoroutine(FireCooldown_Coroutine());
        }
    }
    public bool IsFireOnCooldown()
    {
        return (fireCooldown_Ref != null);
    }
    public Coroutine fireCooldown_Ref;
    public IEnumerator FireCooldown_Coroutine()
    {
        yield return new WaitForSeconds(fireDelay);
        isOnCooldown = false;
        fireCooldown_Ref = null;
    }

    public override void OnFireButtonPerformed(InputAction.CallbackContext performed)
    {
        if (!IsFireOnCooldown()) Fire();
    }
    public override void OnFireButtonCanceled(InputAction.CallbackContext performed)
    {
        if (!IsFireOnCooldown()) Fire();
    }

    public void Fire()
    {
        FPCameraShake.StartShake(0.15f, 0.4f, 5f);
        FireRaycast();
        ArmadilloPlayerController.Instance.visualControl.OnEletricGunFire();
        ArmadilloPlayerController.Instance.visualControl.ToggleEletricPistolCharge(false);
        StartFireCooldownTimer();
    }
    public void FireRaycast()
    {
        RaycastHit[] targetsHit;
        Transform camera = ArmadilloPlayerController.Instance.cameraControl.mainCamera.transform;
        targetsHit = Physics.RaycastAll(camera.position, camera.forward, 20f, Physics.AllLayers, QueryTriggerInteraction.Collide);
        Vector3 hitPoint = camera.position + camera.forward * 20f;
        foreach (RaycastHit raycastHit in targetsHit)
        {
            //Debug.Log(raycastHit.transform.name);
            if (raycastHit.transform.TryGetComponent(out IDamageable idamageable))
            {
                Damage damage = new Damage(unchargedHitDamage, Damage.DamageType.Eletric, true, weaponControl.transform.position);
                idamageable.TakeDamage(damage);
                ArmadilloUIControl.Instance.StartHitMarker();
                hitPoint = raycastHit.point;
                if (!raycastHit.collider.isTrigger)break;
            }
        }
        visualHandler.OnUnchargedFire(hitPoint);
    }

    #endregion

    //AltFire
    #region
    public override void OnAltFireButtonPerformed(InputAction.CallbackContext performed)
    {

    }
    public override void OnAltFireButtonCanceled(InputAction.CallbackContext performed)
    {

    }
    #endregion

    //Reload
    #region
    public override void OnReloadButtonPerformed(InputAction.CallbackContext performed)
    {

    }
    public override void OnReloadButtonCanceled(InputAction.CallbackContext performed)
    {

    }
    #endregion

    //Reset
    #region
    public override void ResetGun()
    {
        visualHandler.ResetVisuals();
        isOnCooldown = false;
        fireCooldown_Ref = null;
        ArmadilloPlayerController.Instance.visualControl.ToggleEletricPistolCharge(false);
    }
    #endregion

    //Run
    #region Run
    public override void ToggleOnRun(bool state)
    {

    }
    #endregion

    //Old Pistol
    #region OldPistol

    #endregion
}
