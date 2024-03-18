using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmadilloPickUpControl : MonoBehaviour
{
    [SerializeField] private LayerMask whatIsPickable;

    private Rigidbody objectRb;
    private Transform objectTransform;

    public void Grab(Transform grabbedObjectTransform)
    {
        objectTransform = grabbedObjectTransform;
        objectRb = grabbedObjectTransform.GetComponent<Rigidbody>();
        objectRb.freezeRotation = true;
        objectRb.isKinematic = true;
        ToggleHoldObjectCoroutine(true);
    }
    public void Drop()
    {
        objectRb.freezeRotation = false;
        objectRb.isKinematic = false;
        objectTransform = null;
        objectRb = null;
        ToggleHoldObjectCoroutine(false);
    }
    private void ToggleHoldObjectCoroutine(bool state)
    {
        if (state)
        {
            if (holdObject_Ref == null) holdObject_Ref = StartCoroutine(HoldObject_Coroutine());
        }
        else
        {
            if (holdObject_Ref != null)
            {
                StopCoroutine(holdObject_Ref);
                holdObject_Ref = null;
            }
        }
    }
    private Coroutine holdObject_Ref;
    public IEnumerator HoldObject_Coroutine()
    {
        Transform cameraTransform = ArmadilloPlayerController.Instance.cameraControl.mainCamera.transform;
        while (true)
        {
            objectRb.MovePosition(cameraTransform.position + cameraTransform.forward * 3f);
            yield return new WaitForFixedUpdate();
        }
    }
}
