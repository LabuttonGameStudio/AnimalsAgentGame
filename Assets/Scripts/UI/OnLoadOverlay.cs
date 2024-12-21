using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OnLoadOverlay : MonoBehaviour
{
    Image image;
    private void Awake()
    {
        image = GetComponent<Image>();
    }
    private void Start()
    {
        StartCoroutine(OnLoad_Coroutine());
    }
    private IEnumerator OnLoad_Coroutine()
    {
        float timer =0;
        float duration = 0.5f;
        image.color = new Color(image.color.r, image.color.g, image.color.b, 1);

        while (timer < duration)
        {
            image.color = new Color(image.color.r,image.color.g,image.color.b,1-timer/duration);
            yield return null;
            timer += Time.deltaTime;
        }
        image.color = new Color(image.color.r, image.color.g, image.color.b, 0);
    }
}
