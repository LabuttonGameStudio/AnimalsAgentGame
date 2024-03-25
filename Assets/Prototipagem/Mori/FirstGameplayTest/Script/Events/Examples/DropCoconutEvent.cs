using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DropCoconutEvent : IEvent
{
    Transform coconut;
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
