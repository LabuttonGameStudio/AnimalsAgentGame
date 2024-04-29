using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InteractiveObjectRange : MonoBehaviour
{
    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            ArmadilloInteractController.Instance.OnInteractiveObjectEnterRange();
        }

    }
    public void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            ArmadilloInteractController.Instance.OnInteractiveObjectLeaveRange();
        }
    }
}
