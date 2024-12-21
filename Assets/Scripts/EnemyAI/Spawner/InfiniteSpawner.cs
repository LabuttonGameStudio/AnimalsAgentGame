using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfiniteSpawner : MonoBehaviour
{
    [SerializeField] private bool startOnPlay;
    [SerializeField] private bool spawnOnStart;

    [SerializeField]private IEnemy[] enemyPool;

    [SerializeField] private int maxEnemiesAtOnce;
    [SerializeField] private float enemiesPerMinute;

    private int lastPickedEnemy;

    private void Start()
    {
        if(startOnPlay)StartSpawning();
    }
    public void StartSpawning()
    {
        spawn_Ref = StartCoroutine(Spawn_Coroutine());
    }
    public void StopSpawning()
    {
        if(spawn_Ref != null)
        {
            StopCoroutine(spawn_Ref);
            spawn_Ref = null;
        }
    }
    public void StopSpawningAndKillAll()
    {
        if (spawn_Ref != null)
        {
            StopCoroutine(spawn_Ref);
            spawn_Ref = null;
        }
        foreach(IEnemy enemy in enemyPool)
        {
            if (enemy.isDead || !enemy.gameObject.activeInHierarchy) continue;
            ((IDamageable)enemy).TakeDamage(new Damage(1000, Damage.DamageType.Blunt, false, Vector3.zero));
        }
    }

    private Coroutine spawn_Ref;
    private IEnumerator Spawn_Coroutine()
    {
        float timer = 0;
        bool firstSpawn = true;
        while(true)
        {
            if(CurrentActiveEnemies()<maxEnemiesAtOnce)
            {
                timer += 1;
                if (timer > 60/enemiesPerMinute || (spawnOnStart&& firstSpawn))
                {
                    firstSpawn = false;
                    IEnemy enemy = GetDisabledEnemy();
                    if(enemy != null)
                    {
                        if (enemy.isDead)
                        {
                            enemy.currentAIBehaviour = AIBehaviourEnums.AIBehaviour.Attacking;
                            enemy.Revive(transform.position, transform.rotation);
                        }
                        else
                        {
                            enemy.transform.position = transform.position;
                            enemy.transform.rotation = transform.rotation;
                            enemy.currentAIBehaviour = AIBehaviourEnums.AIBehaviour.Attacking;
                            enemy.gameObject.SetActive(true);
                        }
                        timer = 0;
                    }
                }
            }
            yield return new WaitForSeconds(1);
        }
    }

    private int CurrentActiveEnemies()
    {
        int enemyCount = 0;
        foreach(IEnemy enemy in enemyPool)
        {
            if(enemy.gameObject.activeInHierarchy)enemyCount++;
        }
        return enemyCount;
    }
    private IEnemy GetDisabledEnemy()
    {
        for(int i = lastPickedEnemy+1; i < enemyPool.Length; i++)
        {
            if (!enemyPool[i].gameObject.activeInHierarchy)
            {
                lastPickedEnemy = i;
                return enemyPool[i];
            }
        }
        for (int i = 0; i < enemyPool.Length; i++)
        {
            if (!enemyPool[i].gameObject.activeInHierarchy)
            {
                lastPickedEnemy = i;
                return enemyPool[i];
            }
        }
        return null;
    }
}
