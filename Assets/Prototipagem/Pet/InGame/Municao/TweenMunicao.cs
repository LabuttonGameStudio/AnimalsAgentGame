using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using static Tween;

public class TweenMunicao : MonoBehaviour
{
    public static TweenMunicao Instance;

    private Vector2 startPos;
    public RectTransform objectToMove;
    public Vector2 deltaPosition;
    public float moveDuration = 1f;
    [SerializeField] private float moveTranslationDuration;

    private void Awake()
    {
        Instance = this;
    }
    void Start()
    {
        startPos = objectToMove.anchoredPosition;
    }

    public void ShakeAmmunition()
    {
        StartShakeAmmunition();
        //Tween.MoveTransformLocalPosition(this, objectToMove, targetPosition, moveDuration, Tween.LerpType.Lerp);
    }
    private void StartShakeAmmunition()
    {
        if(shakeAmmunition_Ref == null)
        {
            shakeAmmunition_Ref = StartCoroutine(ShakeAmmunition_Coroutine());
        }
        else
        {
            StopCoroutine(shakeAmmunition_Ref);
            shakeAmmunition_Ref = StartCoroutine(ShakeAmmunition_Coroutine());
        }
    }
    private Coroutine shakeAmmunition_Ref;
    private IEnumerator ShakeAmmunition_Coroutine()
    {
        float timer = 0;
        while(timer<=moveDuration)
        {
            MoveRectTransform(this, objectToMove, startPos - deltaPosition, moveTranslationDuration, LerpType.Lerp);
            yield return new WaitForSeconds(moveTranslationDuration);
            timer += moveTranslationDuration;
            MoveRectTransform(this, objectToMove, startPos + deltaPosition, moveTranslationDuration*2, LerpType.Lerp);
            yield return new WaitForSeconds(moveTranslationDuration*2);
            timer += moveTranslationDuration*2;
        }
        MoveRectTransform(this, objectToMove, startPos, moveDuration, LerpType.Lerp);
    }
}
