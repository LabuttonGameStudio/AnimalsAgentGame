using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundEmitter : MonoBehaviour
{
    [Tooltip("Apenas para organizacao")][SerializeField] public string audioName;
    [SerializeField] public AudioSource connectedAudioSource;

    [Header("Configs")]
    [Tooltip("If is OneShot can play more than one at a time")][SerializeField]private bool isOneShot;
    [SerializeField] public float audioRange;

    public enum AudioType
    {
        Ambient,
        Suspicious

    }

    [SerializeField] public AudioType audioType;

    #region Gizmos
    private Color gizmosColor;
    private void OnDrawGizmosSelected()
    {
        Random.InitState(GetInstanceID());
        Color gizmosColor = Random.ColorHSV();
        gizmosColor.a = 1;
        GizmosExtra.DrawString(audioName, transform.position + Vector3.up * audioRange,gizmosColor);
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
        Collider[] collidersInZone =Physics.OverlapSphere(transform.position, audioRange,SoundGeneralControl.Instance.soundReceiversLayerMask);
        foreach(Collider collider in collidersInZone)
        {
            float distancePercentage = Vector3.Distance(collider.transform.position, transform.position) / audioRange;
            distancePercentage = Mathf.Max(0, distancePercentage);
            distancePercentage = Mathf.Min(1, distancePercentage);
            Debug.Log(collider.gameObject.name +"| Distance ="+ distancePercentage);
            yield return null;
        }
    }
    private void OnValidate()
    {
        connectedAudioSource.maxDistance = audioRange;
    }
}
