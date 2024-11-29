using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI;
using static WeaponData;
public class WaterGun : Weapon
{
    private ArmadilloWeaponControl weaponControl;
    public WaterGun(ArmadilloWeaponControl armadilloWeaponControl)
    {
        weaponControl = armadilloWeaponControl;
        ammoType = AmmoType.Ammo;
        currentAmmoAmount = 20;
        maxAmmoAmount = 20;

        ammoReserveAmount = 80;
        maxAmmoReserveAmount = 120;
        name = "Arma de Agua";
        description = "Atire nos inimigos para infringir dano e no chão para criar poças";
        ammoSlider = armadilloWeaponControl.Weaponammoslider;
        weaponType = WeaponType.Watergun;

        onFireSlow = new SpeedMultipler { value = 0.5f };
        onReloadSlow = new SpeedMultipler { value = 0.75f };
    }


    //----- Stats -----
    //

    //Tiro normal
    private float bulletDamage = 15;
    private float minbulletDamage = 7.5f;
    private Vector3 bodyVelocity;
    readonly private float fireDelay = 0.33f;
    private bool isOnCooldown;


    //----- Visual -----

    //-----Slowness -----
    private SpeedMultipler onFireSlow;

    private SpeedMultipler onReloadSlow;
    //Visual
    #region

    public override void ToggleVisual(bool state)
    {
        WaterGunVisual.Instance.ToggleVisual(state);
    }
    public override void OnEquip(bool playAnimation)
    {
        WaterGunVisual.Instance.OnEquip(playAnimation);
        if(UpdateWaterSprayVelocity_Ref == null )
        {
            UpdateWaterSprayVelocity_Ref = weaponControl.StartCoroutine(UpdateWaterSprayVelocity_Coroutine());
        }
        if (playAnimation) ArmadilloPlayerController.Instance.visualControl.EquipWaterGunAnim();
        else ArmadilloPlayerController.Instance.visualControl.EquipWaterGun();
        WaterGunVisual.Instance.UpdateUI(currentAmmoAmount, maxAmmoAmount, ammoReserveAmount, maxAmmoReserveAmount);


    }
    public override void OnUnequip()
    {
        if (UpdateWaterSprayVelocity_Ref != null)
        {
            weaponControl.StopCoroutine(UpdateWaterSprayVelocity_Ref);
        }
        ArmadilloPlayerController.Instance.visualControl.UnequipEletricPistol();
        weaponControl.StartCoroutine(OnUnequipAnimEnd());
    }
    public IEnumerator OnUnequipAnimEnd()
    {
        yield return new WaitForSeconds(0.16f);
        WaterGunVisual.Instance.ToggleVisual(false);
    }
    #endregion

    //Fire
    #region
    public override void OnFireButtonPerformed(InputAction.CallbackContext performed)
    {
        if(!isOnCooldown)ToggleFire(true);
    }
    public override void OnFireButtonCanceled(InputAction.CallbackContext performed)
    {
        ToggleFire(false);
    }
    public void ToggleFire(bool value)
    {
        if (currentAmmoAmount <= 0 && value)
        {
            Reload();
            return;
        }
        if (value)
        {
            if (fire_Ref == null)
            {
                ArmadilloMovementController movementControl = ArmadilloPlayerController.Instance.movementControl;
                movementControl.speedMultiplerList.Add(onFireSlow);
                movementControl.isFiring = true;
                movementControl.isSprintButtonHeld = false;
                movementControl.CancelSprint();
                fire_Ref = weaponControl.StartCoroutine(Fire_Coroutine());
            }
        }
        else
        {
            if (fire_Ref != null)
            {
                ArmadilloMovementController movementControl = ArmadilloPlayerController.Instance.movementControl;
                movementControl.speedMultiplerList.Remove(onFireSlow);
                movementControl.isFiring = false;
                weaponControl.StopCoroutine(fire_Ref);
                fire_Ref = null;
            }
        }
        ArmadilloPlayerController.Instance.visualControl.ToggleOnFireWaterGun(value);
        WaterGunVisual.Instance.ToggleOnFire(value);
    }
    public Coroutine fire_Ref;
    public IEnumerator Fire_Coroutine()
    {
        while (currentAmmoAmount > 0)
        {
            Fire(WaterGunVisual.Instance.firePivot.position, WaterGunVisual.Instance.firePivot.forward);
            currentAmmoAmount--;
            WaterGunVisual.Instance.UpdateUI(currentAmmoAmount, maxAmmoAmount, ammoReserveAmount, maxAmmoReserveAmount);
            yield return new WaitForSeconds(0.25f);
        }
        ArmadilloMovementController movementControl = ArmadilloPlayerController.Instance.movementControl;
        movementControl.speedMultiplerList.Remove(onFireSlow);
        WaterGunVisual.Instance.ToggleOnFire(false);
        ArmadilloPlayerController.Instance.visualControl.ToggleOnFireWaterGun(false);
        fire_Ref = null;
    }
    public void Fire(Vector3 position, Vector3 direction)
    {
        WaterGunProjectileManager manager = WaterGunProjectileManager.Instance;
        WaterGunProjectile waterGunProjectile = manager.watergunProjectilePool[0];
        manager.watergunProjectilePool.RemoveAt(0);
        manager.watergunProjectileInGame.Add(waterGunProjectile);
        waterGunProjectile.transform.position = position;
        waterGunProjectile.duration = 1.25f;
        waterGunProjectile.bulletDamage = bulletDamage;
        waterGunProjectile.minbulletDamage = minbulletDamage;
        waterGunProjectile.playerPos = weaponControl.transform.position;
        waterGunProjectile.gameObject.SetActive(true);
        waterGunProjectile.rb.velocity = direction * manager.velocity + bodyVelocity;
    }

