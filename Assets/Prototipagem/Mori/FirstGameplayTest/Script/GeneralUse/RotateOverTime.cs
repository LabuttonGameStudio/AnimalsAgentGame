using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateOverTime : MonoBehaviour
{
    [SerializeField] private Vector3 rotation;
    private void Update()
    {
        transform.Rotate(transform.rotation.eulerAngles+rotation*Time.deltaTime);
    }
}
