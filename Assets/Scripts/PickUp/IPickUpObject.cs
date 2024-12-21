using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

public interface IPickUpObject : IRaycastableInLOS
{
    public float m_objectSize { get; set; }
    public enum PickUpObjectType
    {
        Small,
        Medium,
        Big
    }
    public PickUpObjectType m_pickUpObjectType { get; set; }
    void IRaycastableInLOS.OnEnterLOS()
    {
        ArmadilloPickUpControl pickUpControl = ArmadilloPlayerController.Instance.pickUpControl;
        pickUpControl.OnObjectEnterVision(this);
    }

    void IRaycastableInLOS.OnLeaveLOS()
    {
        ArmadilloPickUpControl pickUpControl = ArmadilloPlayerController.Instance.pickUpControl;
        pickUpControl.OnObjectLeaveVision(this);
    }
    bool isBeeingHeld { get; set; }
    public abstract string GetObjectName();

    public abstract string GetObjectDescription();
    public abstract Vector3 GetObjectDeltaCenter();
    UnityEvent m_OnPickUpEvent { get; set; }
    UnityEvent m_OnDropEvent { get; set; }

    public void ToggleHold(bool state)
    {
        if (state) m_OnPickUpEvent.Invoke();
        else m_OnDropEvent.Invoke();
    }
}
