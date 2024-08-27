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
        currentAmmoAmount = 50;
        maxAmmoAmount = 50;

        ammoReserveAmount = 100;
        maxAmmoReserveAmount = 250;
        name = "Arma de Agua";
        description = "Atire nos inimigos para infringir dano e no chão para criar poças";
        ammoSlider = armadilloWeaponControl.Weaponammoslider;
        weaponType = WeaponType.Watergun;
        LoadVisual();
    }


    //----- Stats -----
    //
    private Transform firePivot;

    //Tiro normal
    private Vector3 bodyVelocity;
    readonly private float fireDelay = 0.33f;
    private bool isOnCooldown;


    //----- Visual -----
    private WaterGunVisual visualHandler;

    //Visual
    #region
    private void LoadVisual()
    {
        visualHandler = WaterGunVisual.Instance;
        firePivot = visualHandler.firePivot;
    }

    public override void ToggleVisual(bool state)
    {
        //throw new NotImplementedException();
    }
    public override void OnEquip(bool playAnimation)
    {
        visualHandler.OnEquip(playAnimation);
        if(UpdateWaterSprayVelocity_Ref == null )
        {
            UpdateWaterSprayVelocity_Ref = weaponControl.StartCoroutine(UpdateWaterSprayVelocity_Coroutine());
        }
        if (playAnimation) ArmadilloPlayerController.Instance.visualControl.EquipWaterGunAnim();
        else ArmadilloPlayerController.Instance.visualControl.EquipWaterGun();


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
        ToggleFire(true);
    }
    public override void OnFireButtonCanceled(InputAction.CallbackContext performed)
    {
        ToggleFire(false);
    }
    public void ToggleFire(bool value)
    {
        if (currentAmmoAmount <= 0)
        {
            Reload();
            return;
        }
        if (value)
        {
            if (fire_Ref == null)
            {
                fire_Ref = weaponControl.StartCoroutine(Fire_Coroutine());
            }
        }
        else
        {
            if (fire_Ref != null)
            {
                weaponControl.StopCoroutine(fire_Ref);
                fire_Ref = null;
            }
        }
        ArmadilloPlayerController.Instance.visualControl.ToggleOnFireWaterGun(value);
        visualHandler.ToggleOnFire(value);
    }
    public Coroutine fire_Ref;
    public IEnumerator Fire_Coroutine()
    {
        while (currentAmmoAmount > 0)
        {
            Fire(firePivot.position, firePivot.forward);
            currentAmmoAmount--;
            yield return new WaitForSeconds(0.25f);
        }
        visualHandler.ToggleOnFire(false);
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
            //visualHandler.UpdateSprayVelocity(velocity);
            this.bodyVelocity = velocity;
            visualHandler.UpdateSprayVelocity(velocity);
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
        ArmadilloPlayerController.Instance.visualControl.ReloadWaterGun();
        visualHandler.OnReload();
        yield return new WaitForSeconds(3.563f + 0.25f);
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
        Debug.Log(currentAmmoAmount + "|" + ammoReserveAmount);
    }
    #endregion

    //Reset
    #region Reset
    public override void ResetGun()
    {
        Debug.Log("Reset");
        visualHandler.ResetVisuals();
    }
    #endregion

    //Run
    #region Run
    public override void ToggleOnRun(bool state)
    {
        visualHandler.ToggleOnRun(state);
    }
    #endregion
}
