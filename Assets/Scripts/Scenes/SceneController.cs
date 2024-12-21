using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    public enum ScenesEnum
    {
        MenuPause,
        Loading,
        Cutscene,
        Tutorial,
        Cais,
        Bossfight
    }
    public static SceneController Instance;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }
    public static void LoadLevel(int nextScene)
    {
        levelLoad_Ref = Instance.StartCoroutine(LoadLevelAsync((ScenesEnum)nextScene));
    }
    public static void LoadLevel(ScenesEnum nextScene)
    {
        levelLoad_Ref = Instance.StartCoroutine(LoadLevelAsync(nextScene));
    }

    private static Coroutine levelLoad_Ref;
    private static IEnumerator LoadLevelAsync(ScenesEnum loadNextScene)
    {
        // Carrega a cena da tela de carregamento
        Scene currentScene = SceneManager.GetActiveScene();
        yield return SceneManager.LoadSceneAsync((int)ScenesEnum.Loading, LoadSceneMode.Additive);
        SceneManager.UnloadSceneAsync(currentScene);

        // carrega de forma assin o tutorial
        AsyncOperation levelLoadOperation = SceneManager.LoadSceneAsync((int)loadNextScene, LoadSceneMode.Additive);
        //levelLoadOperation.allowSceneActivation = false;  // nao att imediatamente

        // delay para carregar a cena de tuto
        float elapsedTime = 0f;
        while (elapsedTime < 3)
        {
            LoadingScreen.Instance.loadingSlider.value = (elapsedTime / 2) * 0.25f;
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        LoadingScreen.Instance.loadingSlider.value = 0.25f;
        // aguarda se nao tiver carregado a cena
        while (!levelLoadOperation.isDone)
        {
            LoadingScreen.Instance.loadingSlider.value = 0.25f + 0.75f * (levelLoadOperation.progress);
            yield return null;
        }
        LoadingScreen.Instance.loadingSlider.value = 1;
        yield return new WaitForSeconds(0.5f);
        levelLoadOperation.allowSceneActivation = true;

        // descarrega loading
        yield return SceneManager.UnloadSceneAsync((int)ScenesEnum.Loading);
        levelLoad_Ref = null;
    }
}
