using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static WeaponData;

public class PendriveSniper : Weapon
{
    public PendriveSniper(ArmadilloWeaponControl armadilloWeaponControl)
    {
        weaponControl = armadilloWeaponControl;
        ammoType = AmmoType.Ammo;
        currentAmmoAmount = 3;
        maxAmmoAmount = 3;

        ammoReserveAmount = 6;
        maxAmmoReserveAmount = 6;
        name = "Zarabatana de Pendrive";
        description = "Atire nos inimigos para infiltrar o sistema dos robos";
        ammoSlider = armadilloWeaponControl.Weaponammoslider;
        weaponType = WeaponType.Pendrivegun;
        LoadVisual();
    }

    //----- Components -----
    private ArmadilloWeaponControl weaponControl;
    private PendriveSniperVisualHandler visualHandler;

    //----- Fire Projectile Variables -----
    private Transform firePivot;

    //-----Fire Variables -----
    private float bulletDamage = 50;
    readonly private float fireDelay = 1f;
    private bool isOnCooldown;

    //----- Scope -----
    private bool isScoped;


    //Visual
    #region
    private void LoadVisual()
    {
        visualHandler = PendriveSniperVisualHandler.Instance;
        visualHandler.ToggleVisual(false);
        firePivot = visualHandler.firePivot;
    }

    public override void ToggleVisual(bool state)
    {
        visualHandler.ToggleVisual(state);
    }
    public override void OnEquip(bool playAnimation)
    {
        visualHandler.OnEquip(playAnimation);
        //if (playAnimation) ArmadilloPlayerController.Instance.visualControl.EquipWaterGunAnim();
        //else ArmadilloPlayerController.Instance.visualControl.EquipWaterGun();


    }
    public override void OnUnequip()
    {
        //ArmadilloPlayerController.Instance.visualControl.UnequipEletricPistol();
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
            isOnCooldown = true;
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
        if (!isOnCooldown)
        {
            if (currentAmmoAmount <= 0)
            {
                Reload();
            }
            else
            {
                Fire(firePivot.position, firePivot.transform.up, isScoped);
                StartFireCooldownTimer();
            }
        }
    }
    public override void OnFireButtonCanceled(InputAction.CallbackContext performed)
    {

    }
    public void Fire(Vector3 position, Vector3 direction, bool scopeIn)
    {
        PendriveSniperProjectileManager manager = PendriveSniperProjectileManager.Instance;
        PendriveSniperProjectile pendriveProjectile = manager.pendriveSniperProjectilePool[0];
        manager.pendriveSniperProjectilePool.RemoveAt(0);
        manager.pendriveSniperProjectileInGame.Add(pendriveProjectile);
        pendriveProjectile.transform.position = position;
        pendriveProjectile.duration = 3f;
        pendriveProjectile.bulletDamage = bulletDamage;
        pendriveProjectile.playerPos = weaponControl.transform.position;
        pendriveProjectile.gameObject.SetActive(true);
        pendriveProjectile.rb.velocity = direction * manager.velocity;
        currentAmmoAmount -= 1;

    }
    #endregion

    //AltFire
    #region
    public override void OnAltFireButtonPerformed(InputAction.CallbackContext performed)
    {
        isScoped = true;
        visualHandler.ToggleScope(true);
    }
    public override void OnAltFireButtonCanceled(InputAction.CallbackContext performed)
    {
        isScoped = false;
        visualHandler.ToggleScope(false);
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
        //ArmadilloPlayerController.Instance.visualControl.ReloadWaterGun();
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
        visualHandler.ResetVisuals();
        isScoped = false;
        visualHandler.ToggleScope(false);
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
