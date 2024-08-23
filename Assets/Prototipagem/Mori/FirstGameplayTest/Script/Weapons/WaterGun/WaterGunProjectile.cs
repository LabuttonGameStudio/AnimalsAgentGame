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
        if(other.CompareTag("Puddle"))
        {
            DisableProjectile();
            other.GetComponent<WaterPuddle>().ChangeSize(1);
        }
        Debug.Log(other.gameObject.name);
    }
    private void DisableProjectile()
    {
        timer =0;
        rb.velocity = Vector3.zero;
        WaterGunProjectileManager.Instance.ReturnToPool(this);
        gameObject.SetActive(false);
    }
}
