using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmadilloPlayerInputController : MonoBehaviour
{
    public PlayerInputAction inputAction;
    private void Awake()
    {
        inputAction = new PlayerInputAction();
    }
}
