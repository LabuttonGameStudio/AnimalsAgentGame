using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static IEnemy;

public class EnemyCurrentStateVisual : MonoBehaviour
{
    public static Camera mainCamera;

    private SpriteRenderer spriteRenderer;
    [SerializeField]private Sprite confusedIcon;
    [SerializeField]private Sprite visibleIcon;
    [SerializeField]private Sprite battleIcon;
    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
    public void Start()
    {
        if(mainCamera == null) mainCamera = Camera.main;
    }
    private void Update()
    {
        transform.LookAt(mainCamera.transform);
    }
    public void ChangeVisualState(AIState aiState)
    {
        switch(aiState)
        {
            case AIState.Roaming:
                spriteRenderer.sprite = null;
                break;
            case AIState.Observing:
                spriteRenderer.sprite = confusedIcon;
                break;
            case AIState.Searching:
                spriteRenderer.sprite = visibleIcon;
                break;
            case AIState.Attacking:
                spriteRenderer.sprite = battleIcon;
                break;
        }
    }
}
