using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using static WeaponData;

public abstract class Weapon
{
    public WeaponType weaponType;
    public string name;

    public string description;

    public enum AmmoType
    {
        Ammo = 0,
        Overheat =1
    }

    public AmmoType ammoType;

    public int currentAmmoAmount;
    public int maxAmmoAmount;

    public int ammoReserveAmount;
    public int maxAmmoReserveAmount;

    public Slider ammoSlider;

    //Fire
    public abstract void OnFireButtonPerformed(InputAction.CallbackContext performed);
    public abstract void OnFireButtonCanceled(InputAction.CallbackContext performed);

    //Alt Fire
    public abstract void OnAltFireButtonPerformed(InputAction.CallbackContext performed);
    public abstract void OnAltFireButtonCanceled(InputAction.CallbackContext performed);

    //Reload
    public abstract void OnReloadButtonPerformed(InputAction.CallbackContext performed);
    public abstract void OnReloadButtonCanceled(InputAction.CallbackContext performed);

    //Visual
    public abstract void ToggleVisual(bool state);

    public abstract void OnEquip(bool playAnimation);

    public abstract void OnUnequip();

    public abstract void ResetGun();

    public abstract void ToggleOnRun(bool state);
}
