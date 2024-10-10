using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static IPickUpObject;

public class PickUpObject : MonoBehaviour, IPickUpObject
{
    private Rigidbody rb;
    public float objectSize;
    public PickUpObjectType pickUpObjectType;
    public string objectName;
    public string onGroundDescription;
    public string pickedUpDescription;

    private MeshRenderer meshRenderer;
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
        meshRenderer = GetComponent<MeshRenderer>();
        Vector3 centerPos;
        centerPos = meshRenderer.bounds.center;
        Gizmos.DrawWireSphere(centerPos, objectSize);
    }
    private void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        rb = GetComponent<Rigidbody>();
        isBeeingHeld = false;
    }

    public string GetObjectName() { return objectName; }

    public string GetObjectDescription()
    {
        return isBeeingHeld ? pickedUpDescription : onGroundDescription;
    }

    public Vector3 GetObjectDeltaCenter()
    {
        return meshRenderer.bounds.center-rb.position;
    }
}
