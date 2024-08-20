using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class WaterGunProjectileManager : MonoBehaviour
{
    public static WaterGunProjectileManager Instance;
    [SerializeField] private GameObject watergunProjectilePrefab;

    public List<WaterGunProjectile> watergunProjectilePool;
    [HideInInspector]public List<WaterGunProjectile> watergunProjectileInGame;

    [SerializeField] private int projectilePoolSize;
    private GameObject poolParent;

    private GameObject projectilePoolParent;
    private GameObject projectilePuddleParent;

    [SerializeField] private float velocity;

    private void Awake()
    {
        Instance = this;
        poolParent = new GameObject();
        poolParent.name = "WaterGunPool";

        projectilePoolParent = new GameObject();
        projectilePoolParent.transform.parent = poolParent.transform;
        projectilePoolParent.name = "ProjectilePool";

        watergunProjectilePool = new List<WaterGunProjectile>();
        watergunProjectileInGame = new List<WaterGunProjectile>();
        for (int i = 0; i < projectilePoolSize; i++)
        {
            watergunProjectilePool.Add(Instantiate(watergunProjectilePrefab,projectilePoolParent.transform).GetComponent<WaterGunProjectile>());
            watergunProjectilePool[i].rb = watergunProjectilePool[i].GetComponent<Rigidbody>();
            watergunProjectilePool[i].gameObject.SetActive(false);
        }

        projectilePuddleParent = new GameObject();
        projectilePuddleParent.transform.parent = poolParent.transform;
        projectilePuddleParent.name = "PuddlePool";
    }
    public void ReturnToPool(WaterGunProjectile waterGunProjectile)
    {
        watergunProjectileInGame.Remove(waterGunProjectile);
        watergunProjectilePool.Add(waterGunProjectile);
    }
    float timer = 0f;
    float interval = 0.25f;
    private void FixedUpdate()
    {
        timer += Time.fixedDeltaTime;
        if(timer> interval)
        {
            Fire(transform.position, transform.forward, velocity);
            timer = 0f;
        }
    }
    //Transfer to Weapon Script
    public void Fire(Vector3 position, Vector3 direction,float velocity)
    {
        WaterGunProjectile waterGunProjectile = watergunProjectilePool[0];
        watergunProjectilePool.RemoveAt(0);
        watergunProjectileInGame.Add(waterGunProjectile);
        waterGunProjectile.transform.position = position;
        waterGunProjectile.duration = 1;
        waterGunProjectile.gameObject.SetActive(true);
        waterGunProjectile.rb.velocity = direction * velocity;
    }
}
