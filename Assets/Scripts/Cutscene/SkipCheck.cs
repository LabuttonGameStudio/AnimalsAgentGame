using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SkipCheck : MonoBehaviour
{
    private readonly string tutorial = "1_Tutorial";
    private readonly string loading = "TeladeCarregamentoScene";
    private readonly string cutscene = "Cutscene";
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if(levelLoad_Ref == null)
            {
                levelLoad_Ref = SceneController.Instance.StartCoroutine(LoadLevelAsync());
            }
        }
    }
    Coroutine levelLoad_Ref;
    IEnumerator LoadLevelAsync()
    {
        // Carrega a cena da tela de carregamento
        Scene currentScene = SceneManager.GetActiveScene();
        yield return SceneManager.LoadSceneAsync(loading, LoadSceneMode.Additive);
        SceneManager.UnloadSceneAsync(currentScene);

        // carrega de forma assin o tutorial
        AsyncOperation levelLoadOperation = SceneManager.LoadSceneAsync(tutorial, LoadSceneMode.Additive);
        //levelLoadOperation.allowSceneActivation = false;  // nao att imediatamente

        // delay para carregar a cena de tuto
        float elapsedTime = 0f;
        while (elapsedTime < 3)
        {
            LoadingScreen.Instance.loadingSlider.value = (elapsedTime / 2)*0.25f;
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        LoadingScreen.Instance.loadingSlider.value = 0.25f;
        // aguarda se nao tiver carregado a cena
        while (!levelLoadOperation.isDone)
        {
            LoadingScreen.Instance.loadingSlider.value = 0.25f + 0.75f*(levelLoadOperation.progress);
            yield return null;
        }
        LoadingScreen.Instance.loadingSlider.value = 1;
        yield return new WaitForSeconds(0.5f);
        levelLoadOperation.allowSceneActivation = true;

        // descarrega loading
        SceneManager.UnloadSceneAsync(loading);
    }
}
