using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public interface IPickUpObject : IRaycastableInLOS
{
    void IRaycastableInLOS.OnEnterLOS()
    {
        //ArmadilloPlayerController.Instance.pickUpControl.AddToInteractButtonAction(this);
    }

    void IRaycastableInLOS.OnLeaveLOS()
    {

       // ArmadilloPlayerController.Instance.pickUpControl.ClearInteractButtonAction();
    }
    public abstract string GetObjectName();

    public abstract string GetObjectDescription();
}
