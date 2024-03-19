using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

//Armadillo Line of Sight Control
public class ArmadilloLOSControl : MonoBehaviour
{
    [SerializeField] private float distanceOfChecking;
    [SerializeField] private LayerMask whatIsEnemies;
    [SerializeField] private LayerMask whatIsPickable;
    [SerializeField] private LayerMask whatIsInteractive;
    [HideInInspector] public RaycastHit currentRaycastHit;

    private enum RaycastTypes
    {
        Enemy,
        Pickable,
        Interactive
    }
    private RaycastTypes currentRaycastHitType;

    private void Awake()
    {
        OnEnemyObjectFound = new OnRaycastHitEvent();
        OnPickableObjectFound = new OnRaycastHitEvent();
        OnInteractiveObjectFound = new OnRaycastHitEvent();
    }
    private void Start()
    {
        checkLOS_Ref = StartCoroutine(CheckLOS_Coroutine());
    }
    private Coroutine checkLOS_Ref;
    private IEnumerator CheckLOS_Coroutine()
    {
        Camera camera = ArmadilloPlayerController.Instance.cameraControl.mainCamera;
        while (true)
        {
            LayerMask layer = new LayerMask();
            layer |= 1 << whatIsEnemies;
            layer |= 1 << whatIsPickable;
            layer |= 1 << whatIsInteractive;
            RaycastHit newRaycastHit;
            if (Physics.Raycast(camera.transform.position, camera.transform.forward, out newRaycastHit, distanceOfChecking, layer, QueryTriggerInteraction.Ignore))
            {
                if (currentRaycastHit.rigidbody != newRaycastHit.rigidbody)
                {
                    switch (currentRaycastHitType)
                    {
                        case RaycastTypes.Enemy:
                            OnEnemyObjectLostLOS.Invoke();
                            break;
                        case RaycastTypes.Pickable:
                            OnPickableObjectLostLOS.Invoke();
                            break;
                        case RaycastTypes.Interactive: 
                            OnInteractiveObjectLostLOS.Invoke();
                            break;
                    }
                }
                int hitLayerInt = currentRaycastHit.transform.gameObject.layer;

                if (hitLayerInt == whatIsEnemies.value)
                {
                    OnEnemyObjectFound.Invoke(currentRaycastHit);
                    currentRaycastHitType = RaycastTypes.Enemy;
                }
                else if (hitLayerInt == whatIsPickable.value)
                {
                    OnPickableObjectFound.Invoke(currentRaycastHit);
                    currentRaycastHitType = RaycastTypes.Pickable;
                }
                else if (hitLayerInt == whatIsInteractive.value)
                {
                    OnInteractiveObjectFound.Invoke(currentRaycastHit);
                    currentRaycastHitType = RaycastTypes.Interactive;
                }
            }
            yield return null;
        }
    }
    [HideInInspector] public OnRaycastHitEvent OnEnemyObjectFound;
    [HideInInspector] public UnityEvent OnEnemyObjectLostLOS;

    [HideInInspector] public OnRaycastHitEvent OnPickableObjectFound;
    [HideInInspector] public UnityEvent OnPickableObjectLostLOS;

    [HideInInspector] public OnRaycastHitEvent OnInteractiveObjectFound;
    [HideInInspector] public UnityEvent OnInteractiveObjectLostLOS;
}
[System.Serializable]
public class OnRaycastHitEvent : UnityEvent<RaycastHit>
{
}
