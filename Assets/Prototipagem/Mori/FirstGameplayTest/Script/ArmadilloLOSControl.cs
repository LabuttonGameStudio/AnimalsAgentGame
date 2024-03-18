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
    [HideInInspector] public RaycastHit[] raycastHits;

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
            raycastHits = Physics.RaycastAll(camera.transform.position, camera.transform.forward, distanceOfChecking, layer, QueryTriggerInteraction.Ignore);
            if(raycastHits.Length <= 0 ) continue;
            foreach(RaycastHit hit in raycastHits )
            {
                int hitLayerInt = hit.transform.gameObject.layer;
                if( hitLayerInt == whatIsEnemies.value) OnEnemyObjectFound.Invoke(hit);
                else if( hitLayerInt == whatIsPickable.value) OnPickableObjectFound.Invoke(hit);
                else if( hitLayerInt == whatIsInteractive.value) OnInteractiveObjectFound.Invoke(hit);
            }
            yield return new WaitForFixedUpdate();
        }
    }
    [HideInInspector] public OnRaycastHitEvent OnEnemyObjectFound;
    [HideInInspector] public OnRaycastHitEvent OnPickableObjectFound;
    [HideInInspector] public OnRaycastHitEvent OnInteractiveObjectFound;
}
[System.Serializable]
public class OnRaycastHitEvent : UnityEvent<RaycastHit>
{
}
