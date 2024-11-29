using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DeathScreen : MonoBehaviour
{
    public void ReturninicialScreen()
    {
        Time.timeScale = 1;
        ArmadilloPlayerController.Instance.inputControl.TogglePlayerControls(true);
        SceneManager.LoadScene((int)SceneController.ScenesEnum.MenuPause);
    }

    public void ReturnGame()
    {
        Time.timeScale = 1;
        ArmadilloPlayerController.Instance.inputControl.TogglePlayerControls(true);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
