using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.Localization;
using static IPickUpObject;

public class PickUpObject : MonoBehaviour, IPickUpObject
{
    private Rigidbody rb;
    public float objectSize;
    public PickUpObjectType pickUpObjectType;

    public LocalizedString objectName;
    public LocalizedString onGroundDescription;
    public LocalizedString pickedUpDescription;

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

    public UnityEvent onPickUpEvent;
    public UnityEvent m_OnPickUpEvent
    {
        get => onPickUpEvent;
        set => onPickUpEvent = value;
    }

    public UnityEvent onDropEvent;
    public UnityEvent m_OnDropEvent
    {
        get => onPickUpEvent;
        set => onPickUpEvent = value;
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

    public string GetObjectName() { return objectName.GetLocalizedString(); }

    public string GetObjectDescription()
    {
        return isBeeingHeld ? pickedUpDescription.GetLocalizedString() : onGroundDescription.GetLocalizedString();
    }

    public Vector3 GetObjectDeltaCenter()
    {
        return meshRenderer.bounds.center - rb.position;
    }
}
