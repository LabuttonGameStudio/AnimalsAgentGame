using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;
using static WeaponData;

public class EletricPistol : Weapon
{
    private ArmadilloWeaponControl weaponControl;
    public EletricPistol(ArmadilloWeaponControl armadilloWeaponControl)
    {
        weaponControl = armadilloWeaponControl;
        ammoType = AmmoType.Ammo;

        currentAmmoAmount = 12;
        maxAmmoAmount = 12;

        ammoReserveAmount = 24;
        maxAmmoReserveAmount = 72;
        name = "Pistola Eletrica";
        description = "Pressione pra atirar, segure para carregar ";
        ammoSlider = armadilloWeaponControl.Weaponammoslider;
        weaponType = WeaponType.Zapgun;
        LoadVisual();

        onFireSlow = new SpeedMultipler { value = 0.75f };
        onReloadSlow = new SpeedMultipler { value = 0.75f };
    }

    //----- Stats -----
    //Fire
    readonly private float fireDelay = 0.33f;
    private bool isOnCooldown;

    readonly private int unchargedHitDamage = 15;
    readonly private int minUnchargedHitDamage = 5;

    readonly private float damageFallOffStartRange = 5;
    readonly private float damageFallOffEndRange = 20;

    //Reload
    readonly private float reloadDuration = 1.75f;

    //----- Visual -----
    private EletricPistolVisual visualHandler;

    private bool isReloading;

    //-----Slowness -----
    private SpeedMultipler onFireSlow;

