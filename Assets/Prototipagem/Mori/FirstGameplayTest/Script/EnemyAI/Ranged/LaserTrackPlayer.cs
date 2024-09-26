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
    private void Start()
    {
        gameObject.SetActive(false);
    }
    public void ResetLaser()
    {
        laserTrail.SetPosition(0, transform.position);
        laserTrail.SetPosition(1, transform.position);
    }

    private void Update()
    {
        Vector3 position = Vector3.Lerp(laserTrail.GetPosition(1), target, Time.deltaTime*10);
        laserTrail.SetPosition(0, transform.position);
        laserTrail.SetPosition(1, position);
    }
}
