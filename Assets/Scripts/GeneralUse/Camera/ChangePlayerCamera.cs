using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows.WebCam;

public class ChangePlayerCamera : MonoBehaviour
{
    public static ChangePlayerCamera Instance;

    private float duration;
    private void Awake()
    {
        Instance = this;
    }
    public void SetDurationForChange(float duration)
    {
        this.duration = duration;
    }
    public void ChangeCamOnObjectiveComplete(CinemachineVirtualCamera newCam)
    {
        if (duration != 0)
        {
            Change(newCam, duration, true);
            duration = 0;
        }
        else
        {
            Change(newCam, 2, true);
        }
    }
    public void Change(CinemachineVirtualCamera newCam, float duration, bool makePlayerInvincible)
    {
        StartCoroutine(Change_Coroutine(newCam, duration, makePlayerInvincible));
    }
    private IEnumerator Change_Coroutine(CinemachineVirtualCamera newCam, float duration, bool makePlayerInvincible)
    {
        if (makePlayerInvincible) ArmadilloPlayerController.Instance.hpControl.canReceiveDamage = false;

        int defaultPriority = newCam.Priority;
        ArmadilloPlayerController.Instance.cameraControl.weaponCamera.enabled = false;
        ArmadilloPlayerController.Instance.TogglePlayerControls(false);
        newCam.Priority = 1000;

        yield return new WaitForSeconds(duration + 0.5f);

        newCam.Priority = defaultPriority;
        yield return new WaitForSeconds(0.5f);
        ArmadilloPlayerController.Instance.TogglePlayerControls(true);
        ArmadilloPlayerController.Instance.cameraControl.weaponCamera.enabled = true;

        if (makePlayerInvincible) ArmadilloPlayerController.Instance.hpControl.canReceiveDamage = true;
    }
}
