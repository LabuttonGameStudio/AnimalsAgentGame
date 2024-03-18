using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ArmadilloWeaponControl : MonoBehaviour
{
    [System.NonSerialized] public Weapon[] weaponsInInventory;

    [System.NonSerialized] public int currentWeaponID = -1;

    [SerializeField] private Camera weaponCamera;

    public TextMeshProUGUI currentWeaponUI;

    public void Start()
    {
        weaponsInInventory = new Weapon[2];
        weaponsInInventory[0] = new EletricPistol(this);
        ChangeWeapon(0);
    }
    public GameObject LoadModel(GameObject model, Vector3 position, Quaternion rotation)
    {
        GameObject objectInstantiated = Instantiate(model, weaponCamera.transform);
        objectInstantiated.transform.localPosition = position;
        objectInstantiated.transform.localRotation = rotation;
        return objectInstantiated;
    }
    public void ChangeWeapon(int nextWeapon)
    {
        PlayerInputAction.ArmadilloActions playerInput = ArmadilloPlayerController.Instance.inputControl.inputAction.Armadillo;
        if (nextWeapon < 0 || nextWeapon > weaponsInInventory.Length)
        {
            playerInput.Fire.performed -= weaponsInInventory[currentWeaponID].OnFireButtonPerformed;
            playerInput.Fire.canceled -= weaponsInInventory[currentWeaponID].OnFireButtonCanceled;
            playerInput.Fire.Disable();

            playerInput.AltFire.performed -= weaponsInInventory[currentWeaponID].OnAltFireButtonPerformed;
            playerInput.AltFire.canceled -= weaponsInInventory[currentWeaponID].OnAltFireButtonCanceled;
            playerInput.AltFire.Disable();

            playerInput.Reload.performed -= weaponsInInventory[currentWeaponID].OnReloadButtonPerformed;
            playerInput.Reload.canceled -= weaponsInInventory[currentWeaponID].OnReloadButtonCanceled;
            playerInput.Reload.Disable();
            currentWeaponID = -1;
            return;
        }
        if (currentWeaponID != -1)
        {
            playerInput.Fire.performed -= weaponsInInventory[currentWeaponID].OnFireButtonPerformed;
            playerInput.Fire.canceled -= weaponsInInventory[currentWeaponID].OnFireButtonCanceled;

            playerInput.AltFire.performed -= weaponsInInventory[currentWeaponID].OnAltFireButtonPerformed;
            playerInput.AltFire.canceled -= weaponsInInventory[currentWeaponID].OnAltFireButtonCanceled;

            playerInput.Reload.performed -= weaponsInInventory[currentWeaponID].OnReloadButtonPerformed;
            playerInput.Reload.canceled -= weaponsInInventory[currentWeaponID].OnReloadButtonCanceled;
        }
        else
        {
            playerInput.Fire.Enable();
            playerInput.AltFire.Enable();
            playerInput.Reload.Enable();
        }
        playerInput.Fire.performed += weaponsInInventory[nextWeapon].OnFireButtonPerformed;
        playerInput.Fire.canceled += weaponsInInventory[nextWeapon].OnFireButtonCanceled;

        playerInput.AltFire.performed += weaponsInInventory[nextWeapon].OnAltFireButtonPerformed;
        playerInput.AltFire.canceled += weaponsInInventory[nextWeapon].OnAltFireButtonCanceled;

        playerInput.Reload.performed += weaponsInInventory[nextWeapon].OnReloadButtonPerformed;
        playerInput.Reload.canceled += weaponsInInventory[nextWeapon].OnReloadButtonCanceled;
        currentWeaponID = nextWeapon;
    }
}
