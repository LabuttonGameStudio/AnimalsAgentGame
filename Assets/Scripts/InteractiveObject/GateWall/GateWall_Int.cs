using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.Events;

public class GateWall_Int : MonoBehaviour
{
    [SerializeField] private UnityEvent onGateOpen;
    [SerializeField] private CinemachineVirtualCamera virtualCamera;
    [Header("Doors")]
    [SerializeField]private Transform leftDoor;
    [SerializeField]private Transform rightDoor;
    [Header("Lamps")]
    [SerializeField] private MeshRenderer[] lamps;
    [Header("Gears")]
    [SerializeField] private Transform[] gears;
    [Header("Locks")]
    [SerializeField] private Transform[] locks;

    private int segmentsOn;
    private void Awake()
    {
        foreach(MeshRenderer lamp in lamps)
        {
            lamp.material = new Material(lamp.material);
        }
    }
    public void TurnOnSegment()
    {
        segmentsOn++;
        CheckForCompletion();
    }
    public void CheckForCompletion()
    {
        if(segmentsOn >=3)
        {
            ChangePlayerCamera.Instance.Change(virtualCamera, 4.5f, true); 
            StartCoroutine(OpenGateWall_Coroutine());
        }
        else
        {
            ChangePlayerCamera.Instance.Change(virtualCamera, 2, true); 
            StartCoroutine(OpenSegment_Coroutine(segmentsOn-1));
        }
    }
    private IEnumerator OpenSegment_Coroutine(int segmentIndex)
    {
        yield return new WaitForSeconds(0.5f);
        lamps[segmentIndex].material.SetInt("_Light_on_off", 1);
        float timer = 0;
        float duration = 2f;

        Vector3 gearRotationAmount = new Vector3(1080,0,0);
        Vector3 gearStartRot = gears[segmentIndex].localRotation.eulerAngles;

        Vector3 lockMovementAmount = new Vector3(0,0,4.5f);
        Vector3 lockStartPos = locks[segmentIndex].localPosition;
        while (timer< duration)
        {
            gears[segmentIndex].localRotation = Quaternion.Euler(Vector3.Lerp(gearStartRot, gearStartRot + gearRotationAmount, timer / duration));
            locks[segmentIndex].localPosition = Vector3.Lerp(lockStartPos, lockStartPos + lockMovementAmount, timer / duration);
            timer += Time.deltaTime;
            yield return null;
        }
        gears[segmentIndex].localRotation = Quaternion.Euler(gearStartRot + gearRotationAmount);
        locks[segmentIndex].localPosition = lockStartPos + lockMovementAmount;
    }
    private IEnumerator OpenGateWall_Coroutine()
    {
        yield return new WaitForSeconds(0.5f);
        yield return OpenSegment_Coroutine(segmentsOn - 1);
        yield return new WaitForSeconds(0.5f);
        Quaternion leftDoorStartRot = leftDoor.localRotation;
        Quaternion rightDoorStartRot = rightDoor.localRotation;

        Quaternion leftDoorFinalRot = Quaternion.Euler(leftDoor.localRotation.eulerAngles + new Vector3(0, -90, 0));
        Quaternion rightDoorFinalRot = Quaternion.Euler(rightDoor.localRotation.eulerAngles + new Vector3(0, 90, 0));

        float timer = 0;
        float duration = 2f;
        while (timer < duration)
        {
            leftDoor.localRotation = Quaternion.Lerp(leftDoorStartRot, leftDoorFinalRot, timer / duration);
            rightDoor.localRotation = Quaternion.Lerp(rightDoorStartRot, rightDoorFinalRot, timer / duration);
            timer+= Time.deltaTime;
            yield return null;
        }
        leftDoor.localRotation = leftDoorFinalRot;
        rightDoor.localRotation = rightDoorFinalRot;
        onGateOpen.Invoke();
    }
}
