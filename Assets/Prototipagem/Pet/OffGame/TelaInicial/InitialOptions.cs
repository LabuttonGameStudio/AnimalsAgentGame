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

        if (!levelLoaded) //  level n�o carregado
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

        // Obt�m a refer�ncia para a cena atual
        Scene currentScene = SceneManager.GetActiveScene();

        // Descarrega a cena atual para evitar lags e bugs
        SceneManager.UnloadSceneAsync(currentScene);

        // Carrega a cena da tela de carregamento
        yield return SceneManager.LoadSceneAsync(loadingScreenScene, LoadSceneMode.Additive);

        // Espera um tempo antes de carregar a pr�xima cena
        yield return new WaitForSeconds(delayBeforeLoading);

        // Carrega a pr�xima cena de forma ass�ncrona
        AsyncOperation levelLoadOperation = SceneManager.LoadSceneAsync(levelToLoad, LoadSceneMode.Additive);
        levelLoadOperation.allowSceneActivation = false;

        // Aguarda at� que a pr�xima cena esteja pronta para ser ativada
        while (!levelLoadOperation.isDone)
        {
            if (levelLoadOperation.progress >= 0.9f)
            {
                // Ativa a pr�xima cena
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
