using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SandSlowArea : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            ArmadilloPlayerController.Instance.movementControl.speedMultiplier = 0.25f;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            ArmadilloPlayerController.Instance.movementControl.speedMultiplier = 1f;
        }
    }
}
