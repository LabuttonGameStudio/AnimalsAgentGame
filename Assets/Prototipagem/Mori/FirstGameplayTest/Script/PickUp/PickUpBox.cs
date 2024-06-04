using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PickUpBox : MonoBehaviour, IPickUpObject
{
    [SerializeField] private string objectName;
    [SerializeField] private string onGroundDescription;
    [SerializeField] private string pickedUpDescription;
    public bool isBeeingHeld { get; set; }

    private void Awake()
    {
        isBeeingHeld = false;
    }

    public string GetObjectName() { return objectName; }

    public string GetObjectDescription()
    {
        return isBeeingHeld ? pickedUpDescription : onGroundDescription;
    }
    
}
