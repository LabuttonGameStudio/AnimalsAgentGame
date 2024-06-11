using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmadilloVisualControl : MonoBehaviour
{
    private enum CameraMode
    {
        FP,
        TP
    }
    [SerializeField] private CameraMode m_CameraMode;
    private enum FPModeLayer0
    {
        Walking,
        Sprinting,
        Lurking
    }
    [SerializeField] private FPModeLayer0 fp_Layer0;
    private bool isClimbing;
    private enum FPModeLayer1
    {
        Null,
        LedgeGrab,
        Sonar,
        Gun_Default,
        Pause,
    }
    [SerializeField] private FPModeLayer1 fp_Layer1;
    private bool hasGunEquiped;
    [Header("Default")]
    [SerializeField] private SkinnedMeshRenderer modelRenderer;
    [SerializeField] private Animator modelAnimator;

    [Space]
    [Header("Ball")]
    [SerializeField] private Transform ballTransform;
    [SerializeField] private float ballRollVelocity;
    [Space]
    [Header("Fisrt Person")]
    [SerializeField] private SkinnedMeshRenderer fpRenderer;
    [SerializeField] private Animator fpAnimator;

    [Header("VFX")]
    [SerializeField] private ParticleSystem transformationSmoke;
    [SerializeField] private ParticleSystem TreckOnomatopeia;
    [SerializeField] private ParticleSystem onBallHitParticle;


    #region VFX
    public void OnBallHit(Vector3 position, Vector3 lookAtPos)
    {
        onBallHitParticle.transform.position = position;
        onBallHitParticle.transform.LookAt(lookAtPos);
        onBallHitParticle.Play();
    }
    #endregion

    #region ThirdPerson
    public void OnEnterBallMode()
    {
        PlayerCamera cameraControl = ArmadilloPlayerController.Instance.cameraControl;
        StartRotateBall();
        cameraControl.ToggleFPCamera(false);
        cameraControl.ToggleCullingMask(false);
        modelAnimator.SetBool("rollingAction", true);
        m_CameraMode = CameraMode.TP;
        isClimbing = false;
    }
    public void ChangeMeshToDefault()
    {
        StopRotateBall();
        modelRenderer.enabled = true;
        ballTransform.gameObject.SetActive(false);
        modelAnimator.SetBool("rollingAction", false);
    }
    public void PlayTransformationSmoke()
    {
        transformationSmoke.Play();
        TreckOnomatopeia.Play();
    }
    public void ChangeMeshToBall()
    {
        ballTransform.gameObject.SetActive(true);
        modelRenderer.enabled = false;
    }
    public void OnEnterDefaultMode()
    {
        PlayerCamera cameraControl = ArmadilloPlayerController.Instance.cameraControl;
        cameraControl.ToggleFPCamera(true);
        cameraControl.ToggleCullingMask(true);
        m_CameraMode = CameraMode.FP;
    }

    #region Ball Rotation
    public void StartRotateBall()
    {
        if (rotateBall_Ref == null)
        {
            rotateBall_Ref = StartCoroutine(RotateBall_Coroutine());
        }
    }
    public void StopRotateBall()
    {
        if (rotateBall_Ref != null)
        {
            StopCoroutine(rotateBall_Ref);
            rotateBall_Ref = null;
        }
    }
    public Coroutine rotateBall_Ref;
    public IEnumerator RotateBall_Coroutine()
    {
        Vector3 velocity;
        while (true)
        {
            velocity = ArmadilloPlayerController.Instance.movementControl.rb.velocity;
            velocity.y = Mathf.Max(0, velocity.y * 1.5f);
            ballTransform.Rotate(Time.deltaTime * -1 * velocity.magnitude * ballRollVelocity, 0, 0);
            yield return null;
        }
    }
    #endregion
    #endregion

    #region FirstPerson
    public void OnLedgeGrab()
    {
        ArmadilloPlayerController.Instance.weaponControl.ToggleWeapon(false);
        fp_Layer1 = FPModeLayer1.LedgeGrab;
        fpAnimator.SetTrigger("ledgeGrab");
        StartToggleWeaponDelay(0.6f, true);
    }
    public void OnClimbingStart()
    {
        OnStartClimbing();
        ArmadilloPlayerController.Instance.weaponControl.ToggleWeapon(false);
    }
    public void OnClimbingStop()
    {
        isClimbing = false;
        ArmadilloPlayerController.Instance.weaponControl.ToggleWeapon(true);
        switch (fp_Layer0)
        {
            case FPModeLayer0.Sprinting:
                ToggleRun(true);
                break;
            case FPModeLayer0.Lurking:
                ToggleLurking(true);
                break;
        }
    }
    public void OnSprintStart()
    {
        if (fp_Layer0 == FPModeLayer0.Lurking)
        {
            ToggleLurking(false);
        }
        fp_Layer0 = FPModeLayer0.Sprinting;
        ToggleRun(true);

    }
    public void OnSprintStop()
    {
        if (fp_Layer0 == FPModeLayer0.Sprinting) fp_Layer0 = FPModeLayer0.Walking;
        ToggleRun(false);
    }
    public void OnLurkStart()
    {
        if (fp_Layer0 == FPModeLayer0.Sprinting)
        {
            ToggleRun(false);
        }
        fp_Layer0 = FPModeLayer0.Lurking;
        ToggleLurking(true);
    }
    public void OnLurkStop()
    {
        if (fp_Layer0 == FPModeLayer0.Lurking) fp_Layer0 = FPModeLayer0.Walking;
        ToggleLurking(false);
    }
    public void OnSonar()
    {
        ArmadilloPlayerController.Instance.weaponControl.ToggleWeapon(false);
        fpAnimator.SetTrigger("scannerOn");
        fp_Layer1 = FPModeLayer1.Sonar;
        StartToggleWeaponDelay(1.25f, true);
    }

    public void OnPause(bool pause)
    {
        ArmadilloPlayerController.Instance.weaponControl.ToggleWeapon(!pause);
        fpAnimator.SetBool("isPaused", pause);
        fp_Layer1 = FPModeLayer1.Pause;
    }

    public void ReturnPause()
    {

        fpAnimator.SetBool("isPaused", false);
        fp_Layer1 = FPModeLayer1.Pause;
    }

    private void StartToggleWeaponDelay(float delay, bool state)
    {
        if (toggleWeaponDelay_Ref != null)
        {
            StopCoroutine(toggleWeaponDelay_Ref);
        }
        toggleWeaponDelay_Ref = StartCoroutine(ToggleWeaponDelay_Coroutine(delay, state));
    }
    private Coroutine toggleWeaponDelay_Ref;
    private IEnumerator ToggleWeaponDelay_Coroutine(float delay, bool state)
    {
        yield return new WaitForSeconds(delay);
        if (isClimbing)
        {
            toggleWeaponDelay_Ref = null;
            yield break;
        }
        ArmadilloPlayerController.Instance.weaponControl.ToggleWeapon(state);
        ReturnToDefaultState();
        toggleWeaponDelay_Ref = null;
    }

    private void ToggleRun(bool state)
    {
        if (isClimbing) return;
        fpAnimator.SetBool("isRunning", state);
    }
    private void ToggleLurking(bool state)
    {
        if (isClimbing) return;
        fpAnimator.SetBool("isSneaking", state);
    }
    public void OnGunEquiped()
    {
        hasGunEquiped = true;
    }
    public void OnEletricGunFire()
    {
        fpAnimator.SetTrigger("zapGunShoot");
    }
    public void ToggleEletricPistolCharge(bool state)
    {
        fpAnimator.SetBool("zapGunIsCharging",state);
    }
    public void ToggleEletricPistolOverheat(bool state)
    {
        fpAnimator.SetBool("zapGunOverheat", state);
    }
    public void OnStartClimbing()
    {
        isClimbing = true;
        fpAnimator.SetBool("isSneaking", false);
        fpAnimator.SetBool("isRunning", false);
    }
    private void ReturnToDefaultState()
    {
        if (hasGunEquiped)
        {
            fp_Layer1 = FPModeLayer1.Gun_Default;
        }
        else fp_Layer1 = FPModeLayer1.Null;
    }

    public void EquipEletricPistol(Transform eletricPistolTransform)
    {
        fpAnimator.SetBool("zapGunEquip", true);
    }
    public void UnequipEletricPistol(Transform eletricPistolTransform)
    {
        fpAnimator.SetBool("zapGunEquip", false);
    }
    #endregion
}
