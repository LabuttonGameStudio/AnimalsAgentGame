using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioDebugger : MonoBehaviour
{
    [SerializeField] private bool play;

    AudioSource audioSource;
    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }
    public void Update()
    {
        if(play)
        {
            audioSource.Play();
            play = false;
        }
    }
}
