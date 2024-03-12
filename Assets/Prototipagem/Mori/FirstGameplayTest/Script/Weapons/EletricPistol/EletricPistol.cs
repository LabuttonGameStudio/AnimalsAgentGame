using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class EletricPistol : Weapon
{
    private ArmadilloWeaponControl weaponControl;
    public EletricPistol(ArmadilloWeaponControl armadilloWeaponControl)
    {
        weaponControl = armadilloWeaponControl;
        ammoType = AmmoType.Overheat;
        maxAmmoAmount = 100;
        name = "Pistola Eletrica";
        description = "Pressione pra atirar, segure para carregar ";
        LoadVisual();
        UpdateOverheatHUD();
    }
    //----- Stats -----
    readonly private int unchargedHitDamage = 20;
    readonly private int chargedHitDamage = 50;

    readonly private float chargeTime = 0.25f;

    //----- Visual -----
    private EletricPistolVisual visualHandler;
    private void LoadVisual()
    {
        GameObject modelPrefab;
        modelPrefab = Resources.Load<GameObject>("Prefabs/Weapons/EletricPistol");
        if (modelPrefab == null)
        {
            Debug.LogError("Erro ao encontrar Eletric Pistol Prefab em Prefabs/Weapons/EletricPistol");
        }
        Debug.Log(modelPrefab);
        Vector3 position = new Vector3(0.6f, -0.25f, 0.9f);
        Quaternion rotation = Quaternion.Euler(new Vector3(0, 85, 357.5f));
        GameObject model;
        model = weaponControl.LoadModel(modelPrefab, position, rotation);
        if (model.TryGetComponent(out EletricPistolVisual eletricPistolVisual))
        {
            visualHandler = eletricPistolVisual;
        }
        else Debug.LogError("Erro ao encontrar visual handler EletricPistolVisual no prefab");
    }
    //LoadModel


    //Fire
    #region
    public override void OnFireButtonPerformed(InputAction.CallbackContext performed)
    {
        StartHoldOrPressTimer();
    }
    public override void OnFireButtonCanceled(InputAction.CallbackContext performed)
    {
        StopHoldOrPressTimer();
    }

    public void StartHoldOrPressTimer()
    {
        if (holdOrPressTimerRef == null) holdOrPressTimerRef = weaponControl.StartCoroutine(HoldOrPressTimer_Coroutine());
        else
        {
            StopHoldOrPressTimer();
            holdOrPressTimerRef = weaponControl.StartCoroutine(HoldOrPressTimer_Coroutine());
        }
    }
    public void StopHoldOrPressTimer()
    {
        weaponControl.StopCoroutine(holdOrPressTimerRef);
        holdOrPressTimerRef = null;
        Fire(holdOrPressTimer);
    }
    public void Fire(float timer)
    {
        if (timer < chargeTime)
        {
            UnChargedFire();
        }
        else ChargedFire();
    }
    public void UnChargedFire()
    {
        RaycastHit[] targetsHit;
        Transform camera = ArmadilloPlayerController.Instance.cameraControl.mainCamera.transform;
        targetsHit = Physics.RaycastAll(camera.position, camera.forward, 20f);
        foreach (RaycastHit raycastHit in targetsHit)
        {
            if (raycastHit.collider.isTrigger) continue;
            Debug.Log(raycastHit.transform.name);
            if (raycastHit.transform.TryGetComponent(out IDamageable idamageable))
            {
                idamageable.TakeDamage(unchargedHitDamage);
                break;
            }
        }
        visualHandler.OnUnchargedFire();
    }
    public void ChargedFire()
    {
        RaycastHit[] targetsHit;
        Transform camera = ArmadilloPlayerController.Instance.cameraControl.mainCamera.transform;
        targetsHit = Physics.RaycastAll(camera.position, camera.forward, 30f);
        foreach (RaycastHit raycastHit in targetsHit)
        {
            if (raycastHit.collider.isTrigger) continue;
            Debug.Log(raycastHit.transform.name);
            if (raycastHit.transform.TryGetComponent(out IDamageable idamageable))
            {
                idamageable.TakeDamage(chargedHitDamage);
            }
        }
        visualHandler.OnChargedFire();
    }

    public Coroutine holdOrPressTimerRef;
    private float holdOrPressTimer;
    public IEnumerator HoldOrPressTimer_Coroutine()
    {
        holdOrPressTimer = 0;
        while (holdOrPressTimer < chargeTime)
        {
            holdOrPressTimer += Time.deltaTime;
            yield return null;
        }
        visualHandler.OnCharge();
        while (true)
        {
            holdOrPressTimer += Time.deltaTime;
            yield return null;
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

    }
    public override void OnReloadButtonCanceled(InputAction.CallbackContext performed)
    {

    }
    #endregion

    //OverHeat
    #region
    public void UpdateOverheatHUD()
    {
        weaponControl.currentWeaponUI.text = currentAmmoAmount.ToString() + "|" + maxAmmoAmount.ToString();
    }
    public void OnOverheatCharge(int chargeAmount)
    {
        if (currentAmmoAmount + chargeAmount <= maxAmmoAmount)
        {
            currentAmmoAmount += chargeAmount;
        }
        else
        {
            currentAmmoAmount = maxAmmoAmount;
            OnOverheat();
        }
        UpdateOverheatHUD();
    }
    public void OnOverheat()
    {

    }
    #endregion
}
