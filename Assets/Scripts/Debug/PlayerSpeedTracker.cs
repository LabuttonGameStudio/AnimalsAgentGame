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
        float playerSpeed = new Vector2(rb.linearVelocity.x,rb.linearVelocity.z).magnitude;
        playerSpeed=Mathf.Round(playerSpeed * 100)/100;
        speedTracker.text = playerSpeed.ToString();
    }
}
