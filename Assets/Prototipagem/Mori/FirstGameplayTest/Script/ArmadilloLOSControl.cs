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
    [SerializeField] public float distanceOfChecking;
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
        StartLOSCheck();
        ArmadilloPlayerController.Instance.inputControl.inputAction.Armadillo.Interact.Enable();
    }
    #region Check LOS

    public void StartLOSCheck()
    {
        if(checkLOS_Ref == null)
        {
            checkLOS_Ref = StartCoroutine(CheckLOS_Coroutine());
        }
    }

    public void StopLOSCheck()
    {
        if (checkLOS_Ref != null)
        {
            StopCoroutine(checkLOS_Ref);
            checkLOS_Ref= null;
            if(currentConnectedObject != null)
            {
                currentConnectedObject.OnLeaveLOS();
                currentConnectedObject = null;
            }
        }
    }

    private Coroutine checkLOS_Ref;
    private IEnumerator CheckLOS_Coroutine()
    {
        Camera camera = ArmadilloPlayerController.Instance.cameraControl.mainCamera;
        while (true)
        {
            RaycastHit newRaycastHit;
            if (Physics.Raycast(camera.transform.position, camera.transform.forward, out newRaycastHit, distanceOfChecking, Physics.AllLayers, QueryTriggerInteraction.Ignore))
            {
                if (!(newRaycastHit.collider.gameObject.layer == enemyLayer || newRaycastHit.collider.gameObject.layer == pickableLayer || newRaycastHit.collider.gameObject.layer == interactiveLayer))
                {
                    if (currentConnectedObject != null)
                    {
                        currentConnectedObject.OnLeaveLOS();
                        currentConnectedObject = null;
                    }
                    yield return new WaitForFixedUpdate();
                    continue;
                }
                if (newRaycastHit.collider.TryGetComponent(out IRaycastableInLOS raycastable))
                {
                    if ((MonoBehaviour)currentConnectedObject != (MonoBehaviour)raycastable)
                    {
                        if(currentConnectedObject != null)currentConnectedObject.OnLeaveLOS();
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
