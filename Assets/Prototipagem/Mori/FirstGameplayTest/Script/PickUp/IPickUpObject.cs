using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public interface IPickUpObject : IRaycastableInLOS
{
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
}
