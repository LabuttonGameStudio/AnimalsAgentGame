using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFirstPersonState : CameraBaseState
{
    private PlayerCamera playerCamera;
    public override void EnterState(PlayerCamera playerCmr)
    {
        playerCamera = playerCmr;
    }

    public override void ExitState()
    {
       
    }

    public override void FixedUpdateState()
    {
       
    }

    public override void UpdateState()
    {
        
    }
}
