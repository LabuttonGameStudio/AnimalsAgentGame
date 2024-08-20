using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
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
    readonly private float fireDelay = 0.33f;
    private bool isOnCooldown;

    
    //----- Visual -----
    private WaterGunVisual visualHandler;

    //Visual
    #region
    private void LoadVisual()
    {
        
    }

    public override void ToggleVisual(bool state)
    {
        throw new NotImplementedException();
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
        ToggleFire(true);
    }
    public override void OnFireButtonCanceled(InputAction.CallbackContext performed)
    {
        ToggleFire(false);
    }
    public void ToggleFire(bool value)
    {
        if(value)
        {
            if(fire_Ref == null)
            {
                fire_Ref = weaponControl.StartCoroutine(FireCooldown_Coroutine());
            }
        }
        else
        {
            if(fire_Ref != null)
            {
                weaponControl.StopCoroutine(fire_Ref);
            }
        }
    }
    public Coroutine fire_Ref;
    public IEnumerator Fire_Coroutine()
    {
        while(currentAmmoAmount>0)
        {
            Fire(firePivot.position, firePivot.forward);
            currentAmmoAmount--;
            yield return new WaitForSeconds(0.25f);
        }
    }
    //Transfer to Weapon Script
    public void Fire(Vector3 position, Vector3 direction)
    {
        WaterGunProjectileManager manager = WaterGunProjectileManager.Instance;
        WaterGunProjectile waterGunProjectile = manager.watergunProjectilePool[0];
        manager.watergunProjectilePool.RemoveAt(0);
        manager.watergunProjectileInGame.Add(waterGunProjectile);
        waterGunProjectile.transform.position = position;
        waterGunProjectile.duration = 1;
        waterGunProjectile.gameObject.SetActive(true);
        waterGunProjectile.rb.velocity = direction * manager.velocity;
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
    public override void ResetGun()
    {
        Debug.Log("Reset");
        visualHandler.ResetVisuals();
    }
}
