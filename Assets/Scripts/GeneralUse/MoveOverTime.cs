using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveOverTime : MonoBehaviour
{
    [SerializeField] private Vector3 movement;
    private void Update()
    {
        transform.position += movement * Time.deltaTime;
    }
}
