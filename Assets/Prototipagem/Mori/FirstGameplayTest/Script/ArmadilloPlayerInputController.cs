using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class ArmadilloPlayerInputController : MonoBehaviour
{
    public PlayerInputAction inputAction;

    private SensibilityModifier zeroSensibilityModifier;
    private void Awake()
    {
        inputAction = new PlayerInputAction();
        zeroSensibilityModifier = new SensibilityModifier(0);
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
    public void TogglePlayerControls(bool toggle)
    {
        if (toggle)
        {
            inputAction.Armadillo.Enable();
            ArmadilloPlayerController.Instance.cameraControl.RemoveSensibilityModifier(zeroSensibilityModifier);
        }
        else
        {
            inputAction.Armadillo.Disable();
            ArmadilloPlayerController.Instance.cameraControl.AddSensibilityModifier(zeroSensibilityModifier);
        }
    }
    public void ToggleDialogueControls(bool toggle)
    {
        if (toggle)
        {
            inputAction.Dialogue.Enable();
        }
        else inputAction.Dialogue.Disable();
    }
    public void ToggleChangeWeapon(bool state)
    {
        if (state)
        {
            inputAction.Armadillo.Weapon1.Enable();
            inputAction.Armadillo.Weapon1.Enable();
            inputAction.Armadillo.Weapon2.Enable();
        }
        else
        {
            inputAction.Armadillo.Weapon1.Disable();
            inputAction.Armadillo.Weapon1.Disable();
            inputAction.Armadillo.Weapon2.Disable();
        }
    } 
}
