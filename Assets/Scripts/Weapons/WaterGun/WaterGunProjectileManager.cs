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

    public List<WaterPuddle> waterPuddleInPool;
    public List<WaterPuddle> waterPuddleInGame;

    private GameObject projectilePuddleParent;
    [SerializeField] private float waterPuddleMinimalSize;

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
        waterPuddleInGame = new List<WaterPuddle>();
        for (int i = 0; i < waterPuddlePoolSize; i++)
        {
            waterPuddleInPool.Add(Instantiate(waterPuddlePrefab, projectilePoolParent.transform).GetComponent<WaterPuddle>());
            waterPuddleInPool[i].gameObject.SetActive(false);
            waterPuddleInPool[i].gameObject.name = "WaterPuddle"+i;
        }
    }
    public void ReturnProjectileToPool(WaterGunProjectile waterGunProjectile)
    {
        watergunProjectileInGame.Remove(waterGunProjectile);
        watergunProjectilePool.Add(waterGunProjectile);
    }
    public void ReturnPuddleToPool(WaterPuddle waterPuddle)
    {
        waterPuddle.damageablesInPuddle = new List<IDamageable>();
        waterPuddle.UpdateSize(waterPuddleMinimalSize);
        waterPuddle.size = waterPuddleMinimalSize;
        waterPuddleInGame.Remove(waterPuddle);
        waterPuddleInPool.Add(waterPuddle);
    }
    public void SpawnPuddle(Vector3 position)
    {
        if (waterPuddleInPool.Count <= 0)
        {
            waterPuddleInGame[0].gameObject.SetActive(false);
            ReturnPuddleToPool(waterPuddleInGame[0]);
        }
        WaterPuddle waterPuddle = waterPuddleInPool[0];
        waterPuddleInPool.RemoveAt(0);
        waterPuddleInGame.Add(waterPuddle);

        waterPuddle.transform.position = position;
        waterPuddle.gameObject.SetActive(true);
    }
    public void TryMergePuddle(WaterPuddle puddle1, GameObject puddle2)
    {
        if(mergePuddle==null)
        {
            mergePuddle = new MergePuddle()
            {
                puddle1 = puddle1,
                puddle2 = puddle2,
            };
        }
        else
        {
            if(puddle1.gameObject == mergePuddle.puddle2)
            {
                MergePuddle(mergePuddle.puddle1, puddle1);
                mergePuddle = null;
            }
        }
    }
    private MergePuddle mergePuddle;
    public void MergePuddle(WaterPuddle puddle1, WaterPuddle puddle2)
    {
        float puddleMaxSize = puddle1.realSize + puddle2.realSize;
        Vector3 middlePoint = puddle1.transform.position*(puddle1.size/ puddleMaxSize) + puddle2.transform.position*(puddle2.size/ puddleMaxSize);
        puddle2.gameObject.SetActive(false);
        puddle1.transform.position = middlePoint;
        puddle1.UpdateSize(Mathf.Max(puddle1.realSize, puddle2.realSize));
        puddle1.ChangeSize(puddle1.realSize+puddle2.realSize);
        ReturnPuddleToPool(puddle2);
    }
}
public class MergePuddle
{
    public WaterPuddle puddle1;
    public GameObject puddle2;
}
