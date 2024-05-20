using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

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
        ammoSlider = armadilloWeaponControl.Weaponammoslider;
        LoadVisual();
        UpdateOverheatHUD();
    }

    //----- Stats -----
    //Tiro normal
    readonly private int unchargedHitDamage = 20;
    readonly private int unchargedOverheatCharge = 15;
    //Tiro carregado
    readonly private int chargedHitDamage = 50;
    readonly private float chargeTime = 0.25f;
    readonly private int chargedOverheatCharge = 55;
    //Overheat
    private bool isOverheating;
    private Image AmmoImage;
    readonly private float overheatDuration = 2f;
    readonly private float timeUntilPassiveCoolOffStarts = 1f;
    readonly private float timeToFullCoolOff = 1.5f;
    //----- Visual -----
    private EletricPistolVisual visualHandler;

    //Visual
    #region
    private void LoadVisual()
    {
        GameObject modelPrefab;
        modelPrefab = Resources.Load<GameObject>("Prefabs/Weapons/EletricPistol");
        if (modelPrefab == null)
        {
            Debug.LogError("Erro ao encontrar Eletric Pistol Prefab em Prefabs/Weapons/EletricPistol");
        }
        Vector3 position = new Vector3(0.6f, -0.25f, 0.9f);
        Quaternion rotation = Quaternion.Euler(new Vector3(0, 89, 0));
        GameObject model;
        model = weaponControl.LoadModel(modelPrefab, position, rotation);
        if (model.TryGetComponent(out EletricPistolVisual eletricPistolVisual))
        {
            visualHandler = eletricPistolVisual;
            visualHandler.gameObject.SetActive(false);
        }
        else Debug.LogError("Erro ao encontrar visual handler EletricPistolVisual no prefab");
    }

    public override void ToggleVisual(bool state)
    {
        visualHandler.gameObject.SetActive(state);
    }
    #endregion
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
        if (!isOverheating)
        {
            if (holdOrPressTimerRef == null) holdOrPressTimerRef = weaponControl.StartCoroutine(HoldOrPressTimer_Coroutine());
            else
            {
                StopHoldOrPressTimer();
                holdOrPressTimerRef = weaponControl.StartCoroutine(HoldOrPressTimer_Coroutine());
            }
        }
    }
    public void StopHoldOrPressTimer()
    {
        if (holdOrPressTimerRef != null)
        {
            weaponControl.StopCoroutine(holdOrPressTimerRef);
            holdOrPressTimerRef = null;
            Fire(holdOrPressTimer);
        }
    }
    public void Fire(float timer)
    {
        if (timer < chargeTime)
        {
            FPCameraShake.StartShake(0.05f, 0.25f, 5f);
            UnChargedFire();
        }
        else
        {
            FPCameraShake.StartShake(0.1f, 0.5f, 5f);
            ChargedFire();
        }
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
                Damage damage = new Damage(unchargedHitDamage, Damage.DamageType.Eletric, true, weaponControl.transform.position);
                idamageable.TakeDamage(damage);
                break;
            }
        }
        OnOverheatCharge(unchargedOverheatCharge);
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
                Damage damage = new Damage(chargedHitDamage, Damage.DamageType.Eletric, true, weaponControl.transform.position);
                idamageable.TakeDamage(damage);
            }
        }
        OnOverheatCharge(chargedOverheatCharge);
        FPCameraShake.StopShake(holdShakeStats);
        holdShakeStats = null;
        visualHandler.OnChargedFire();
    }

    public Coroutine holdOrPressTimerRef;
    private float holdOrPressTimer;
    private ShakeStats holdShakeStats;
    public IEnumerator HoldOrPressTimer_Coroutine()
    {
        holdOrPressTimer = 0;
        while (holdOrPressTimer < chargeTime)
        {
            holdOrPressTimer += Time.deltaTime;
            yield return null;
        }
        visualHandler.OnCharge();
        holdShakeStats = FPCameraShake.StartShake(0.05f, 1f);
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
        weaponControl.currentWeaponUI.text = " x" + currentAmmoAmount.ToString() + "%";
        float currentPercentage = ((float)currentAmmoAmount) / ((float)maxAmmoAmount);
        if (currentPercentage > 1) currentPercentage = 1;
        ammoSlider.value = currentPercentage;
        weaponControl.currentWeaponUI.color = new Color(1, 1 - currentPercentage, 1 - currentPercentage);
        AmmoImage = ammoSlider.GetComponentInChildren<Image>();
        AmmoImage.color = new Color(1, 1 - currentPercentage, 1 - currentPercentage);

    }
    public void OnOverheatCharge(int chargeAmount)
    {
        currentAmmoAmount += chargeAmount;
        StopPassiveCoolOff();
        if (currentAmmoAmount >= maxAmmoAmount) OnOverheat();
        else
        {
            StartPassiveCoolOff();
        }
        UpdateOverheatHUD();
    }
    public void OnOverheat()
    {
        isOverheating = true;
        visualHandler.OnEnterOverheatMode();
        if (overheatTimer_Ref == null)
        {
            overheatTimer_Ref = weaponControl.StartCoroutine(OverheatTimer_Coroutine());
        }
        else
        {
            weaponControl.StopCoroutine(overheatTimer_Ref);
            overheatTimer_Ref = weaponControl.StartCoroutine(OverheatTimer_Coroutine());
        }
        //start timer 
    }

    private Coroutine overheatTimer_Ref;
    public IEnumerator OverheatTimer_Coroutine()
    {
        yield return new WaitForSeconds(0.25f);
        float timer = 0;
        int startingAmmoAmount = currentAmmoAmount;
        while (timer < overheatDuration)
        {
            Debug.Log(startingAmmoAmount);
            currentAmmoAmount = Mathf.RoundToInt(Mathf.Lerp(startingAmmoAmount, 0, timer / (overheatDuration-0.25f)));
            UpdateOverheatHUD();
            timer += 0.05f;
            yield return new WaitForSeconds(0.05f);
        }
        isOverheating = false;
        visualHandler.OnLeaveOverheatMode();
        currentAmmoAmount = 0;
        UpdateOverheatHUD();
    }
    private void StartPassiveCoolOff()
    {
        if (passiveCoolOff_Ref == null) passiveCoolOff_Ref = weaponControl.StartCoroutine(PassiveCoolOff_Coroutine());
    }
    private void StopPassiveCoolOff()
    {
        if (passiveCoolOff_Ref != null)
        {
            weaponControl.StopCoroutine(passiveCoolOff_Ref);
            passiveCoolOff_Ref = null;
        }
    }
    private Coroutine passiveCoolOff_Ref;
    public IEnumerator PassiveCoolOff_Coroutine()
    {
        yield return new WaitForSeconds(timeUntilPassiveCoolOffStarts);

        float timer = 0;
        float ammoAmountAsFloat = currentAmmoAmount;
        while (ammoAmountAsFloat > 0)
        {
            currentAmmoAmount = Mathf.RoundToInt(ammoAmountAsFloat);
            UpdateOverheatHUD();
            timer += 0.05f;
            yield return new WaitForSeconds(0.05f);
            ammoAmountAsFloat -= (maxAmmoAmount / timeToFullCoolOff) * 0.05f;
        }
        currentAmmoAmount = 0;
        UpdateOverheatHUD();
        passiveCoolOff_Ref = null;
    }
    #endregion

}
