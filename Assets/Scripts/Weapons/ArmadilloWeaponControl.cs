using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using static Damage;
using static WeaponData;

public class WeaponData
{
    public enum WeaponType
    {
        Zapgun,
        Watergun,
        Pendrivegun
    }
}
public class ArmadilloWeaponControl : MonoBehaviour
{
    private bool isGunPocketed;

    [SerializeField] private bool startWithMelee;
    [SerializeField] private bool startWithPistol;
    [SerializeField] private bool startWithWaterGun;
    [SerializeField] private bool startWithPendriveSniper;

    [System.NonSerialized] public Weapon[] weaponsInInventory;

    [SerializeField] public int currentWeaponID = -1;
    [SerializeField] private int pocketedWeaponID = -1;

    [SerializeField] private Camera weaponCamera;

    public TextMeshProUGUI currentWeaponUI;
    public Slider Weaponammoslider;

    [Header("Weapons")]
    [SerializeField] public Transform eletricPistolSpawnPoint;

    EletricPistol eletricPistol;
    WaterGun waterGun;
    PendriveSniper pendriveSniper;

    private void Awake()
    {
        weakMeleeSpeedMultipler = new SpeedMultipler { value = 0.75f };
        HeavyMeleeSpeedMultipler = new SpeedMultipler { value = 0.75f };
    }

    public void Start()
    {
        LoadWeapons();
        weaponsInInventory = new Weapon[3];
        if (startWithMelee) GivePlayerMelee();
        if (startWithWaterGun) GivePlayerWaterGun();
        if (startWithPendriveSniper) GivePlayerPendriveSniper();
        if (startWithPistol) GivePlayerEletricPistol();
    }
    //----- Melee Functions -----
    #region Melee
    [Header("Melee")]
    [SerializeField] private float weakMeleeDamage;
    [SerializeField] private float strongMeleeDamage;
    [SerializeField] private float meleeCooldown;
    private bool meleeIsOnCooldown;
    [SerializeField] private MeleeHitbox meleeHitbox;

    private SpeedMultipler weakMeleeSpeedMultipler;
    private SpeedMultipler HeavyMeleeSpeedMultipler;

    [SerializeField] private float heavyMeleeHoldDuration;

    private float meleeHoldDuration;
    public void Melee(InputAction.CallbackContext performed)
    {
        if (meleeIsOnCooldown || !ArmadilloPlayerController.Instance.visualControl.isArmVisible) return;
        if(pressOrHoldHit_Ref == null && hit_Ref ==null)
        {
            ToggleArms(false);
            ToggleWeapon(false, false);
            pressOrHoldHit_Ref = StartCoroutine(PressOrHoldHit_Coroutine());
        }
    }
    public void MeleeCancel(InputAction.CallbackContext performed)
    {
        if (pressOrHoldHit_Ref != null)
        {
            StopCoroutine(pressOrHoldHit_Ref);
            if (meleeHoldDuration < heavyMeleeHoldDuration)
            {
                hit_Ref = StartCoroutine(HitWeak_Coroutine(weakMeleeDamage));
            }
            else
            {
                hit_Ref = StartCoroutine(HitStrong_Coroutine(strongMeleeDamage));
            }
            pressOrHoldHit_Ref = null;
        }
    }

    private Coroutine pressOrHoldHit_Ref;
    private IEnumerator PressOrHoldHit_Coroutine()
    {
        meleeHoldDuration = 0;
        while(meleeHoldDuration<heavyMeleeHoldDuration)
        {
            meleeHoldDuration += Time.deltaTime;
            yield return null;
        }
        hit_Ref = StartCoroutine(HitStrong_Coroutine(strongMeleeDamage));
        pressOrHoldHit_Ref = null;
    }

