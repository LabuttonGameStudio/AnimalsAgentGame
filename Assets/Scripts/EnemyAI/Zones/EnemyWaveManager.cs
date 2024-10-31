using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyWaveManager : MonoBehaviour
{
    [SerializeField]private IEnemy[] enemiesInWave;

    public void ReleaseWave()
    {
        foreach(IEnemy enemy in enemiesInWave)
        {
            enemy.gameObject.SetActive(true);
        }
    }
    public bool IsWaveDead()
    {
        foreach (IEnemy enemy in enemiesInWave)
        {
            if (!enemy.isDead)
            {
                return false;
            }
        }
        return true;
    }
}
