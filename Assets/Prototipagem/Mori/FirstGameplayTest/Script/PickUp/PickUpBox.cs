using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static IPickUpObject;

public class PickUpBox : MonoBehaviour, IPickUpObject
{
    [SerializeField]private PickUpObjectType pickUpObjectType;
    [SerializeField] private string objectName;
    [SerializeField] private string onGroundDescription;
    [SerializeField] private string pickedUpDescription;
    public bool isBeeingHeld { get; set; }
    public PickUpObjectType m_pickUpObjectType
    {
        get => pickUpObjectType;
        set => pickUpObjectType = value;
    }

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
