using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponCameraFollowPlayer : MonoBehaviour
{
    [SerializeField] private Transform cameraFollowPoint;
    private void Update()
    {
        transform.position = cameraFollowPoint.position;
        transform.rotation = cameraFollowPoint.rotation;
    }
}
