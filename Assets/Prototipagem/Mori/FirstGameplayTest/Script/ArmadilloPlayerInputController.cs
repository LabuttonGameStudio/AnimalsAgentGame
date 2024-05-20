using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class ArmadilloPlayerInputController : MonoBehaviour
{
    public PlayerInputAction inputAction;
    private void Awake()
    {
        inputAction = new PlayerInputAction();
    }
    private void Start()
    {
        //Debug_ResetLevel
        inputAction.Armadillo.Debug_Reset.Enable();
        inputAction.Armadillo.Debug_Reset.performed += ResetLevel;
    }
    private void ResetLevel(InputAction.CallbackContext value)
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
