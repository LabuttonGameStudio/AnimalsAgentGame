using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class ArmadilloVisualControl : MonoBehaviour
{
    private readonly float crossFadeTime = 0.2f;
    private enum CameraMode
    {
        FP,
        TP
    }
    [SerializeField] private CameraMode m_CameraMode;
    private enum FPModeLayer0
    {
        Idle,
        Walking,
        Sprinting,
        Lurking
    }
    [SerializeField] private FPModeLayer0 fp_Layer0;
    private bool isClimbing;
    private enum FPModeLayer1
    {
        Null,
        ZapGun,
        WaterGun,
        PendriveGun,
    }
    [SerializeField] private FPModeLayer1 fp_Layer1;

    private enum FPModeLayer2
    {
        Null,
        Hold,
        Interact,
        Sonar,
        Melee,
        LedgeGrab
    }

    [SerializeField] private FPModeLayer2 fp_Layer2;

    private enum FPModeLayer99
    {
        Null, Pause
    }
    [SerializeField] private FPModeLayer99 fp_Layer99;

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
    [SerializeField] public Animator fpAnimator;

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
    public void ToggleArmView(bool state)
    {
        fpAnimator.transform.GetChild(0).gameObject.SetActive(state);
    }

    #region FP Layer
    private void ChangeCurrentLayer(int newLayerInt)
    {
        if (crossFadeLayers_Ref != null)
        {
            StopCoroutine(crossFadeLayers_Ref.crossFadeCoroutine);
            fpAnimator.SetLayerWeight(crossFadeLayers_Ref.previousLayer, 0);
            fpAnimator.SetLayerWeight(crossFadeLayers_Ref.previousLayer, 1);
            crossFadeLayers_Ref = null;
        }
        crossFadeLayers_Ref = new CrossFadeLayer();
        crossFadeLayers_Ref.previousLayer = CheckCurrentFPLayer()+1;
        crossFadeLayers_Ref.newLayer = newLayerInt+1;
        crossFadeLayers_Ref.transitionAmount = 0;
        crossFadeLayers_Ref.crossFadeCoroutine = StartCoroutine(CrossFadeLayers_Coroutine());
    }
    private CrossFadeLayer crossFadeLayers_Ref;
    private IEnumerator CrossFadeLayers_Coroutine()
    {
        float timer = 0;
        while (timer <= crossFadeTime)
        {
            crossFadeLayers_Ref.transitionAmount = timer / crossFadeTime;
            fpAnimator.SetLayerWeight(crossFadeLayers_Ref.previousLayer, 1 - crossFadeLayers_Ref.transitionAmount);
            fpAnimator.SetLayerWeight(crossFadeLayers_Ref.newLayer, crossFadeLayers_Ref.transitionAmount);
            timer += Time.deltaTime;
            yield return null;
        }
        fpAnimator.SetLayerWeight(crossFadeLayers_Ref.previousLayer, 0);
        fpAnimator.SetLayerWeight(crossFadeLayers_Ref.newLayer, 1);
    }
    public int CheckCurrentFPLayer()
    {
        if (fp_Layer99 != FPModeLayer99.Null) return 3;
        if (fp_Layer2 != FPModeLayer2.Null) return 2;
        if (fp_Layer1 != FPModeLayer1.Null) return 1;
        return 0;
    }
    public bool CheckIfActionsIsPossible(int actionLayer)
    {
        int currentLayer = CheckCurrentFPLayer();
        return actionLayer >= currentLayer;
    }

    private void OnLayer2AnimationCancel()
    {
        switch (fp_Layer2)
        {
            case FPModeLayer2.Hold:
                break;
        }
    }
    private void ResetLayer1()
    {
        FPModeLayer1 previousLayer1 = fp_Layer1;
        fp_Layer1 = FPModeLayer1.Null;
    }
    private void ResetLayer2()
    {
        fp_Layer2 = FPModeLayer2.Null;
    }
    private void ResetLayer99()
    {
        fp_Layer99 = FPModeLayer99.Null;
    }
    #endregion

    #region LedgeGrab & Climb
    public void OnLedgeGrab()
    {
        ArmadilloPlayerController.Instance.weaponControl.ToggleWeapon(false, true);
        fp_Layer2 = FPModeLayer2.LedgeGrab;
        fpAnimator.SetTrigger("ledgeGrab");
        StartToggleWeaponDelay(0.28f, true);
    }
    public void OnStartClimbing()
    {
        isClimbing = true;
        fpAnimator.SetBool("isSneaking", false);
        fpAnimator.SetBool("isRunning", false);
    }
    public void OnClimbingStart()
    {
        OnStartClimbing();
        ArmadilloPlayerController.Instance.weaponControl.ToggleArms(false);
    }

    public void OnClimbingStop()
    {
        isClimbing = false;
        ArmadilloPlayerController.Instance.weaponControl.ToggleArms(true);
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
    #endregion

    #region Sprint
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
    private void ToggleRun(bool state)
    {
        if (isClimbing) return;
        fpAnimator.SetBool("isRunning", state);
    }
    #endregion

    #region Lurk
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
    private void ToggleLurking(bool state)
    {
        if (isClimbing) return;
        fpAnimator.SetBool("isSneaking", state);
    }
    #endregion

    #region Sonar
    //Layer 2
    public void OnSonar()
    {
        if (CheckIfActionsIsPossible(2))
        {
            ArmadilloPlayerController.Instance.weaponControl.ToggleWeapon(false, false);
            fpAnimator.CrossFade("TatuSkillSonar", crossFadeTime);

            OnLayer2AnimationCancel();
            ChangeCurrentLayer(2);
            fp_Layer2 = FPModeLayer2.Sonar;

            StartToggleWeaponDelay(1.25f, true);
            AnimationClip animationClip = fpAnimator.GetCurrentAnimatorClipInfo(CheckCurrentFPLayer()+1)[0].clip;
            StartCoroutine(OnSonarAnimEnd_Coroutine(animationClip.length));
        }
    }
    private IEnumerator OnSonarAnimEnd_Coroutine(float duration)
    {
        yield return new WaitForSeconds(duration);
        if (fp_Layer2 == FPModeLayer2.Sonar)
        {
            ResetLayer2();
        }
    }
    #endregion

    #region Pause
    public void OnPause()
    {
        fpAnimator.CrossFade("TatuPauseIdle", crossFadeTime);
        fp_Layer99 = FPModeLayer99.Pause;
    }

    public void ReturnPause()
    {
        fpAnimator.CrossFade("Idle", crossFadeTime);
        fp_Layer99 = FPModeLayer99.Null;
    }
    #endregion


    #region Weapons
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
        if (state) ArmadilloPlayerController.Instance.weaponControl.ToggleWeapon(state, false);
        else ArmadilloPlayerController.Instance.weaponControl.ToggleWeapon(state, false);
        ReturnToDefaultState();
        toggleWeaponDelay_Ref = null;
    }

    private void ReturnToDefaultState()
    {
        if (hasGunEquiped)
        {
            fp_Layer1 = FPModeLayer1.ZapGun;
        }
        else fp_Layer1 = FPModeLayer1.Null;
    }
    #endregion

    #region EletricPistol
    public void OnGunEquiped()
    {
        hasGunEquiped = true;
    }

    public void EquipEletricPistolAnim()
    {
        fpAnimator.CrossFade("TatuZapGunEquip", 0.1f);
    }
    public void EquipEletricPistol(Transform eletricPistolTransform)
    {
        fpAnimator.CrossFade("TatuZapGunIdle", 0.1f);
    }
    public void UnequipEletricPistol(Transform eletricPistolTransform)
    {
        fpAnimator.CrossFade("TatuZapGunUnequip", 0.1f);
    }
    public void OnEletricGunFire()
    {
        fpAnimator.SetTrigger("zapGunShoot");
    }
    public void ToggleEletricPistolCharge(bool state)
    {
        fpAnimator.SetBool("zapGunIsCharging", state);
    }
    public void ToggleEletricPistolOverheat(bool state)
    {
        fpAnimator.SetBool("zapGunOverheat", state);
    }

    #endregion
    #endregion
}

public class CrossFadeLayer
{
    public Coroutine crossFadeCoroutine;
    public int previousLayer;
    public int newLayer;
    public float transitionAmount;
}
