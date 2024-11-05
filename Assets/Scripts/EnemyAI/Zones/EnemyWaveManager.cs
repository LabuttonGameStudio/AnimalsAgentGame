using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EnemyWaveManager : MonoBehaviour
{
    [SerializeField]private IEnemy[] enemiesInWave;
    private bool hasBeenDefeated;
    [SerializeField] private UnityEvent onWaveReleased;
    [SerializeField] private UnityEvent onWaveDefeated;
    private void Start()
    {
        foreach(IEnemy enemy in enemiesInWave)
        {
            enemy.onDeathEvent.AddListener(CheckOnWaveDefeated);
        }
    }
    public void ReleaseWave()
    {
        onWaveReleased.Invoke();
        foreach(IEnemy enemy in enemiesInWave)
        {
            enemy.transform.parent.gameObject.SetActive(true);
        }
    }
    public void CheckOnWaveDefeated()
    {
        if (hasBeenDefeated) return;
        bool defeated = true;
        foreach (IEnemy enemy in enemiesInWave)
        {
            if (!enemy.isDead)
            {
                defeated = false;
            }
        }
        if(defeated)
        {
            hasBeenDefeated = true;
            onWaveDefeated.Invoke();
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
