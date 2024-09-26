using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TweenPopUpInfo : MonoBehaviour
{
    public RectTransform objectToScale;
    public float scaleDuration = 0.5f;
    public float TimeForStart = 0f;
    public Tween.LerpType lerpType = Tween.LerpType.Lerp;

    void Start()
    {
        objectToScale.localScale = Vector3.zero;
        StartCoroutine(ScaleUp());
    }

    void Update()
    {
        // Verifica se o objeto est� escalado em Vector3.one e se o jogador pressiona Esc
        if (objectToScale.localScale == Vector3.one && Input.GetKeyDown(KeyCode.Escape))
        {
            StartScaleDown();
        }
    }

    public void StartScaleDown()
    {
        StartCoroutine(ScaleDown());
    }

    public void StartScaleUP()
    {
        StartCoroutine(ScaleUp());
    }

    IEnumerator ScaleUp()
    {
        yield return new WaitForSeconds(TimeForStart);

        Vector3 initialScale = Vector3.zero;
        Vector3 midScale = new Vector3(1.2f, 1.2f, 1.2f);
        Vector3 finalScale = Vector3.one;

        //Aumenta de 0 atea1.2
        yield return Tween.ScaleTransform(this, objectToScale, midScale, scaleDuration / 2, lerpType);

        //Reduz de 1.2 ataa 1.0
        yield return Tween.ScaleTransform(this, objectToScale, finalScale, scaleDuration / 2, lerpType);
    }

    IEnumerator ScaleDown()
    {
        Vector3 initialScale = Vector3.one;
        Vector3 midScale = new Vector3(1.2f, 1.2f, 1.2f);
        Vector3 finalScale = Vector3.zero;

        // Aumenta de 0 atas 1.2
        yield return Tween.ScaleTransform(this, objectToScale, midScale, scaleDuration / 2, lerpType);

        // Reduz de 1.2 ats 1.0
        yield return Tween.ScaleTransform(this, objectToScale, finalScale, scaleDuration / 2, lerpType);

        yield return new WaitForSeconds(1f);

        gameObject.SetActive(false);
    }


}