using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class ArmadilloWeaponControl : MonoBehaviour
{
    private bool isGunPocketed;

    [SerializeField] private bool startWithPistol;

    [System.NonSerialized] public Weapon[] weaponsInInventory;

    [SerializeField] public int currentWeaponID = -1;
    [SerializeField] private int pocketedWeaponID = -1;

    [SerializeField] private Camera weaponCamera;

    public TextMeshProUGUI currentWeaponUI;
    public Slider Weaponammoslider;

    public void Start()
    {
        weaponsInInventory = new Weapon[2];
        if (startWithPistol) GivePlayerEletricPistol();
    }
    public void GivePlayerEletricPistol()
    {
        weaponsInInventory[0] = new EletricPistol(this);
        if (isGunPocketed)
        {
            Debug.Log("a");
            pocketedWeaponID = 0;
        }
        else ChangeWeapon(0);
    }
    public GameObject LoadModel(GameObject model, Vector3 position, Quaternion rotation)
    {
        GameObject objectInstantiated = Instantiate(model, weaponCamera.transform); 
        objectInstantiated.transform.localPosition = position;
        objectInstantiated.transform.localRotation = rotation;
        return objectInstantiated;
    }
    private enum DelegateType
    {
        Add,
        Remove,
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

    public void ChangeWeapon(int nextWeapon)
    {
        PlayerInputAction.ArmadilloActions playerInput = ArmadilloPlayerController.Instance.inputControl.inputAction.Armadillo;
        if (nextWeapon < 0 || nextWeapon > weaponsInInventory.Length)
        {

            if (currentWeaponID == -1) return;
            DefineDelegates(playerInput, DelegateType.Remove, currentWeaponID);
            ToggleStateInputs(playerInput, false);
            weaponsInInventory[currentWeaponID].ToggleVisual(false);
            currentWeaponID = -1;
            return;
        }
        if (currentWeaponID != -1)
        {
            DefineDelegates(playerInput, DelegateType.Remove, currentWeaponID);
            weaponsInInventory[currentWeaponID].ToggleVisual(false);
        }
        else
        {
            ToggleStateInputs(playerInput, true);
        }
        currentWeaponID = nextWeapon;
        Debug.Log(nextWeapon);
        OnGunEquip(nextWeapon);
        DefineDelegates(playerInput, DelegateType.Add, nextWeapon);
        weaponsInInventory[nextWeapon].ToggleVisual(true);
    }

    public void ToggleWeapon(bool state)
    {
        isGunPocketed =!state;
        if (!state)
        {
            if (currentWeaponID != -1) pocketedWeaponID = currentWeaponID;
            ChangeWeapon(-1);
        }
        else
        {
            ChangeWeapon(pocketedWeaponID);
        }
    }
    public void OnGunEquip(int gunEquiped)
    {
        ArmadilloPlayerController.Instance.visualControl.OnGunEquiped();
    }
}
