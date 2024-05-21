using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakOnBallCollision : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        if(collision.collider.CompareTag("Player"))
        {
            if (ArmadilloPlayerController.Instance.currentForm == ArmadilloPlayerController.Form.Ball)
            {
                if(collision.impulse.magnitude>400) gameObject.SetActive(false);
            }
        }
    }
}
