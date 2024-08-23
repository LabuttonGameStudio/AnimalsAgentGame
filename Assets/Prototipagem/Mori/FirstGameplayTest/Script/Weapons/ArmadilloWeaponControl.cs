using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
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

    [System.NonSerialized] public Weapon[] weaponsInInventory;

    [SerializeField] public int currentWeaponID = -1;
    [SerializeField] private int pocketedWeaponID = -1;

    [SerializeField] private Camera weaponCamera;

    public TextMeshProUGUI currentWeaponUI;
    public Slider Weaponammoslider;

    [Header("Weapons")]
    [SerializeField] public Transform eletricPistolSpawnPoint;

    public void Start()
    {
        weaponsInInventory = new Weapon[2];
        if (startWithPistol) GivePlayerEletricPistol();
        if(startWithWaterGun) GivePlayerWaterGun();
        if (startWithMelee) GivePlayerMelee();
    }
    private void Update()
    {

    }
    #region Melee
    [Header("Melee")]
    [SerializeField] private float meleeDamage;
    [SerializeField] private float meleeStabModifier;
    [SerializeField] private float meleeCooldown;
    [SerializeField] private bool meleeIsOnCooldown;
    [SerializeField] private MeleeHitbox meleeHitbox;
    public void GivePlayerMelee()
    {
        PlayerInputAction inputActions = ArmadilloPlayerController.Instance.inputControl.inputAction;
        inputActions.Armadillo.Melee.Enable();
        inputActions.Armadillo.Melee.performed += Melee;
    }
    public void Melee(InputAction.CallbackContext performed)
    {
        if (meleeIsOnCooldown) return;
        Debug.Log("Melee");
        meleeHitbox.Hit(meleeDamage, meleeStabModifier);
        StartMeleeColldownTimer();
    }
    private void StartMeleeColldownTimer()
    {
        if (meleeCooldownTimer_Ref != null) return;
        meleeCooldownTimer_Ref = StartCoroutine(MeleeCooldownTimer_Coroutine());
    }

    private Coroutine meleeCooldownTimer_Ref;
    private IEnumerator MeleeCooldownTimer_Coroutine()
    {
        meleeIsOnCooldown = true;
        yield return new WaitForSeconds(meleeCooldown);
        meleeIsOnCooldown = false;
        meleeCooldownTimer_Ref = null;
    }
    #endregion
    public void GivePlayerEletricPistol()
    {
        weaponsInInventory[0] = new EletricPistol(this);
        if (isGunPocketed)
        {
            Debug.Log("a");
            pocketedWeaponID = 0;
        }
        else ChangeWeapon(0,true);
    }
    public void GivePlayerWaterGun()
    {
        weaponsInInventory[1] = new WaterGun(this);
         ChangeWeapon(1, true);
    }
    public GameObject LoadModel(GameObject model, Vector3 position, Quaternion rotation)
    {
        GameObject objectInstantiated = Instantiate(model, weaponCamera.transform);
        objectInstantiated.transform.localPosition = position;
        objectInstantiated.transform.localRotation = rotation;
        return objectInstantiated;
    }
    public GameObject LoadModel(GameObject model, Transform transform)
    {
        GameObject objectInstantiated = Instantiate(model, weaponCamera.transform);
        objectInstantiated.transform.position = transform.position;
        objectInstantiated.transform.rotation = transform.rotation;
        objectInstantiated.transform.parent = transform.parent;
        return objectInstantiated;
    }
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

            currentWeaponID = -1;
            return;
        }
        if (currentWeaponID != -1)
        {
            //Remove current weapon
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

    public void ToggleWeapon(bool state,bool playAnimation)
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
            ChangeWeapon(pocketedWeaponID, playAnimation);
            if(pocketedWeaponID != -1)
            {
                isGunPocketed = false;
            }
        }
    }
    public void ToggleArms(bool state)
    {
        ArmadilloPlayerController.Instance.visualControl.ToggleArmView(state);
        if(!state)ResetAllCurrentWeapons();
    }
    public void OnGunEquip(WeaponType weaponType)
    {
        ArmadilloPlayerController.Instance.visualControl.OnGunEquiped(weaponType);
    }
    public void ResetAllCurrentWeapons()
    {
        foreach(Weapon weapon in weaponsInInventory)
        {
            if(weapon != null)weapon.ResetGun();
        }
    }

    public void OnRun(bool state)
    {
        if (currentWeaponID != -1 && weaponsInInventory[currentWeaponID] != null)
        {
            weaponsInInventory[currentWeaponID].ToggleOnRun(state);
        }
    }
}
