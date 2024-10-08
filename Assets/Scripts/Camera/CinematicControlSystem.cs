using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class CinematicEventTimeStamp
{
    public UnityEvent events;
    public float delay;
}

public class CinematicControlSystem : MonoBehaviour
{
    [SerializeField]private CinematicEventTimeStamp[] eventSequence;
    private bool startOnPlay;

    private void Start()
    {
        if (startOnPlay)StartCinematic();
    }
    public void StartCinematic()
    {
        if(cinematic_Ref != null)
        {
            StopCoroutine(cinematic_Ref);
        }
        cinematic_Ref = StartCoroutine(Cinematic_Coroutine());
    }

    private Coroutine cinematic_Ref;
    private IEnumerator Cinematic_Coroutine()
    {
        for(int i = 0; i < eventSequence.Length; i++)
        {
            eventSequence[i].events.Invoke();
            yield return new WaitForSeconds(eventSequence[i].delay);
        }
    }
}
