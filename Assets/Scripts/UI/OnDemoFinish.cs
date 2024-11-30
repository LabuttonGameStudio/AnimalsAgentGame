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
        ArmadilloPlayerController.Instance.inputControl.TogglePlayerControls(false);
        ArmadilloPlayerController.Instance.inputControl.TogglePauseControls(false);
        float timer =0f;
        float duration = 3f;

        while(timer < duration)
        {
            Time.timeScale = timer / duration;
            canvasGroup.alpha = timer/duration;
            timer += Time.unscaledDeltaTime;
            yield return null;
        }
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        Time.timeScale = 0;
        canvasGroup.alpha = 1;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
    }
}
