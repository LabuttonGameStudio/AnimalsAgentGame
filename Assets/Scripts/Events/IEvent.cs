using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class IEvent : MonoBehaviour
{
    [SerializeField] protected bool isSingleUse;
    protected bool wasUsed;
    [SerializeField] public UnityEvent consequences;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) consequences.Invoke();
    }
}
