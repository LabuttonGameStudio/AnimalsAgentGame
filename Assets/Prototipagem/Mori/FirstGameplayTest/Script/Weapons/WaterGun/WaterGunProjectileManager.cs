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
    public float velocity;

    [Header("Puddle")]
    [SerializeField] private GameObject waterPuddlePrefab;
    [SerializeField] private int waterPuddlePoolSize;

    [HideInInspector] public List<WaterPuddle> waterPuddleInPool;
    [HideInInspector] public List<WaterPuddle> waterPuddleGame;

    private GameObject projectilePuddleParent;


    private void Awake()
    {
        Instance = this;
        poolParent = new GameObject();
        poolParent.name = "WaterGunPool";

        //Projectile Pool 
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

        //Puddle Pool
        projectilePuddleParent = new GameObject();
        projectilePuddleParent.transform.parent = poolParent.transform;
        projectilePuddleParent.name = "PuddlePool";

        waterPuddleInPool = new List<WaterPuddle>();
        waterPuddleGame = new List<WaterPuddle>();
        for (int i = 0; i < waterPuddlePoolSize; i++)
        {
            waterPuddleInPool.Add(Instantiate(waterPuddlePrefab, projectilePoolParent.transform).GetComponent<WaterPuddle>());
            waterPuddleInPool[i].gameObject.SetActive(false);
        }
    }
    public void ReturnProjectileToPool(WaterGunProjectile waterGunProjectile)
    {
        watergunProjectileInGame.Remove(waterGunProjectile);
        watergunProjectilePool.Add(waterGunProjectile);
    }
    public void ReturnPuddleToPool(WaterPuddle waterPuddle)
    {
        waterPuddleGame.Remove(waterPuddle);
        waterPuddleInPool.Add(waterPuddle);
    }
    public void SpawnPuddle(Vector3 position)
    {
        WaterPuddle waterPuddle = waterPuddleInPool[0];
        waterPuddleInPool.RemoveAt(0);
        waterPuddleInPool.Add(waterPuddle);

        waterPuddle.transform.position = position;
        waterPuddle.gameObject.SetActive(true);
    }
    public void TryMergePuddle(WaterPuddle puddle1, WaterPuddle puddle2)
    {

    }
    public void MergePuddle(WaterPuddle puddle1, WaterPuddle puddle2)
    {
        Vector3 middlePoint = (puddle1.transform.position + puddle2.transform.position)/2;
        puddle2.gameObject.SetActive(false);
        puddle1.transform.position = middlePoint;
        puddle1.ChangeSize(puddle1.size + puddle2.size);
        ReturnPuddleToPool(puddle2);
    }
}
