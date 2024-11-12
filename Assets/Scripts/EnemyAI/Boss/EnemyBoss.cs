using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBoss : MonoBehaviour
{
    private float currentHp;
    [SerializeField]private float maxHp;
    private void Awake()
    {
        currentHp = maxHp;
    }
}
