using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollowPlayer : MonoBehaviour
{
    [SerializeField] private Transform playerCameraPoint;
    void Update()
    {
        transform.position = playerCameraPoint.position;
    }
}
