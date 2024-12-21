using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Tween;

public class TweenPopUpManual : MonoBehaviour
{
    public RectTransform objectToScale; 
    public float scaleDuration = 0.5f; 
    public Tween.LerpType lerpType = Tween.LerpType.Lerp; 

    void Start()
    {
        objectToScale.localScale = Vector3.zero;
    }

    public void StartScaleUP()
    { 
        StartCoroutine(ScaleUp());
    }

    public void StartScaleDown()
    {
        StartCoroutine(ScaleDown());
    }
    IEnumerator ScaleUp()
    {
        yield return new WaitForSeconds(0.6f);
        Vector3 initialScale = Vector3.zero;
        Vector3 midScale = new Vector3(1.2f, 1.2f, 1.2f);
        Vector3 finalScale = Vector3.one;

        //Aumenta de 0 ate 1.2
        yield return Tween.ScaleTransform(this, objectToScale, midScale, scaleDuration / 2, lerpType);

        //Reduz de 1.2 ate 1.0
        yield return Tween.ScaleTransform(this, objectToScale, finalScale, scaleDuration / 2, lerpType);
    }

    IEnumerator ScaleDown()
    {
        Vector3 initialScale = Vector3.one;
        Vector3 midScale = new Vector3(1.2f, 1.2f, 1.2f);
        Vector3 finalScale = Vector3.zero;

        //Aumenta de 0 ate 1.2
        yield return Tween.ScaleTransform(this, objectToScale, midScale, scaleDuration / 2, lerpType);

        //Reduz de 1.2 ate 1.0
        yield return Tween.ScaleTransform(this, objectToScale, finalScale, scaleDuration / 2, lerpType);
    }
}
