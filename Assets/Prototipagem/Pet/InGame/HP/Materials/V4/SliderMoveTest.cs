using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SliderMoveTest : MonoBehaviour
{
    public Slider slider; 
    public float duration = 5f; // tempo para valor maax
    public float waitTime = 2f; // espera no avlor max

    private float currentTime = 0f;
    private bool isWaiting = false;

    void Start()
    {
        slider.value = 0f;
    }


    void Update()
    {
        if (!isWaiting)
        {
            currentTime += Time.deltaTime;
            slider.value = Mathf.Lerp(0f, slider.maxValue, currentTime / duration);


            if (slider.value >= slider.maxValue)
            {
                isWaiting = true; // tempo de espera
                currentTime = 0f; // reset
            }
        }
        else
        {
            // tempo de spra
            currentTime += Time.deltaTime;
            if (currentTime >= waitTime)
            {
                slider.value = 0f; 
                currentTime = 0f; 
                isWaiting = false; 
            }
        }
    }
}
