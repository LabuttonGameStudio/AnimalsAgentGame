using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InitialOptions : MonoBehaviour
{

    //nao utilizar string
    public string levelToLoad;
    public string loadingScreenScene;
    public string actuallevel;

    private bool levelLoaded = false;
    public float delayBeforeLoading = 10f;

    public void iniciar()
    {
        // save game

        if (!levelLoaded) //  level não carregado
        {
            StartCoroutine(LoadLevelAsync());
            levelLoaded = true;
        }

    }

    //verificar dps
    IEnumerator LoadLevelAsync()
    {


        //salvar game


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

    public void opcoes()
    {
        // i don't now
    }

}
