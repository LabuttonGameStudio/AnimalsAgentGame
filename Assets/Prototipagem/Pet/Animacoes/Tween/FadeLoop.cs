using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Tween;

public class FadeLoop : MonoBehaviour
{
    public RectTransform objectToAnimate;
    public float maxOpacity = 1f; 
    public float minOpacity = 0f; 
    public float fadeDuration = 1f;
    public Tween.LerpType lerpType; 

    private CanvasGroup canvasGroup;

    void Start()
    {
        canvasGroup = objectToAnimate.GetComponent<CanvasGroup>(); 
        StartCoroutine(FadeAnimationLoop());
    }

    IEnumerator FadeAnimationLoop()
    {
        float currentOpacity = maxOpacity; 

        while (true) // Loop infinito para manter a animacao em loop
        {
            yield return FadeCanvasGroup(this, canvasGroup, minOpacity, fadeDuration);

            yield return FadeCanvasGroup(this, canvasGroup, maxOpacity, fadeDuration);
        }
    }
}
