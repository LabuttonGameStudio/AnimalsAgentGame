using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowObject : MonoBehaviour
{
    [SerializeField] private Transform objectTransform;
    void Update()
    {
        transform.position = objectTransform.position;
    }
}
