using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class InitialOptions : MonoBehaviour
{

    //nao utilizar string
    public string levelToLoad;
    public string loadingScreenScene;
    public string actuallevel;

    public GameObject Cinemachine1;

    private bool levelLoaded = false;
    public float delayBeforeLoading = 10f;

    public void iniciar()
    {
        // save game

        if (!levelLoaded) //  level não carregado
        {
            StartCoroutine(CinemachineRun());
            StartCoroutine(LoadLevelAsync());
            Debug.Log("Iniciando");
            levelLoaded = true;
        }

    }

    //verificar dps
    IEnumerator LoadLevelAsync()
    {

        //salvar game

        yield return new WaitForSeconds(5f);

        // Obtém a referência para a cena atual
        Scene currentScene = SceneManager.GetActiveScene();

        // Descarrega a cena atual para evitar lags e bugs
        SceneManager.UnloadSceneAsync(currentScene);

        // Carrega a cena da tela de carregamento
        yield return SceneManager.LoadSceneAsync(loadingScreenScene, LoadSceneMode.Additive);

        // Espera um tempo antes de carregar a próxima cena
        yield return new WaitForSeconds(delayBeforeLoading);

        // Carrega a próxima cena de forma assíncrona
        AsyncOperation levelLoadOperation = SceneManager.LoadSceneAsync(levelToLoad, LoadSceneMode.Additive);
        levelLoadOperation.allowSceneActivation = false;

        // Aguarda até que a próxima cena esteja pronta para ser ativada
        while (!levelLoadOperation.isDone)
        {
            if (levelLoadOperation.progress >= 0.9f)
            {
                // Ativa a próxima cena
                levelLoadOperation.allowSceneActivation = true;
            }
            yield return null;
        }

        // Descarrega a cena da tela de carregamento
        SceneManager.UnloadSceneAsync(loadingScreenScene);
        SceneManager.UnloadSceneAsync(actuallevel);
    }

    //IEnumerator WallDown()
    //{
        
   // }

    IEnumerator CinemachineRun()
    {
        Cinemachine1.SetActive(false);
        yield return null;
       
    }

    public void opcoes()
    {
        // i don't now
    }

}
