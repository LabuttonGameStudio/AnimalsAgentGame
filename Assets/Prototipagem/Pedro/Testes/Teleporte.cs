using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teleporte : MonoBehaviour
{
    public Transform Destination;
    public Transform player;
    public void OnTriggerEnter(Collider other)
    {
        player.transform.position = Destination.transform.position;
    }
}