    Coroutine UpdateWaterSprayVelocity_Ref;
    IEnumerator UpdateWaterSprayVelocity_Coroutine()
    {
        Rigidbody rb = ArmadilloPlayerController.Instance.movementControl.rb;
        while (true)
        {
            Vector3 velocity = rb.velocity / 2;
            velocity.y = velocity.y/2;
            bodyVelocity = velocity;
            WaterGunVisual.Instance.UpdateSprayVelocity(velocity);
            yield return new WaitForSeconds(0.05f);
        }
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
    public void Reload()
    {
        if (ammoReserveAmount > 0 && currentAmmoAmount < maxAmmoAmount)
        {
            if (reloadTimer_Ref == null)
            {
                reloadTimer_Ref = weaponControl.StartCoroutine(ReloadTimer_Coroutine());
            }
        }
    }
    public Coroutine reloadTimer_Ref;
    public IEnumerator ReloadTimer_Coroutine()
    {
        ToggleFire(false);
        isOnCooldown = true;
        //ArmadilloPlayerController.Instance.movementControl.speedMultiplerList.Add(onReloadSlow);
        ArmadilloPlayerController.Instance.visualControl.ReloadWaterGun();
        WaterGunVisual.Instance.OnReload();
        yield return new WaitForSeconds(3.563f + 0.25f);
        //ArmadilloPlayerController.Instance.movementControl.speedMultiplerList.Remove(onReloadSlow);
        if (ammoReserveAmount >= maxAmmoAmount)
        {
            int ammoDif = maxAmmoAmount - currentAmmoAmount;
            currentAmmoAmount = maxAmmoAmount;
            ammoReserveAmount -= ammoDif;
            if (ammoReserveAmount < 0) ammoReserveAmount = 0;
        }
        else
        {
            currentAmmoAmount = ammoReserveAmount;
            ammoReserveAmount = 0;
        }
        reloadTimer_Ref = null;
        isOnCooldown = false;
        WaterGunVisual.Instance.UpdateUI(currentAmmoAmount, maxAmmoAmount, ammoReserveAmount, maxAmmoReserveAmount);
    }
    #endregion

    //Reset
    #region Reset
    public override void ResetGun()
    {
        ToggleFire(false);
        WaterGunVisual.Instance.ResetVisuals();
        isOnCooldown = false;
        ToggleOnRun(false);
        ArmadilloPlayerController.Instance.movementControl.isFiring = false;
        ArmadilloPlayerController.Instance.movementControl.SprintCheck();
        if (reloadTimer_Ref != null)
        {
            ArmadilloPlayerController.Instance.movementControl.speedMultiplerList.Remove(onReloadSlow);
            weaponControl.StopCoroutine(reloadTimer_Ref);
            reloadTimer_Ref = null;
        }
    }
    #endregion

    //Run
    #region Run
    public override void ToggleOnRun(bool state)
    {
        WaterGunVisual.Instance.ToggleOnRun(state);
    }
    #endregion

    //OnGainAmmo
    #region OnGainAmmo
    public void OnGainAmmo()
    {
        WaterGunVisual.Instance.UpdateUI(currentAmmoAmount, maxAmmoAmount, ammoReserveAmount, maxAmmoReserveAmount);
    }
    #endregion
}
