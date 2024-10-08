using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LayerManager : MonoBehaviour
{
    public static LayerManager Instance;
    [SerializeField] public LayerMask groundMask;

    [SerializeField] public LayerMask playerMask;
    private void Awake()
    {
        Instance = this;
    }
}
