using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class CinematicEvent
{
    public UnityEvent[] events;
    public float delay;
}

public class Cinematic : MonoBehaviour
{
    [SerializeField]private CinematicEvent[] events;

    public void StartCinematic()
    {

    }
}
