using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlipOverTime : MonoBehaviour
{
    RectTransform rectTransform;
    [SerializeField] float intervalInSeconds;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }
    private void OnEnable()
    {
        StartCoroutine(Flip_Coroutine());
    }
    IEnumerator Flip_Coroutine()
    {
        while (true)
        {
            rectTransform.localRotation = Quaternion.Euler(rectTransform.localRotation.eulerAngles.x, 0, rectTransform.localRotation.eulerAngles.z);
            yield return new WaitForSeconds(intervalInSeconds);
            rectTransform.localRotation = Quaternion.Euler(rectTransform.localRotation.eulerAngles.x, 180, rectTransform.localRotation.eulerAngles.z);
            yield return new WaitForSeconds(intervalInSeconds);
        }

    }
}
