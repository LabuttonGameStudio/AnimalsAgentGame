using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static IPickUpObject;

public class PickUpBox : MonoBehaviour, IPickUpObject
{
    private Rigidbody rb;
    [SerializeField]private float objectSize;
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
    public float m_objectSize 
    {
        get => objectSize;
        set => objectSize = value;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, objectSize);
    }
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        isBeeingHeld = false;
    }

    public string GetObjectName() { return objectName; }

    public string GetObjectDescription()
    {
        return isBeeingHeld ? pickedUpDescription : onGroundDescription;
    }
    private void FixedUpdate()
    {
        rb.AddForce(Vector3.up*(-1)*10f,ForceMode.Acceleration);
    }
}
