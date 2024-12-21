using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HP_Collectable : MonoBehaviour
{
    [SerializeField] private float hpAmount;

    [SerializeField]private bool respawn;
    [SerializeField] private float respawnTimer;
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
            if(respawn)ArmadilloPlayerController.Instance.StartCoroutine(RespawnTimer_Coroutine());
        }
    }
    private IEnumerator RespawnTimer_Coroutine()
    {
        yield return new WaitForSeconds(respawnTimer);
        gameObject.SetActive(true);
    }
}
