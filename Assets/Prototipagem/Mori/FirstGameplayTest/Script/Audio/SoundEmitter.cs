using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static SoundGeneralControl;
using AudioType = SoundGeneralControl.AudioType;

public class SoundEmitter : MonoBehaviour
{
    [Tooltip("Apenas para organizacao")][SerializeField] public string audioName;
    [SerializeField] public AudioSource connectedAudioSource;
    [SerializeField] public AudioClip[] audioClips;

    [Header("AudioSource")] private int lastAudioSourceIndexPlayed;

    [Header("Configs")]
    [SerializeField] private bool doesLoop;
    [SerializeField] private float loopInterval;
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
        gizmosColor.r = Mathf.Max(0.25f, gizmosColor.r);
        gizmosColor.g = Mathf.Max(0.25f, gizmosColor.g);
        gizmosColor.b = Mathf.Max(0.25f, gizmosColor.b);
        GizmosExtra.DrawString(audioName, transform.position + Vector3.up * audioRange, gizmosColor);
        Gizmos.color = gizmosColor;
        Gizmos.DrawWireSphere(transform.position, audioRange);
    }
    #endregion

    private void Start()
    {
        if (doesLoop) isOneShot = true;
    }

    public void PlayAudio()
    {
        if (doesLoop)
        {
            if (audioSourceLoop_Ref == null)
            {
                audioSourceLoop_Ref = StartCoroutine(AudioSourceLoop_Coroutine());

            }
            if (audioSphereCastLoop_Ref == null)
            {
                audioSphereCastLoop_Ref = StartCoroutine(AudioSphereCastLoop_Coroutine());
            }
        }
        else
        {
            if (audioType == AudioType.Suspicious) StartCoroutine(CheckForNearbySoundReceivers());
            PlayAudioSource();
        }
    }
    public void StopAudio()
    {
        if (doesLoop)
        {
            if (audioSourceLoop_Ref != null)
            {
                StopCoroutine(audioSourceLoop_Ref);
                audioSourceLoop_Ref = null;
            }
            if (audioSphereCastLoop_Ref != null)
            {
                StopCoroutine(audioSphereCastLoop_Ref);
                audioSphereCastLoop_Ref = null;
            }
            connectedAudioSource.Stop();
        }
    }

    private Coroutine audioSourceLoop_Ref;
    public IEnumerator AudioSourceLoop_Coroutine()
    {
        while (true)
        {
            PlayAudioSource();
            yield return new WaitForSeconds(loopInterval);
        }
    }

    private Coroutine audioSphereCastLoop_Ref;
    public IEnumerator AudioSphereCastLoop_Coroutine()
    {
        while (true)
        {
            yield return StartCoroutine(CheckForNearbySoundReceivers());
            yield return new WaitForSeconds(0.1f);
        }
    }

    private void PlayAudioSource()
    {
        if (audioClips.Length < 2)
        {
            if (!isOneShot)
            {
                connectedAudioSource.Play();

            }
            else
            {
                connectedAudioSource.PlayOneShot(connectedAudioSource.clip);
            }
        }
        else
        {
            if (!isOneShot)
            {
                connectedAudioSource.clip = audioClips[RandomizeAudioSource()];
                connectedAudioSource.Play();

            }
            else
            {
                int randomNumber = RandomizeAudioSource();
                connectedAudioSource.PlayOneShot(audioClips[randomNumber]);
            }
        }
    }
    private int RandomizeAudioSource()
    {

        while (true)
        {
            int randomIndex = Random.Range(0, audioClips.Length);
            if (randomIndex != lastAudioSourceIndexPlayed)
            {
                lastAudioSourceIndexPlayed = randomIndex;
                return randomIndex;
            }
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
        if (connectedAudioSource == null) return;
        connectedAudioSource.maxDistance = audioRange;
    }
}
