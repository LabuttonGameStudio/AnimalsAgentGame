using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundEmitter : MonoBehaviour
{
    [Tooltip("Apenas para organizacao")][SerializeField] private string audioName;
    [SerializeField] private AudioSource connectedAudioSource;

    [Header("Configs")]
    [SerializeField]private bool canPlayMoreThenOnceAtATime;
}
