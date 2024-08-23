using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class WaterGunProjectileManager : MonoBehaviour
{
    public static WaterGunProjectileManager Instance;
    private GameObject poolParent;

    [Header("Projectile")]
    [SerializeField] private GameObject watergunProjectilePrefab;
    [SerializeField] private int projectilePoolSize;

    [HideInInspector] public List<WaterGunProjectile> watergunProjectilePool;
    [HideInInspector]public List<WaterGunProjectile> watergunProjectileInGame;

    private GameObject projectilePoolParent;

    [Header("Puddle")]
    [SerializeField] private GameObject waterPuddlePrefab;
    [SerializeField] private int waterPuddlePoolSize;

    [HideInInspector] public List<WaterGunProjectile> waterPuddleInPool;
    [HideInInspector] public List<WaterGunProjectile> waterPuddleGame;

    private GameObject projectilePuddleParent;

    public float velocity;

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
}
