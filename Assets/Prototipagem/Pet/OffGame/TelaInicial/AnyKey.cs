using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Tween;

public class AnyKey : MonoBehaviour
{
    public GameObject objectToAnimate;
    public float maxOpacity = 1f;
    public float minOpacity = 0f;
    public float fadeDuration = 1f;
    public Tween.LerpType lerpType;

    private CanvasGroup canvasGroup;

    private void Awake()
    {
        canvasGroup = objectToAnimate.GetComponent<CanvasGroup>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.anyKeyDown)
        {
            StartCoroutine(FadeAnimation());
            
        }
    }

    IEnumerator FadeAnimation()
    {
        float currentOpacity = maxOpacity;

        yield return FadeCanvasGroup(this, canvasGroup, minOpacity, fadeDuration);

        yield return new WaitForSeconds(1f);
        objectToAnimate.SetActive(false);
    }
}