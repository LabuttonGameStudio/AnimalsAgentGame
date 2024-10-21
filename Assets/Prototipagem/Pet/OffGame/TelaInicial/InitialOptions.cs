using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;


public class InitialOptions : MonoBehaviour
{

    [Header("LEVEL AND LOAD")]
    public string levelToLoad;
    public string loadingScreenScene;
    public string actuallevel;
    private bool levelLoaded = false;
    public float delayBeforeLoading = 10f; // para o tutorial ir carregando

    [Header("MOUSE")]
    public Texture2D cursorTexture;


    private void Start()
    {
        Cursor.SetCursor(cursorTexture, Vector2.zero, CursorMode.Auto);
    }


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

    //verificar dps Mori
    IEnumerator LoadLevelAsync()
    {
        // anim 
        yield return new WaitForSeconds(2f);

        // Carrega a cena da tela de carregamento
        yield return SceneManager.LoadSceneAsync(loadingScreenScene, LoadSceneMode.Additive);
        SceneManager.UnloadSceneAsync(actuallevel);

        // carrega de forma assin o tutorial
        AsyncOperation levelLoadOperation = SceneManager.LoadSceneAsync(levelToLoad, LoadSceneMode.Additive);
        //levelLoadOperation.allowSceneActivation = false;  // nao att imediatamente

        // delay para carregar a cena de tuto
        float elapsedTime = 0f;
        while (elapsedTime < delayBeforeLoading)
        {
            elapsedTime += Time.deltaTime;
            yield return null;  
        }

        // aguarda se nao tiver carregado a cena
        while (!levelLoadOperation.isDone)
        {
            if (levelLoadOperation.progress >= 0.9f)
            {
                //att quando estivar quase ompleto
                levelLoadOperation.allowSceneActivation = true;
            }
            yield return null;
        }

        // descarrega loading
        SceneManager.UnloadSceneAsync(loadingScreenScene);
    }

    public void Exit()
    {
        Application.Quit();
    }

}


