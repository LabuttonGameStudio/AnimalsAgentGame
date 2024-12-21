using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SandSlowArea : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            OverlayEffects.Instance.ToggleImage(OverlayEffects.Instance.sandOverlay, true);
            ArmadilloPlayerController.Instance.movementControl.speedMultiplier = 0.5f;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            OverlayEffects.Instance.ToggleImage(OverlayEffects.Instance.sandOverlay, false);
            ArmadilloPlayerController.Instance.movementControl.speedMultiplier = 1f;
        }
    }
}
