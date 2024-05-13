using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Tween;

public class TweenVertHoriLoop : MonoBehaviour
{
    public RectTransform objectToMove;
    public float moveDistanceHorizontal = 50f; 
    //public float moveDistanceVertical = 50f;
    public float moveDuration = 1f;
    public Tween.LerpType lerpType; 

    private Vector2 startPos;

    void Start()
    {
        startPos = objectToMove.anchoredPosition; 
        StartCoroutine(MovementAnimationLoop());
    }

    IEnumerator MovementAnimationLoop()
    {
        while (true) 
        {
            Vector2 finalPositionHorizontal = startPos + Vector2.right * moveDistanceHorizontal;
            //Vector2 finalPositionVertical = startPos + Vector2.up * moveDistanceVertical;

            yield return MoveRectTransform(this, objectToMove, finalPositionHorizontal, moveDuration, lerpType);

            // yield return MoveRectTransform(this, objectToMove, finalPositionVertical, moveDuration, lerpType);

            yield return MoveRectTransform(this, objectToMove, startPos, moveDuration, lerpType);
        }
    }
}
