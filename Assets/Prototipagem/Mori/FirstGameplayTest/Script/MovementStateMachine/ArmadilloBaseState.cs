using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ArmadilloBaseState
{
    public abstract void EnterState(ArmadilloMovementController movementControl);
    public abstract void UpdateState();
    public abstract void FixedUpdateState();
    public abstract void ExitState();
}
