using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static SoundGeneralControl;
using AudioType = SoundGeneralControl.AudioType;

public class SoundEmitter : MonoBehaviour
{
    [Tooltip("Apenas para organizacao")][SerializeField] public string audioName;
    [SerializeField] public AudioSource[] connectedAudioSource;

    [Header("AudioSource")] private int lastAudioSourceIndexPlayed;

    [Header("Configs")]
    [Tooltip("If is OneShot can play more than one at a time")][SerializeField] private bool isOneShot;
    [SerializeField] public float audioRange;


    [SerializeField] public AudioType audioType;

    #region Gizmos
    private Color gizmosColor;
    private void OnDrawGizmosSelected()
    {
        Random.InitState(GetInstanceID());
        Color gizmosColor = Random.ColorHSV();
        gizmosColor.a = 1;
        GizmosExtra.DrawString(audioName, transform.position + Vector3.up * audioRange, gizmosColor);
        Gizmos.color = gizmosColor;
        Gizmos.DrawWireSphere(transform.position, audioRange);
    }
    #endregion


    public void PlayAudio()
    {
        if (audioType == AudioType.Suspicious) StartCoroutine(CheckForNearbySoundReceivers());
        PlayAudioSource();

    }
    private void PlayAudioSource()
    {
        if (connectedAudioSource.Length < 2)
        {
            if (!isOneShot)
            {
                connectedAudioSource[0].Play();

            }
            else
            {
                connectedAudioSource[0].PlayOneShot(connectedAudioSource[0].clip);
            }
        }
        else
        {
            if (!isOneShot)
            {
                connectedAudioSource[RandomizeAudioSource()].Play();

            }
            else
            {
                int randomNumber = RandomizeAudioSource();
                connectedAudioSource[randomNumber].PlayOneShot(connectedAudioSource[randomNumber].clip);
            }
        }
    }
    private int RandomizeAudioSource()
    {

        while (true)
        {
            int randomIndex = Random.Range(0, connectedAudioSource.Length);
            if (randomIndex != lastAudioSourceIndexPlayed) return randomIndex;
        }
    }
    public IEnumerator CheckForNearbySoundReceivers()
    {
        Collider[] collidersInZone = Physics.OverlapSphere(transform.position, audioRange, SoundGeneralControl.Instance.soundReceivers_LayerMask);
        foreach (Collider collider in collidersInZone)
        {
            float distancePercentage = 1 - Vector3.Distance(collider.transform.position, transform.position) / audioRange;
            distancePercentage = Mathf.Min(1, distancePercentage);


            if (collider.TryGetComponent(out ISoundReceiver soundReceiver))
            {
                RaycastHit[] objectsInTheMiddle;
                objectsInTheMiddle = Physics.RaycastAll(transform.position, collider.transform.position, float.PositiveInfinity, SoundGeneralControl.Instance.soundOcclusion_LayerMask, QueryTriggerInteraction.Ignore);
                foreach (RaycastHit obj in objectsInTheMiddle)
                {
                    Debug.Log(obj.collider.gameObject.name);
                    distancePercentage -= 0.2f;
                }
                distancePercentage = Mathf.Max(0, distancePercentage);
                soundReceiver.OnSoundHear(new SoundData
                {
                    audioPercentage = distancePercentage,
                    audioType = this.audioType,
                    originPoint = transform.position
                });
            }
            yield return null;
        }
    }
    private void OnValidate()
    {
        if(connectedAudioSource.Length<1)return;
        foreach (AudioSource audioSource in connectedAudioSource)
        {
            if (audioSource == null) return;
            audioSource.maxDistance = audioRange;
        }
    }
}
