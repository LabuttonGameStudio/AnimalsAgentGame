using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImpactVelocityReceiver : MonoBehaviour
{
    [SerializeField]TextMesh textMesh;
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            textMesh.text=collision.impulse.magnitude.ToString();
        }
    }
}
