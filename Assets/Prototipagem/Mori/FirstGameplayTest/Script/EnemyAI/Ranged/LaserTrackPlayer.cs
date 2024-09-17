using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserTrackPlayer : MonoBehaviour
{
    LineRenderer laserTrail;
    [SerializeField]Transform target;
    private void Awake()
    {
        laserTrail = GetComponent<LineRenderer>();
    }
    private void Update()
    {
        Vector3 position = Vector3.Lerp(laserTrail.GetPosition(1), target.position - transform.position, Time.deltaTime*10);
        laserTrail.SetPosition(1, position);
    }
}
