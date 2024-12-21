using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static SceneController;

public class LoadSceneCaller : MonoBehaviour
{
    [SerializeField]private ScenesEnum nextScene;
    public void LoadLevel()
    {
        SceneController.LoadLevel(nextScene);
    }
}