    private Coroutine hit_Ref;
    public IEnumerator HitWeak_Coroutine(float meleeDamage)
    {
        meleeHitbox.CheckListIntegrity();
        //Do damage for all entities
        MeleeAnimator.Instance.PlayRandomMeleeAnimation();
        ArmadilloPlayerController.Instance.movementControl.speedMultiplerList.Add(weakMeleeSpeedMultipler);
        yield return new WaitForSeconds(0.2f);
        for (int i = 0; i < meleeHitbox.damageablesInHitbox.Count; ++i)
        {
            IDamageable damageable = meleeHitbox.damageablesInHitbox.ElementAt(i).Value;
            damageable.TakeDamage(new Damage(meleeDamage, DamageType.Slash, true, ArmadilloPlayerController.Instance.transform.position));
        }
        yield return new WaitForSeconds(0.469f - 0.2f);
        ArmadilloPlayerController.Instance.movementControl.speedMultiplerList.Remove(weakMeleeSpeedMultipler);
        ToggleWeapon(true, false);
        ToggleArms(true);
        yield return new WaitForSeconds(meleeCooldown);
        hit_Ref = null;
    }
    public IEnumerator HitStrong_Coroutine(float meleeDamage)
    {
        meleeHitbox.CheckListIntegrity();
        //Do damage for all entities
        MeleeAnimator.Instance.PlayTakedownAnimation();
        ArmadilloPlayerController.Instance.movementControl.speedMultiplerList.Add(HeavyMeleeSpeedMultipler);
        yield return new WaitForSeconds(0.42f);
        for (int i = 0; i < meleeHitbox.damageablesInHitbox.Count; ++i)
        {
            IDamageable damageable = meleeHitbox.damageablesInHitbox.ElementAt(i).Value;
            damageable.TakeDamage(new Damage(meleeDamage, DamageType.Slash, true, ArmadilloPlayerController.Instance.transform.position));
        }
        yield return new WaitForSeconds(1.406f - 0.42f);
        ArmadilloPlayerController.Instance.movementControl.speedMultiplerList.Remove(HeavyMeleeSpeedMultipler);
        ToggleWeapon(true, false);
        ToggleArms(true);
        yield return new WaitForSeconds(meleeCooldown);
        hit_Ref = null;
    }
    #endregion

    //----- Inputs Functions -----
    #region Inputs
    public void EquipSlot0Weapon(InputAction.CallbackContext performed)
    {
        if (!isGunPocketed && ArmadilloPlayerController.Instance.canSwitchWeapon)
        {
            ChangeWeapon(0, false);
            ChangeWeaponUI.Instance.SelectItem(0);
        }
    }
    public void EquipSlot1Weapon(InputAction.CallbackContext performed)
    {
        if (!isGunPocketed && ArmadilloPlayerController.Instance.canSwitchWeapon)
        {
            ChangeWeapon(1, false);
            ChangeWeaponUI.Instance.SelectItem(1);
        }
    }
    public void EquipSlot2Weapon(InputAction.CallbackContext performed)
    {
        if (!isGunPocketed && ArmadilloPlayerController.Instance.canSwitchWeapon)
        {
            ChangeWeapon(2, false);
            ChangeWeaponUI.Instance.SelectItem(2);
        }
    }
    #endregion

