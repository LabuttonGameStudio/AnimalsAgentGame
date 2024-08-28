using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using static Tween;

public class TweenHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public RectTransform objectToScale; 
    public float hoverScale = 1.1f; 
    public float normalScale = 1.0f; 
    public float scaleDuration = 0.2f; 
    public Tween.LerpType lerpType = Tween.LerpType.Lerp; 

    
    public void OnPointerEnter(PointerEventData eventData)
    {
        StopAllCoroutines(); 
        StartCoroutine(ScaleUp());
    }

    
    public void OnPointerExit(PointerEventData eventData)
    {
        StopAllCoroutines(); 
        StartCoroutine(ScaleDown());
    }

    private IEnumerator ScaleUp()
    {
        yield return Tween.ScaleTransform(this, objectToScale, Vector3.one * hoverScale, scaleDuration, lerpType);
    }

    private IEnumerator ScaleDown()
    {
        yield return Tween.ScaleTransform(this, objectToScale, Vector3.one * normalScale, scaleDuration, lerpType);
    }
}
