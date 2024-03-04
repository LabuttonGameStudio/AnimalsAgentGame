using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CameraBaseState
{
    public abstract void EnterState(PlayerCamera playerCamera);
    public abstract void UpdateState();
    public abstract void ExitState();
}