    private void LoadWeapons()
    {
        eletricPistol = new EletricPistol(this);
        waterGun = new WaterGun(this);
        pendriveSniper = new PendriveSniper(this);
    }
    //----- Give Player Functions -----
    #region Give Player Weapons 
    public void GivePlayerMelee()
    {
        PlayerInputAction inputActions = ArmadilloPlayerController.Instance.inputControl.inputAction;
        inputActions.Armadillo.Melee.Enable();
        inputActions.Armadillo.Melee.performed += Melee;
        inputActions.Armadillo.Melee.canceled += MeleeCancel;
    }
    public void GivePlayerEletricPistol()
    {
        weaponsInInventory[0] = eletricPistol;
        if (isGunPocketed)
        {
            pocketedWeaponID = 0;
        }
        else
        {
            ChangeWeapon(0, true);
            ChangeWeaponUI.Instance.SelectItem(0);
        }
        ArmadilloPlayerController.Instance.inputControl.inputAction.Armadillo.Weapon0.Enable();
        ArmadilloPlayerController.Instance.inputControl.inputAction.Armadillo.Weapon0.performed += EquipSlot0Weapon;
    }
    public void GivePlayerWaterGun()
    {
        weaponsInInventory[1] = waterGun;
        if (isGunPocketed)
        {
            pocketedWeaponID = 1;
        }
        else
        {
            ChangeWeapon(1, true);
            ChangeWeaponUI.Instance.SelectItem(1);
        }
        ArmadilloPlayerController.Instance.inputControl.inputAction.Armadillo.Weapon1.Enable();
        ArmadilloPlayerController.Instance.inputControl.inputAction.Armadillo.Weapon1.performed += EquipSlot1Weapon;
    }
    public void GivePlayerPendriveSniper()
    {
        weaponsInInventory[2] = pendriveSniper;
        if (isGunPocketed)
        {
            pocketedWeaponID = 2;
        }
        else
        {
            ChangeWeapon(2, true);
            ChangeWeaponUI.Instance.SelectItem(2);
        }
        ArmadilloPlayerController.Instance.inputControl.inputAction.Armadillo.Weapon2.Enable();
        ArmadilloPlayerController.Instance.inputControl.inputAction.Armadillo.Weapon2.performed += EquipSlot2Weapon;
    }
    #endregion

    //----- Delegate Inputs Functions -----
    #region Delegate Input Functions
    private enum DelegateType
    {
        Add,
        Remove,
    }
    private Coroutine DelegateDelay_Ref;
    private IEnumerator DelegateDelay_Coroutine(float delay, PlayerInputAction.ArmadilloActions playerInput, DelegateType delegateType, int weaponID)
    {
        yield return new WaitForSeconds(delay);
        DefineDelegates(playerInput, delegateType, weaponID);
    }
    private void DefineDelegates(PlayerInputAction.ArmadilloActions playerInput, DelegateType delegateType, int weaponID)
    {
        switch (delegateType)
        {
            case DelegateType.Add:
                playerInput.Fire.performed += weaponsInInventory[weaponID].OnFireButtonPerformed;
                playerInput.Fire.canceled += weaponsInInventory[weaponID].OnFireButtonCanceled;

                playerInput.AltFire.performed += weaponsInInventory[weaponID].OnAltFireButtonPerformed;
                playerInput.AltFire.canceled += weaponsInInventory[weaponID].OnAltFireButtonCanceled;

                playerInput.Reload.performed += weaponsInInventory[weaponID].OnReloadButtonPerformed;
                playerInput.Reload.canceled += weaponsInInventory[weaponID].OnReloadButtonCanceled;
                break;
            case DelegateType.Remove:
                playerInput.Fire.performed -= weaponsInInventory[weaponID].OnFireButtonPerformed;
                playerInput.Fire.canceled -= weaponsInInventory[weaponID].OnFireButtonCanceled;

                playerInput.AltFire.performed -= weaponsInInventory[weaponID].OnAltFireButtonPerformed;
                playerInput.AltFire.canceled -= weaponsInInventory[weaponID].OnAltFireButtonCanceled;

                playerInput.Reload.performed -= weaponsInInventory[weaponID].OnReloadButtonPerformed;
                playerInput.Reload.canceled -= weaponsInInventory[weaponID].OnReloadButtonCanceled;
                break;
        }
    }
    private void ToggleStateInputs(PlayerInputAction.ArmadilloActions playerInput, bool state)
    {
        if (state)
        {
            playerInput.Fire.Enable();
            playerInput.AltFire.Enable();
            playerInput.Reload.Enable();
            return;
        }
        playerInput.Fire.Disable();
        playerInput.AltFire.Disable();
        playerInput.Reload.Disable();
    }
    #endregion

