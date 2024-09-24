using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LayerManager : MonoBehaviour
{
    public static LayerManager Instance;
    [SerializeField] public LayerMask groundMask;
    private void Awake()
    {
        Instance = this;
    }
}
