using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static IEnemy;
using static AIBehaviourEnums;

public class EnemyBehaviourVisual : MonoBehaviour
{
    public static Camera mainCamera;

    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField]private Sprite observingIcon;
    [SerializeField]private Sprite searchingIcon;
    [SerializeField]private Sprite attackingIcon;
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
                spriteRenderer.sprite = observingIcon;
                break;
            case AIBehaviour.Searching:
                spriteRenderer.sprite = searchingIcon;
                break;
            case AIBehaviour.Attacking:
                spriteRenderer.sprite = attackingIcon;
                break;
        }
    }
}