    //----- Toggle Weapons & Arms Functions-----
    #region Toggle Weapon & Arms Functions
    public void ToggleWeapon(bool state, bool playAnimation)
    {
        if (!state)
        {
            if (currentWeaponID != -1)
            {
                isGunPocketed = true;
                pocketedWeaponID = currentWeaponID;
            }
            ChangeWeapon(-1, playAnimation);
        }
        else
        {
            if (pocketedWeaponID != -1)
            {
                ChangeWeapon(pocketedWeaponID, playAnimation);
                isGunPocketed = false;
                pocketedWeaponID = -1;
            }
        }
    }
    public void ToggleArms(bool state)
    {
        ArmadilloPlayerController.Instance.visualControl.ToggleArmView(state);
        if (!state) ResetAllCurrentWeapons();
    }
    #endregion

    public void ChangeWeapon(int nextWeapon, bool playAnimation)
    {
        PlayerInputAction.ArmadilloActions playerInput = ArmadilloPlayerController.Instance.inputControl.inputAction.Armadillo;
        if (nextWeapon < 0 || nextWeapon > weaponsInInventory.Length)
        {
            //Unequip current gun
            if (currentWeaponID == -1) return;
            DefineDelegates(playerInput, DelegateType.Remove, currentWeaponID);
            ToggleStateInputs(playerInput, false);
            if (playAnimation)
            {
                weaponsInInventory[currentWeaponID].OnUnequip();
            }
            else weaponsInInventory[currentWeaponID].ToggleVisual(false);
            weaponsInInventory[currentWeaponID].ResetGun();
            currentWeaponID = -1;

            return;
        }
        if (currentWeaponID != -1)
        {
            //Remove current weapon
            weaponsInInventory[currentWeaponID].ResetGun();
            DefineDelegates(playerInput, DelegateType.Remove, currentWeaponID);
            weaponsInInventory[currentWeaponID].ToggleVisual(false);
        }
        else
        {
            //Toggle Inputs
            ToggleStateInputs(playerInput, true);
        }
        currentWeaponID = nextWeapon;
        OnGunEquip(weaponsInInventory[nextWeapon].weaponType);
        if (playAnimation)
        {
            weaponsInInventory[nextWeapon].ToggleVisual(true);
            weaponsInInventory[nextWeapon].OnEquip(true);
            if (DelegateDelay_Ref != null) StopCoroutine(DelegateDelay_Ref);
            DelegateDelay_Ref = StartCoroutine(DelegateDelay_Coroutine(1.125f, playerInput, DelegateType.Add, nextWeapon));
        }
        else
        {
            weaponsInInventory[nextWeapon].ToggleVisual(true);
            weaponsInInventory[nextWeapon].OnEquip(false);
            if (DelegateDelay_Ref != null) StopCoroutine(DelegateDelay_Ref);
            DelegateDelay_Ref = StartCoroutine(DelegateDelay_Coroutine(0.2f, playerInput, DelegateType.Add, nextWeapon));
        }

    }
    public void OnGunEquip(WeaponType weaponType)
    {
        ArmadilloPlayerController.Instance.visualControl.OnGunEquiped(weaponType);
    }
    public void ResetAllCurrentWeapons()
    {
        foreach (Weapon weapon in weaponsInInventory)
        {
            if (weapon != null) weapon.ResetGun();
        }
    }

    public void OnRun(bool state)
    {
        if (currentWeaponID != -1 && weaponsInInventory[currentWeaponID] != null)
        {
            weaponsInInventory[currentWeaponID].ToggleOnRun(state);
        }
    }
    public bool GainAmmo(WeaponType weaponType, int ammoAmount)
    {
        Weapon weapon;
        switch (weaponType)
        {
            default:
            case WeaponType.Zapgun:
                weapon = eletricPistol;
                break;
            case WeaponType.Watergun:
                weapon = waterGun;
                break;
            case WeaponType.Pendrivegun:
                weapon = pendriveSniper;
                break;
        }
        int ammoReserves;
        if (weapon.ammoReserveAmount >= weapon.maxAmmoReserveAmount)
        {
            return false;
        }
        else
        {
            ammoReserves = weapon.ammoReserveAmount;
            ammoReserves += ammoAmount;
            ammoReserves = Mathf.Min(ammoReserves, weapon.maxAmmoReserveAmount);
            weapon.ammoReserveAmount = ammoReserves;
            return true;
        }
    }
}
