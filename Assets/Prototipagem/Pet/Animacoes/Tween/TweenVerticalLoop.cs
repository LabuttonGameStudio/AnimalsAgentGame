using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Tween;

public class TweenVerticalLoop : MonoBehaviour
{
    public RectTransform objectToMove;
    public float moveDistance = 100f; 
    public float moveDuration = 1f; 
    public Tween.LerpType lerpType;

    private Vector2 startPos;

    void Start()
    {
        startPos = objectToMove.anchoredPosition; 
        StartCoroutine(MoveVertical());
    }

    IEnumerator MoveVertical()
    {
        Vector2 finalPosition = startPos + Vector2.up * moveDistance; 

        while (enabled) 
        {
            yield return Tween.MoveRectTransform(this, objectToMove, finalPosition, moveDuration, lerpType);

            yield return Tween.MoveRectTransform(this, objectToMove, startPos, moveDuration, lerpType);
        }
    }
}
