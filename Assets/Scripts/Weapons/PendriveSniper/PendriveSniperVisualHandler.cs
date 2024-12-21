using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PendriveSniperVisualHandler : MonoBehaviour
{
    public static PendriveSniperVisualHandler Instance;
    private void Awake()
    {
        Instance = this;
    }
    public Transform firePivot;

    [SerializeField] private CanvasGroup canvasScope;

    [SerializeField] private GameObject model;
    public void ToggleVisual(bool state)
    {
        model.SetActive(state);
    }
    public void ResetVisuals()
    {

    }
    public void OnEquip(bool playAnimation)
    {
        if (playAnimation)
        {
            //weaponAnimator.CrossFade("WaterGunEquip", crossFadeTime);
        }
        else
        {
            //weaponAnimator.CrossFade("WaterGunIdle", crossFadeTime);
        }

    }
    public void OnUnequip()
    {
        //weaponAnimator.CrossFade("WaterGunUnequip", crossFadeTime);
    }
    public void OnReload()
    {
        //weaponAnimator.SetTrigger("waterGunReload");
    }
    public void ToggleOnRun(bool state)
    {
        //weaponAnimator.SetBool("waterGunIsRunning", state);
    }
    public void ToggleScope(bool state )
    {
        ArmadilloPlayerController.Instance.cameraControl.ToggleZoom(state);
        canvasScope.alpha = state? 1 : 0;
    }
}
