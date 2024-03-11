using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ArmadilloWeaponControl : MonoBehaviour
{
    [System.NonSerialized]public Weapon[] weaponsInInventory;

    [System.NonSerialized] public int currentWeaponID;

    [SerializeField] private Camera weaponCamera;

    public TextMeshProUGUI currentWeaponUI;

    public void Start()
    {
        weaponsInInventory = new Weapon[2];
        weaponsInInventory[0] = new EletricPistol(this);
        currentWeaponID = 0;
        ChangeWeapon(0);
    }
    public GameObject LoadModel(GameObject model,Vector3 position,Quaternion rotation)
    {
        return Instantiate(model, position, rotation, weaponCamera.transform);
    }
    public void ChangeWeapon(int nextWeapon)
    {
        PlayerInputAction.ArmadilloActions playerInput = ArmadilloPlayerController.Instance.inputControl.inputAction.Armadillo;
        playerInput.Fire.Enable();
        playerInput.Fire.performed += weaponsInInventory[currentWeaponID].OnFireButtonPerformed;
        playerInput.Fire.canceled += weaponsInInventory[currentWeaponID].OnFireButtonCanceled;

        playerInput.AltFire.Enable();
        playerInput.AltFire.performed += weaponsInInventory[currentWeaponID].OnAltFireButtonPerformed;
        playerInput.AltFire.canceled += weaponsInInventory[currentWeaponID].OnAltFireButtonCanceled;

        playerInput.Reload.Enable();
        playerInput.Reload.performed += weaponsInInventory[currentWeaponID].OnReloadButtonPerformed;
        playerInput.Reload.canceled += weaponsInInventory[currentWeaponID].OnReloadButtonCanceled;
    }
}
