using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadingScreen : MonoBehaviour
{
    public static LoadingScreen Instance;

    public Slider loadingSlider;
    private void Awake()
    {
        Instance = this;
    }
}
