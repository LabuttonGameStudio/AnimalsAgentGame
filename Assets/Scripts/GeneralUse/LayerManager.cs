using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LayerManager : MonoBehaviour
{
    public static LayerManager Instance;
    [SerializeField] public LayerMask groundMask;

    [SerializeField] public LayerMask playerMask;

    [SerializeField] public LayerMask enemyAttackMask;
    private void Awake()
    {
        Instance = this;
    }
}
