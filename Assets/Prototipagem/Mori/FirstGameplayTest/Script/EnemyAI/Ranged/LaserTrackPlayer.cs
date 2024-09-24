using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserTrackPlayer : MonoBehaviour
{
    LineRenderer laserTrail;
    [HideInInspector]public Vector3 target;
    private void Awake()
    {
        laserTrail = GetComponent<LineRenderer>();
    }
    public void ResetLaser()
    {
        laserTrail.SetPosition(1, Vector3.zero);
    }

    private void Update()
    {
        Vector3 position = Vector3.Lerp(laserTrail.GetPosition(1), target - transform.position, Time.deltaTime*10);
        laserTrail.SetPosition(1, position);
    }
}
