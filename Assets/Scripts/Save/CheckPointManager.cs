using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPointManager : MonoBehaviour
{
    public static readonly string PlayerCurrentCheckPointSaveKey = "PlayerLastCheckpoint";

    public static CheckPointManager Instance;

    [SerializeField] private CheckPointArea[] checkpoints;

    private void Awake()
    {
        Instance = this;
    }

    public void SaveCheckPoint(CheckPointArea checkPoint)
    {
        for(int i = 0; i < checkpoints.Length; i++)
        {
            if (checkpoints[i] == checkPoint)
            {
                PlayerPrefs.SetInt(PlayerCurrentCheckPointSaveKey, i);
                return;
            }
        }
        Debug.Log("Error in finding checkpoint");
    }
    public void ClearCheckpoints()
    {
        PlayerPrefs.DeleteKey(PlayerCurrentCheckPointSaveKey);
    }

    public Transform GetReturnPoint(int checkpointIndex)
    {
        return checkpoints[checkpointIndex].returnPoint;
    }
    public void OnLoadCheckpoint(int checkpointIndex)
    {
        for(int i = 0;i <=checkpointIndex; i++)
        {
            checkpoints[i].onLoadIntoCheckpoint.Invoke();
        }
    }
}
