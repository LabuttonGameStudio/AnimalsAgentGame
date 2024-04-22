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
        SceneManager.LoadScene(sceneInicial);
    }

    public void ReturnGame()
    {
        SceneManager.LoadScene(sceneActual);
    }
}
