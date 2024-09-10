using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterGun_Collectable : MonoBehaviour
{
    [SerializeField] private int ammoAmount;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Use();
        }
    }
    private void Use()
    {
        if (ArmadilloPlayerController.Instance.weaponControl.GainAmmo(WeaponData.WeaponType.Watergun, ammoAmount))
        {
            gameObject.SetActive(false);
        }
    }
}
