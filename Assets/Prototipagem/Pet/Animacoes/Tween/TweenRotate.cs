using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Tween;

public class TweenRotate : MonoBehaviour
{
    public static TweenRotate Instance;

    public RectTransform objectToMove;
    public float rotationSpeed = 90f;

    void Start()
    {
        StartCoroutine(RotateIcon());
    }

    private void Update()
    {
        
    }

    IEnumerator RotateIcon()
    {
        while (true) 
        {
            float anglePerFrame = rotationSpeed * Time.deltaTime;
            objectToMove.Rotate(0f, 0f, anglePerFrame); 

            yield return null;
        }
    }
}
