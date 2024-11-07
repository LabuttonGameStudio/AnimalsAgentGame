using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering;

public class BossManager : MonoBehaviour
{
    [Foldout("Components", styled = true)]
    [SerializeField]private Transform cageCapsule;
    [SerializeField]private Transform bossTransform;
    [SerializeField]private Animator bossAnimator;
    [SerializeField]private Volume secondPhaseVolume;
    [Foldout("Second Phase",styled = true)]

    [SerializeField] private UnityEvent onEnterSecondPhaseEvent;
    [SerializeField] private UnityEvent onEndSecondPhaseEvent;
    public void ChangeToSecondPhase()
    {
        StartCoroutine(ChangeToSecondPhase_Coroutine());
    }
    private IEnumerator ChangeToSecondPhase_Coroutine()
    {
        float timer = 0;
        float duration = 1.25f;
        
        Vector3 startPos = cageCapsule.position;
        while (timer < duration)
        {
            cageCapsule.transform.localPosition = Vector3.Lerp(startPos, startPos - Vector3.up * 9, timer / duration);
            timer += Time.deltaTime;
            yield return null;
        }
        cageCapsule.transform.localPosition = startPos - Vector3.up * 9;
        timer = 0;

        onEnterSecondPhaseEvent.Invoke();

        duration = 0.25f;
        while (timer < duration)
        {
            secondPhaseVolume.weight = timer / duration;
            timer += Time.deltaTime;
            yield return null;
        }
        secondPhaseVolume.weight = 1;
        timer = 0;

        duration = 1.25f;
        startPos = bossTransform.localPosition;
        while (timer < duration)
        {
            bossTransform.localPosition = Vector3.Lerp(startPos, startPos + Vector3.up * 9, timer / duration);
            timer += Time.deltaTime;
            yield return null;
        }
        bossTransform.localPosition = startPos + Vector3.up * 9;


        onEndSecondPhaseEvent.Invoke();
    }
}