    private SpeedMultipler onReloadSlow;

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
        visualHandler.UpdateUI(currentAmmoAmount, maxAmmoAmount, ammoReserveAmount,maxAmmoReserveAmount);

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
        return (fireCooldown_Ref != null || isReloading);
    }
    public Coroutine fireCooldown_Ref;
    public IEnumerator FireCooldown_Coroutine()
    {
        ArmadilloMovementController movementControl = ArmadilloPlayerController.Instance.movementControl;
        movementControl.speedMultiplerList.Add(onFireSlow);
        movementControl.isFiring = true;
        movementControl.isSprintButtonHeld = false;
        movementControl.CancelSprint();
        yield return new WaitForSeconds(fireDelay);
        movementControl.isFiring = false;
        ArmadilloPlayerController.Instance.movementControl.speedMultiplerList.Remove(onFireSlow);
        isOnCooldown = false;
        fireCooldown_Ref = null;
    }

    public override void OnFireButtonPerformed(InputAction.CallbackContext performed)
    {
        if (!IsFireOnCooldown()) Fire();
    }
    public override void OnFireButtonCanceled(InputAction.CallbackContext performed)
    {

    }

    private bool AmmoCheck()
    {
        return currentAmmoAmount > 0;
    }

    public void Fire()
    {
        if (AmmoCheck())
        {
            FPCameraShake.StartShake(0.15f, 0.4f, 5f);
            FireRaycast();
            ArmadilloPlayerController.Instance.visualControl.OnEletricGunFire();
            currentAmmoAmount -= 1;
            currentAmmoAmount = Mathf.Max(currentAmmoAmount, 0);
            visualHandler.UpdateUI(currentAmmoAmount, maxAmmoAmount, ammoReserveAmount, maxAmmoReserveAmount);
            StartFireCooldownTimer();
        }
        else
        {
            Reload();
        }
    }
    public void FireRaycast()
    {
        RaycastHit[] targetsHit;
        Transform camera = ArmadilloPlayerController.Instance.cameraControl.mainCamera.transform;
        targetsHit = Physics.RaycastAll(camera.position, camera.forward, 20f, LayerManager.Instance.activeColliders, QueryTriggerInteraction.Collide);
        targetsHit = targetsHit.OrderBy(x => x.distance).ToArray();
        Vector3 hitPoint = camera.position + camera.forward * 20f;
        bool hitSomething = false;
        bool hitEnemy = false;
        foreach (RaycastHit raycastHit in targetsHit)
        {
            //Debug.Log(raycastHit.transform.name);
            if (raycastHit.collider.TryGetComponent(out IDamageable idamageable))
            {
                Damage damage = new Damage(unchargedHitDamage, Damage.DamageType.Eletric, true, weaponControl.transform.position);
                if (raycastHit.distance > damageFallOffStartRange)
                {
                    damage.damageAmount = Mathf.Lerp(damage.damageAmount, minUnchargedHitDamage, (raycastHit.distance - damageFallOffStartRange) / (damageFallOffEndRange - damageFallOffStartRange));
                    idamageable.TakeDamage(damage);
                }
                else idamageable.TakeDamage(damage);
                if(idamageable.isDead())
                {
                    visualHandler.OnLetalShot();
                }
                else
                {
                    if (damage.wasCritical)
                    {
                        visualHandler.OnHeadshotShot();
                    }
                    else
                    {
                        visualHandler.OnBodyShot();
                    }
                }
                if (!raycastHit.collider.isTrigger)
                {
                    hitEnemy = true;
                    hitSomething = true;
                    hitPoint = raycastHit.point;
                    break;
                }
            }
            else
            {
                if (!raycastHit.collider.isTrigger)
                {
                    hitSomething = true;
                    hitPoint = raycastHit.point;
                    break;
                }
            }
        }
        visualHandler.OnUnchargedFire(hitPoint, camera.forward, hitSomething, !hitEnemy);
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
        Reload();
    }
    public override void OnReloadButtonCanceled(InputAction.CallbackContext performed)
    {

    }

    private void Reload()
    {
        if (reload_Ref == null && !isReloading && currentAmmoAmount < maxAmmoAmount && ammoReserveAmount > 0)
        {
            isReloading = true;
            reload_Ref = weaponControl.StartCoroutine(Reload_Coroutine());
        }
    }
    private Coroutine reload_Ref;
    private IEnumerator Reload_Coroutine()
    {
        ArmadilloPlayerController.Instance.movementControl.speedMultiplerList.Add(onReloadSlow);
        ArmadilloPlayerController.Instance.visualControl.ToggleEletricPistolOverheat(true);
        visualHandler.ToggleReload(true);
        yield return new WaitForSeconds(reloadDuration);
        ArmadilloPlayerController.Instance.movementControl.speedMultiplerList.Remove(onReloadSlow);
        ArmadilloPlayerController.Instance.visualControl.ToggleEletricPistolOverheat(false);
        visualHandler.ToggleReload(false);
        ReplenishAmmo();
        visualHandler.UpdateUI(currentAmmoAmount, maxAmmoAmount, ammoReserveAmount, maxAmmoReserveAmount);
        isReloading = false;
        reload_Ref = null;
    }

    private void ReplenishAmmo()
    {
        if (currentAmmoAmount + ammoReserveAmount >= maxAmmoAmount)
        {
            ammoReserveAmount -= maxAmmoAmount - currentAmmoAmount;
            currentAmmoAmount = maxAmmoAmount;
        }
        else
        {
            currentAmmoAmount += ammoReserveAmount;
            ammoReserveAmount = 0;
        }
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
        ArmadilloPlayerController.Instance.movementControl.isFiring = false;
        ArmadilloPlayerController.Instance.movementControl.SprintCheck();

        if (reload_Ref != null)
        {
            weaponControl.StopCoroutine(reload_Ref);
            ArmadilloPlayerController.Instance.movementControl.speedMultiplerList.Remove(onReloadSlow);
            ArmadilloPlayerController.Instance.visualControl.ToggleEletricPistolOverheat(false);
            visualHandler.ToggleReload(false);
            isReloading = false;
            reload_Ref = null;
        }
    }
    #endregion

    //Run
    #region Run
    public override void ToggleOnRun(bool state)
    {

    }
    #endregion

    //OnGainAmmo
    #region
    public void OnGainAmmo()
    {
        visualHandler.UpdateUI(currentAmmoAmount, maxAmmoAmount, ammoReserveAmount, maxAmmoReserveAmount);
    }
    #endregion
}
