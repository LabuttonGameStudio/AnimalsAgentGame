using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PendriveSniperProjectileManager : MonoBehaviour
{
    public static PendriveSniperProjectileManager Instance;

    //-----Projectile Stats-----
    [Header("Projectile Stats")]
    public float velocity;

    //----- Projectile Pooling
    [Header("Projectile Pooling")]
    [SerializeField] private GameObject pendriveSniperProjectilePrefab;
    [SerializeField] private int projectilePoolSize;
    private GameObject projectilePoolParent;

    [HideInInspector] public List<PendriveSniperProjectile> pendriveSniperProjectilePool;
    [HideInInspector] public List<PendriveSniperProjectile> pendriveSniperProjectileInGame;
    private void Awake()
    {
        Instance = this;
        projectilePoolParent = new GameObject();
        projectilePoolParent.name = "PendriveSniperProjectilePool";

        pendriveSniperProjectilePool = new List<PendriveSniperProjectile>();
        pendriveSniperProjectileInGame = new List<PendriveSniperProjectile>();

        for (int i = 0; i < projectilePoolSize; i++)
        {
            pendriveSniperProjectilePool.Add(Instantiate(pendriveSniperProjectilePrefab, projectilePoolParent.transform).GetComponent<PendriveSniperProjectile>());
            pendriveSniperProjectilePool[i].rb = pendriveSniperProjectilePool[i].GetComponent<Rigidbody>();
            pendriveSniperProjectilePool[i].gameObject.SetActive(false);
        }

    }
    public void ReturnProjectileToPool(PendriveSniperProjectile pendriveSniperProjectile)
    {
        pendriveSniperProjectileInGame.Remove(pendriveSniperProjectile);
        pendriveSniperProjectilePool.Add(pendriveSniperProjectile);
    }
}
