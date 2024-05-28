using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static SoundGeneralControl;
using AudioType = SoundGeneralControl.AudioType;

public class SoundEmitter : MonoBehaviour
{
    [Tooltip("Apenas para organizacao")][SerializeField] public string audioName;
    [SerializeField] public AudioSource connectedAudioSource;

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
        StartCoroutine(CheckForNearbySoundReceivers());
    }

    public IEnumerator CheckForNearbySoundReceivers()
    {
        Collider[] collidersInZone = Physics.OverlapSphere(transform.position, audioRange, SoundGeneralControl.Instance.soundReceivers_LayerMask);
        foreach (Collider collider in collidersInZone)
        {
            float distancePercentage = 1 - Vector3.Distance(collider.transform.position, transform.position) / audioRange;
            distancePercentage = Mathf.Min(1, distancePercentage);


            if (collider.TryGetComponent(out SoundReceiver soundReceiver))
            {
                RaycastHit[] objectsInTheMiddle;
                objectsInTheMiddle = Physics.RaycastAll(transform.position, collider.transform.position, float.PositiveInfinity, SoundGeneralControl.Instance.soundOcclusion_LayerMask, QueryTriggerInteraction.Ignore);
                Debug.Log(objectsInTheMiddle.Length);
                foreach(RaycastHit obj in objectsInTheMiddle)
                {
                    Debug.Log(obj.collider.gameObject.name);
                    distancePercentage -= 0.2f;
                }
                distancePercentage = Mathf.Max(0, distancePercentage);
                soundReceiver.OnSoundHear(new SoundData
                {
                    audioPercentage = distancePercentage,
                    audioType = this.audioType
                });
            }
            yield return null;
        }
    }
    private void OnValidate()
    {
        connectedAudioSource.maxDistance = audioRange;
    }
}
