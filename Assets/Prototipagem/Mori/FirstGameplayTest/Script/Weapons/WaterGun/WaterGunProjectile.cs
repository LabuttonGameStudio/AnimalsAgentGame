using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterGunProjectile : MonoBehaviour
{
    [HideInInspector] public Rigidbody rb;
    [HideInInspector] public float duration;
    [HideInInspector]public float timer;
    private void FixedUpdate()
    {
        timer += Time.fixedDeltaTime;
        if(timer>duration)
        {
            DisableProjectile();
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        int layerInt = other.gameObject.layer;
        if(layerInt == LayerMask.NameToLayer("Ground"))
        {
            WaterGunProjectileManager.Instance.SpawnPuddle(rb.position);
            DisableProjectile();
        }
        else if (other.CompareTag("Puddle"))
        {
            DisableProjectile();
            other.GetComponent<WaterPuddle>().AddSize(1);
        }
    }
    private void DisableProjectile()
    {
        timer =0;
        rb.velocity = Vector3.zero;
        WaterGunProjectileManager.Instance.ReturnProjectileToPool(this);
        gameObject.SetActive(false);
    }
}
