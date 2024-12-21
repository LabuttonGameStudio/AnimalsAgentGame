using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CheckPointArea : MonoBehaviour
{
    private bool wasSaved;
    public Transform returnPoint;

    [SerializeField]public UnityEvent onLoadIntoCheckpoint;
    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player") && !wasSaved)
        {
            wasSaved = true;
            ArmadilloPlayerController.Instance.SavePlayerData();
            CheckPointManager.Instance.SaveCheckPoint(this);
        }
    }
}
