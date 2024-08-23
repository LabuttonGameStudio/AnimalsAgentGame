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
        proUGUI.text = "FPS=" + Mathf.Round(count);
    }
    private IEnumerator Start()
    {
        GUI.depth = 2;
        while (true)
        {
            count = 1f / Time.unscaledDeltaTime;
            yield return new WaitForSeconds(0.1f);
        }
    }
}
