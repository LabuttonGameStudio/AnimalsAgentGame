using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyControl : MonoBehaviour
{
    public static EnemyControl Instance { get; private set;}

    [HideInInspector]public List<IEnemy> enemiesList;

    private void Awake()
    {
        enemiesList = new List<IEnemy>();
        Instance = this;
    }
}
