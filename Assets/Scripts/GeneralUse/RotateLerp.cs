using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateLerp : MonoBehaviour
{
    private Quaternion startRotation;

    [SerializeField] private Vector3 rotateAmount;
    [SerializeField] private float rotationDuration;
    private void Awake()
    {
        startRotation = transform.localRotation;
    }
    public void Rotate()
    {
        StartCoroutine(Rotate_Coroutine());
    }
    private IEnumerator Rotate_Coroutine()
    {
        float timer = 0;
        Quaternion finalRotation = Quaternion.Euler(startRotation.eulerAngles + rotateAmount);
        while(timer<rotationDuration)
        {
            transform.localRotation = Quaternion.Lerp(startRotation, finalRotation, timer / rotationDuration);
            timer += Time.deltaTime;
            yield return null;
        }
        transform.localRotation = finalRotation;
    }
}
