using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DeathScreen : MonoBehaviour
{
    //não usar string
    public string sceneInicial;

    public string sceneActual;

    public void ReturninicialScreen()
    {
        Time.timeScale = 1;
        ArmadilloPlayerController.Instance.inputControl.TogglePlayerControls(true);
        SceneManager.LoadScene(sceneInicial);
    }

    public void ReturnGame()
    {
        Time.timeScale = 1;
        ArmadilloPlayerController.Instance.inputControl.TogglePlayerControls(true);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
