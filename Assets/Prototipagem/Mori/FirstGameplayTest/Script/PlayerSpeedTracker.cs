using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerSpeedTracker : MonoBehaviour
{
    [SerializeField]Rigidbody rb;
    private TextMeshProUGUI speedTracker;
    private void Awake()
    {
        speedTracker = GetComponent<TextMeshProUGUI>();
    }
    private void Update()
    {
        float playerSpeed = rb.velocity.magnitude;
        playerSpeed=Mathf.Round(playerSpeed * 10000)/10000;
        speedTracker.text = playerSpeed.ToString();
    }
}
