using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static IEnemy;
using static AIBehaviourEnums;

public class EnemyBehaviourVisual : MonoBehaviour
{
    public static Camera mainCamera;

    [SerializeField] private SpriteRenderer spriteRenderer;

    [SerializeField] private Sprite observingIcon;
    [SerializeField] private Sprite searchingIcon;
    [SerializeField] private Sprite attackingIcon;

    [SerializeField]private Material observingIconMaterial;
    [SerializeField]private Material searchingIconMaterial;
    [SerializeField]private Material attackingIconMaterial;


    [Space]
    [SerializeField] private SpriteRenderer alertSpriteRender;
    public void Start()
    {
        if (mainCamera == null) mainCamera = ArmadilloPlayerController.Instance.cameraControl.mainCamera;
    }
    private void FixedUpdate()
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
                spriteRenderer.material = observingIconMaterial;
                spriteRenderer.sprite = observingIcon;
                break;
            case AIBehaviour.Searching:
                spriteRenderer.material = searchingIconMaterial;
                spriteRenderer.sprite = searchingIcon;
                break;
            case AIBehaviour.Attacking:
                spriteRenderer.material = attackingIconMaterial;
                spriteRenderer.sprite = attackingIcon;
                break;
        }
    }
    public void ToggleAlert(bool state)
    {
        alertSpriteRender.enabled = state;
    }
}
