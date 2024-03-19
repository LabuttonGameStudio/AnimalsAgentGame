using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmadilloPickUpControl : MonoBehaviour
{
    [SerializeField] private LayerMask whatIsPickable;

    private Rigidbody objectRb;

    float objectDefaultDrag;

    public void Grab(Transform grabbedObjectTransform)
    {
        objectRb = grabbedObjectTransform.GetComponent<Rigidbody>();
        objectRb.freezeRotation = true;
        objectDefaultDrag = objectRb.drag;
        objectRb.drag = 10;
        objectRb.useGravity = false;
        ToggleHoldObjectCoroutine(true);
    }
    public void Drop()
    {
        objectRb.freezeRotation = false;
        objectRb.useGravity = true;
        objectRb.drag = objectDefaultDrag;
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
            Vector3 holdArea = cameraTransform.position + cameraTransform.forward * 3f;
            if (Vector3.Distance(holdArea, objectRb.position) > 0.1f)
            {
                Vector3 moveDirection = holdArea - objectRb.position;
                objectRb.AddForce(moveDirection*1.5f,ForceMode.VelocityChange);
            }
            yield return new WaitForFixedUpdate();
        }
    }
}
