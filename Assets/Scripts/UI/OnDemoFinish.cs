using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnDemoFinish : MonoBehaviour
{
    private CanvasGroup canvasGroup;

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
    }

    public void EnterEndScreen()
    {
        StartCoroutine(EnterEndScreen_Coroutine());
    }
    private IEnumerator EnterEndScreen_Coroutine()
    {
        float timer =0f;
        float duration = 1f;

        while(timer < duration)
        {
            canvasGroup.alpha = timer/duration;
            timer += Time.deltaTime;
            yield return null;
        }
        canvasGroup.alpha = 1;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
    }
}
