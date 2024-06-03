using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem.XInput;

//Armadillo Line of Sight Control
public class ArmadilloLOSControl : MonoBehaviour
{
    [SerializeField] private float distanceOfChecking;
    private int enemyLayer;
    private int pickableLayer;
    private int interactiveLayer;
    private IRaycastableInLOS currentConnectedObject;



    private void Awake()
    {
        enemyLayer = LayerMask.NameToLayer("Enemies");
        pickableLayer = LayerMask.NameToLayer("Pickable");
        interactiveLayer = LayerMask.NameToLayer("Interactive");
    }
    private void Start()
    {
        checkLOS_Ref = StartCoroutine(CheckLOS_Coroutine());
        ArmadilloPlayerController.Instance.inputControl.inputAction.Armadillo.Interact.Enable();
    }
    #region Check LOS
    private Coroutine checkLOS_Ref;
    private IEnumerator CheckLOS_Coroutine()
    {
        Camera camera = ArmadilloPlayerController.Instance.cameraControl.mainCamera;
        LayerMask detectLayerMask = new LayerMask();
        detectLayerMask |= 1 << enemyLayer;
        detectLayerMask |= 1 << pickableLayer;
        detectLayerMask |= 1 << interactiveLayer;
        while (true)
        {
            RaycastHit newRaycastHit;
            if (Physics.Raycast(camera.transform.position, camera.transform.forward, out newRaycastHit, distanceOfChecking, detectLayerMask, QueryTriggerInteraction.Ignore))
            {
                if (newRaycastHit.collider.TryGetComponent(out IRaycastableInLOS raycastable))
                {
                    if (currentConnectedObject != raycastable)
                    {
                        raycastable.OnLeaveLOS();
                        raycastable.OnEnterLOS();
                        currentConnectedObject = raycastable;
                    }
                }
                else if (currentConnectedObject != null)
                {
                    currentConnectedObject.OnLeaveLOS();
                    currentConnectedObject = null;
                }
            }
            else if (currentConnectedObject != null)
            {
                currentConnectedObject.OnLeaveLOS();
                currentConnectedObject = null;
            }
            yield return new WaitForFixedUpdate();
        }
    }
    #endregion
}
