using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HP_Collectable : MonoBehaviour
{
    [SerializeField] private float hpAmount;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Use();
        }
    }
    private void Use()
    {
        if(ArmadilloPlayerController.Instance.hpControl.OnHeal(hpAmount))
        {
            gameObject.SetActive(false);
        }
    }
}
