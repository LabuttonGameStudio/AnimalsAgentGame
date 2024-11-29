using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using static SceneController;


public class InitialOptions : MonoBehaviour
{
    [Header("LEVEL AND LOAD")]
    private bool levelLoaded = false;
    public float delayBeforeLoading = 2f; // para o tutorial ir carregando

    [Header("MOUSE")]
    public Texture2D cursorTexture;


    private void Start()
    {
       // Cursor.SetCursor(cursorTexture, Vector2.zero, CursorMode.Auto);
    }

    public void LoadTutorial()
    {
        ArmadilloPlayerController.ClearPlayerSaveData();
        SceneManager.LoadScene((int)SceneController.ScenesEnum.Cutscene);
    }
    public void LoadCais()
    {
        SaveAndLoad(ScenesEnum.Cais);
    }
    public void LoadBoss()
    {
        SaveAndLoad(ScenesEnum.Bossfight);
    }
    public void SaveAndLoad(SceneController.ScenesEnum sceneEnum)
    {
        // save game
        ArmadilloPlayerController.ClearPlayerSaveData();
        if (!levelLoaded) //  level nao carregado
        {
            SceneController.Instance.StartCoroutine(LoadLevelAsync(sceneEnum));
            Debug.Log("Iniciando");
            levelLoaded = true;
        }

    }

    IEnumerator LoadLevelAsync(SceneController.ScenesEnum sceneEnum)
    {
        // anim 
        yield return new WaitForSeconds(2f);

        // Carrega a cena da tela de carregamento
        Scene currentScene = SceneManager.GetActiveScene();
        yield return SceneManager.LoadSceneAsync((int)SceneController.ScenesEnum.Loading, LoadSceneMode.Additive);
        SceneManager.UnloadSceneAsync(currentScene);

        // carrega de forma assin o tutorial
        AsyncOperation levelLoadOperation = SceneManager.LoadSceneAsync((int)sceneEnum, LoadSceneMode.Additive);
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
        SceneManager.UnloadSceneAsync((int)SceneController.ScenesEnum.Loading);
    }

    public void Exit()
    {
        Application.Quit();
    }

}


