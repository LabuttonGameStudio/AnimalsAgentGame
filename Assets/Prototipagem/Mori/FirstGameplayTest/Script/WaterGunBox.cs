using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class WaterGunBox : MonoBehaviour, InteractiveObject
{
    public bool isEnabled { get; set; }
    bool singleUse;
    public string GetObjectDescription()
    {
        return "Pegar Arma de agua";
    }

    public string GetObjectName()
    {
        return "Caixa";
    }

    private void Awake()
    {
        isEnabled = true;
    }

    public void Interact(InputAction.CallbackContext value)
    {
        if (!singleUse)
        {
            singleUse = true;
            ArmadilloPlayerController.Instance.weaponControl.GivePlayerWaterGun();
        }
    }
}
