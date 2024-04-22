using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Tween;

public class TweenHP : MonoBehaviour
{
    public static TweenHP Instance;

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
        
    }

    public void RotateIcon()
    {

    }
}
