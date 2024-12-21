using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponCameraFollowPlayer : MonoBehaviour
{
    [SerializeField] private Transform cameraFollowPoint;
    [SerializeField] private float cameraDistanceFromPlayer;
    private void Awake()
    {
        transform.position = cameraFollowPoint.position;
        transform.rotation = cameraFollowPoint.rotation;
    }
    private void Update()
    {
        transform.position = cameraFollowPoint.position-cameraFollowPoint.forward* cameraDistanceFromPlayer;
        transform.rotation = cameraFollowPoint.rotation;
    }
}
