using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public interface IRequirements
{
    public INeedRequirements connectedObject
    {
        get;
        set;
    }
    public bool isTurnedOn
    {
        get;
        set;
    }
    public void DefineConnectedObject(INeedRequirements INR)
    {
        connectedObject = INR;
    }
}
