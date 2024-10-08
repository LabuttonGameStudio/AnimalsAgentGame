using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class InteractiveEvent : IEvent
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if(isSingleUse)
            {
                if (wasUsed) return;
                wasUsed = true;
            }
            consequences.Invoke();
        }
    }
}
