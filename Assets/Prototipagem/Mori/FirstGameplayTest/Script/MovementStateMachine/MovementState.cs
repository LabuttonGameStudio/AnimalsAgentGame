using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public abstract class MovementState
{
    public abstract void EnterState(ArmadilloMovementController movementControl);
    public abstract void UpdateState();
    public abstract void FixedUpdateState();
    public abstract void Jump(InputAction.CallbackContext value);
    public abstract void ExitState();
}
