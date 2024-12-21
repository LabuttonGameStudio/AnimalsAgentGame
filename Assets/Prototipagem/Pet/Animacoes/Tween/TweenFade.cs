using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Tween;

public class TweenFade : MonoBehaviour
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
    }

    public void StartFade()
    {
        StartCoroutine(FadeAnimation());
    }
    IEnumerator FadeAnimation()
    {
        float currentOpacity = maxOpacity;
        yield return FadeCanvasGroup(this, canvasGroup, maxOpacity, fadeDuration);
       
    }
}
