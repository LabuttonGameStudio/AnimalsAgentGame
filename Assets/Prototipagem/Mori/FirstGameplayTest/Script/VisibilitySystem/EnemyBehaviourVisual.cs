using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static IEnemy;
using static AIBehaviourEnums;

public class EnemyBehaviourVisual : MonoBehaviour
{
    public static Camera mainCamera;

    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField]private Sprite confusedIcon;
    [SerializeField]private Sprite visibleIcon;
    [SerializeField]private Sprite battleIcon;
    public void Start()
    {
        if(mainCamera == null) mainCamera = Camera.main;
    }
    private void Update()
    {
        transform.LookAt(mainCamera.transform);
    }
    public void ChangeVisualState(AIBehaviour AIBehaviour)
    {
        switch(AIBehaviour)
        {
            case AIBehaviour.Roaming:
                spriteRenderer.sprite = null;
                break;
            case AIBehaviour.Observing:
                spriteRenderer.sprite = confusedIcon;
                break;
            case AIBehaviour.Searching:
                spriteRenderer.sprite = visibleIcon;
                break;
            case AIBehaviour.Attacking:
                spriteRenderer.sprite = battleIcon;
                break;
        }
    }
}
