using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IRaycastableInLOS 
{
    public abstract void OnEnterLOS();
    public abstract void OnLeaveLOS();
}
