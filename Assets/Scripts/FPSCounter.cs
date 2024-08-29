using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FPSCounter : MonoBehaviour
{
    TextMeshProUGUI proUGUI;
    private void Awake()
    {
        proUGUI = GetComponent<TextMeshProUGUI>();
    }
    private float count;
    private void Update()
    {
        proUGUI.text = "FPS=" + Mathf.Round(count)+"<br>";
        proUGUI.text += "Average FPS=" + currentAvgFPS;
    }
    private IEnumerator Start()
    {
        while (true)
        {
            count = 1f / Time.unscaledDeltaTime;
            UpdateCumulativeMovingAverageFPS(count);
            yield return new WaitForSeconds(0.1f);
        }
    }
    int qty = 0;
    float currentAvgFPS = 0;
    float UpdateCumulativeMovingAverageFPS(float newFPS)
    {
        ++qty;
        currentAvgFPS += Mathf.Round((newFPS - currentAvgFPS) / qty);

        return currentAvgFPS;
    }
}
