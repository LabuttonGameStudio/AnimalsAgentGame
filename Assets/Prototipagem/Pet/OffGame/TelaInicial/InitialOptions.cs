using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class InitialOptions : MonoBehaviour
{

    [Header("LEVEL AND LOAD")]
    public string levelToLoad;
    public string loadingScreenScene;
    public string actuallevel;
    private bool levelLoaded = false;
    public float delayBeforeLoading = 10f;


    public void SaveAndLoad()
    {
        // save game

        if (!levelLoaded) //  level nao carregado
        {
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

        // obtem a referência para a cena atual
        Scene currentScene = SceneManager.GetActiveScene();

        // descarrega a cena atual para evitar lags e bugs
        SceneManager.UnloadSceneAsync(currentScene);

        // carrega a cena da tela de carregamento
        yield return SceneManager.LoadSceneAsync(loadingScreenScene, LoadSceneMode.Additive);

        // Epera um tempo antes de carregar a próxima cena
        yield return new WaitForSeconds(delayBeforeLoading);

        // carrega a proxima cena de forma assincrona
        AsyncOperation levelLoadOperation = SceneManager.LoadSceneAsync(levelToLoad, LoadSceneMode.Additive);
        levelLoadOperation.allowSceneActivation = false;

        // aguarda ate que a proxima cena esteja pronta para ser ativada
        while (!levelLoadOperation.isDone)
        {
            if (levelLoadOperation.progress >= 0.9f)
            {
                // Ativa a proxima cena
                levelLoadOperation.allowSceneActivation = true;
            }
            yield return null;
        }

        // Descarrega a cena da tela de carregamento
        SceneManager.UnloadSceneAsync(loadingScreenScene);
        SceneManager.UnloadSceneAsync(actuallevel);
    }

    public void Exit()
    {
        Application.Quit();
    }

}


