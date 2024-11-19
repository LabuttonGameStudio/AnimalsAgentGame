using Pixeye.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering;

public class BossManager : MonoBehaviour
{
    public static BossManager Instance;
    [Foldout("Components", true)]
    [SerializeField] private Transform cageCapsule;
    [SerializeField] private Transform bossTransform;
    [SerializeField] private Animator cageAnimator;
    [SerializeField] private EnemyBoss bossScript;
    [SerializeField] private Animator bossAnimator;
    [SerializeField] private Volume secondPhaseVolume;
    [Foldout("Second Phase Events", true)]
    [SerializeField] private UnityEvent onCapsuleComplete;
    [SerializeField] private UnityEvent onCapsuleNoLongerVisible;
    [SerializeField] private UnityEvent onEndSecondPhaseEvent;

    private void Awake()
    {
        Instance = this;
    }
    public void ChangeToSecondPhase()
    {
        StartCoroutine(ChangeToSecondPhase_Coroutine());
    }
    private IEnumerator ChangeToSecondPhase_Coroutine()
    {
        float timer = 0;
        float duration = 3.25f;
        onCapsuleComplete.Invoke();
        cageAnimator.SetBool("lidIsOpen", true);
        yield return new WaitForSeconds(4f);


        Vector3 startPos = cageCapsule.position;
        FPCameraShake.StartShake(duration, 0.2f, 8);
        while (timer < duration)
        {
            cageCapsule.transform.localPosition = Vector3.Lerp(startPos, startPos - Vector3.up * 9, timer / duration);
            timer += Time.deltaTime;
            yield return null;
        }
        cageCapsule.transform.localPosition = startPos - Vector3.up * 7;
        cageCapsule.gameObject.SetActive(false);
        timer = 0;

        onCapsuleNoLongerVisible.Invoke();

        duration = 0.25f;
        while (timer < duration)
        {
            secondPhaseVolume.weight = timer / duration;
            timer += Time.deltaTime;
            yield return null;
        }
        secondPhaseVolume.weight = 1;
        timer = 0;

        onEndSecondPhaseEvent.Invoke();
        yield return new WaitForSeconds(1f);
        bossScript.actionEnum = EnemyBoss.ActionsEnum.Enter;
        bossScript.transform.parent.gameObject.SetActive(true);
        duration = 3.25f;
        FPCameraShake.StartShake(duration, 0.2f, 8);
        bossAnimator.SetBool("bossIntroductionEnter", true);
        startPos = bossTransform.localPosition;
        while (timer < duration)
        {
            bossTransform.localPosition = Vector3.Lerp(startPos, startPos + Vector3.up * 9, timer / duration);
            bossScript.transform.localPosition = Vector3.zero;
            timer += Time.deltaTime;
            yield return null;
        }
        bossTransform.localPosition = startPos + Vector3.up * 9;
        bossScript.transform.localPosition = Vector3.zero;
        bossAnimator.SetBool("bossIntroductionEnter", false);
        yield return new WaitForSeconds(2f);
        FPCameraShake.StartShake(1.2f, 0.2f, 8);
        yield return new WaitForSeconds(1.2f);
        bossScript.actionEnum = EnemyBoss.ActionsEnum.Idle;
    }
}
