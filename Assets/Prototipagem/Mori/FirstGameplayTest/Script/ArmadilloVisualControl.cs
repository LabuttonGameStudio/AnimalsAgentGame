using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using static WeaponData;
using static ArmadilloPlayerController;
using Unity.VisualScripting;

public class ArmadilloVisualControl : MonoBehaviour
{
    public static readonly float crossFadeTime = 0.2f;
    private enum CameraMode
    {
        FP,
        TP
    }
    [SerializeField] private CameraMode m_CameraMode;
    private bool isClimbing;
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
    [HideInInspector] public bool isArmVisible;

    [Header("VFX")]
    [SerializeField] private ParticleSystem transformationSmoke;
    [SerializeField] private ParticleSystem TreckOnomatopeia;
    [SerializeField] private ParticleSystem onBallHitParticle;

    private void Awake()
    {
        isArmVisible = true;
    }
    private void Start()
    {
        StartCoroutine(ArmFollowBodyVelocity_Coroutine());
        OnWalkStart();
    }
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
    #region Arms
    public void ToggleArmView(bool state)
    {
        fpAnimator.transform.GetChild(0).gameObject.SetActive(state);
        isArmVisible = state;
    }
    private IEnumerator ArmFollowBodyVelocity_Coroutine()
    {
        Vector3 defaultPosition = fpAnimator.transform.localPosition;
        Rigidbody rb = ArmadilloPlayerController.Instance.movementControl.rb;
        Vector3 currentOffset = rb.transform.InverseTransformDirection(rb.velocity) / 100;
        while (true)
        {
            currentOffset = Vector3.Lerp(currentOffset, rb.transform.InverseTransformDirection(rb.velocity), Time.fixedDeltaTime * 10);
            if (currentOffset != Vector3.zero)
            {
                Vector3 offset = currentOffset;
                offset = offset / 200f;
                fpAnimator.transform.localPosition = defaultPosition + offset;
            }
            yield return new WaitForFixedUpdate();
        }
    }
    #endregion

    #region Action Layers Animator
    public void ChangeCurrentLayer(int newLayerInt)
    {
        if (crossFadeLayers_Ref != null)
        {
            StopCoroutine(crossFadeLayers_Ref.crossFadeCoroutine);
            fpAnimator.SetLayerWeight(crossFadeLayers_Ref.previousLayer, 0);
            fpAnimator.SetLayerWeight(crossFadeLayers_Ref.newLayer, 1);
            crossFadeLayers_Ref = null;
        }
        crossFadeLayers_Ref = new CrossFadeLayer();
        crossFadeLayers_Ref.previousLayer = ArmadilloPlayerController.Instance.currentLayer + 1;
        crossFadeLayers_Ref.newLayer = newLayerInt + 1;
        crossFadeLayers_Ref.transitionAmount = 0;
        crossFadeLayers_Ref.crossFadeCoroutine = StartCoroutine(CrossFadeLayers_Coroutine(crossFadeTime));
    }
    private CrossFadeLayer crossFadeLayers_Ref;
    private IEnumerator CrossFadeLayers_Coroutine(float crossFadeDuration)
    {
        float timer = 0;
        while (timer <= crossFadeDuration)
        {
            crossFadeLayers_Ref.transitionAmount = timer / crossFadeDuration;
            fpAnimator.SetLayerWeight(crossFadeLayers_Ref.previousLayer, 1 - crossFadeLayers_Ref.transitionAmount);
            fpAnimator.SetLayerWeight(crossFadeLayers_Ref.newLayer, crossFadeLayers_Ref.transitionAmount);
            timer += Time.deltaTime;
            yield return null;
        }
        fpAnimator.SetLayerWeight(crossFadeLayers_Ref.previousLayer, 0);
        fpAnimator.SetLayerWeight(crossFadeLayers_Ref.newLayer, 1);
    }
    #endregion

    #region LedgeGrab & Climb
    public void OnLedgeGrab()
    {
        ArmadilloPlayerController.Instance.ChangeCurrentActionLayer(2, (int)FPModeLayer2.LedgeGrab);
        if (ledgeGrabTimer_Ref != null) StopCoroutine(ledgeGrabTimer_Ref);
        ledgeGrabTimer_Ref = StartCoroutine(LedgeGrabTimer_Coroutine());
    }
    private Coroutine ledgeGrabTimer_Ref;
    public IEnumerator LedgeGrabTimer_Coroutine()
    {
        fpAnimator.CrossFade("TatuMoveLedgeGrab", 0.02f);
        yield return new WaitForSeconds(0.02f);
        yield return new WaitForSeconds(0.531f * 2f);
        if (ArmadilloPlayerController.Instance.fp_Layer2 == FPModeLayer2.LedgeGrab)
        {
            ArmadilloPlayerController.Instance.ChangeCurrentActionLayer(2, (int)FPModeLayer2.Null);
            ArmadilloPlayerController.Instance.UpdateCurrentLayer();
        }
        ledgeGrabTimer_Ref = null;
        ArmadilloPlayerController.Instance.canSwitchWeapon = true;
    }
    public void OnStartClimbing()
    {
        isClimbing = true;
        ArmadilloPlayerController.Instance.ChangeCurrentActionLayer(2, (int)FPModeLayer2.Climb);
        fpAnimator.SetBool("isSneaking", false);
        fpAnimator.SetBool("isRunning", false);
    }
    public void OnClimbingStart()
    {
        OnStartClimbing();
        ArmadilloPlayerController.Instance.weaponControl.ToggleArms(false);
        ArmadilloPlayerController.Instance.weaponControl.ToggleWeapon(false, false);
    }

    public void OnClimbingStop()
    {
        if (!isClimbing) return;
        isClimbing = false;
        ArmadilloPlayerController.Instance.weaponControl.ToggleArms(true);
        ArmadilloPlayerController.Instance.weaponControl.ToggleWeapon(true, false);
        ArmadilloPlayerController.Instance.ChangeCurrentActionLayer(2, (int)FPModeLayer2.Null);
        ArmadilloPlayerController.Instance.UpdateCurrentLayer();
        switch (ArmadilloPlayerController.Instance.fp_Layer0)
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

    #region Walk
    private PivotOffsetStats walkOffSet;
    public void OnWalkStart()
    {
        if(walkOffSet == null)
        {
            Vector3 direction = Vector3.up / 40;
            walkOffSet = FPCameraShake.Instance.StartPivotOffsetLoop(12, Vector3.zero);
            checkWalkMovement_Ref = StartCoroutine(CheckWalkMovement_Coroutine(walkOffSet, direction));
        }
    }
    private Coroutine checkWalkMovement_Ref;
    private IEnumerator CheckWalkMovement_Coroutine(PivotOffsetStats offsetStats, Vector3 offsetRealValue)
    {
        ArmadilloMovementController movementController = ArmadilloPlayerController.Instance.movementControl;
        while (true)
        {
            if (movementController.movementInputVector.magnitude > 0.1f)
            {
                offsetStats.offset = offsetRealValue;
            }
            else
            {
                offsetStats.offset = Vector3.zero;

            }
            yield return new WaitForFixedUpdate();
        }
    }
    public void OnWalkStop()
    {
        if (walkOffSet != null)
        {
            StopCoroutine(checkWalkMovement_Ref);
            checkWalkMovement_Ref = null;
            FPCameraShake.Instance.StopPivotOffset(walkOffSet);
            walkOffSet = null;
        }
    }
    #endregion

    #region Sprint
    private PivotOffsetStats sprintOffSet;
    public void OnSprintStart()
    {
        if (ArmadilloPlayerController.Instance.fp_Layer0 == FPModeLayer0.Lurking)
        {
            ToggleLurking(false);
        }
        ArmadilloPlayerController.Instance.fp_Layer0 = FPModeLayer0.Sprinting;
        ToggleRun(true);
        Vector3 direction = Vector3.up / 25 + Vector3.right / 50;
        if (sprintOffSet == null)
        {
            sprintOffSet = FPCameraShake.Instance.StartPivotOffsetLoop(16, direction);
            PlayerCamera playerCamera = ArmadilloPlayerController.Instance.cameraControl;
            playerCamera.StartLerpSprintZoom(1.025f, 0.2f);
        }
    }
    public void OnSprintStop()
    {
        if (ArmadilloPlayerController.Instance.fp_Layer0 == FPModeLayer0.Sprinting) ArmadilloPlayerController.Instance.fp_Layer0 = FPModeLayer0.Walking;
        ToggleRun(false);
        if (sprintOffSet != null)
        {
            FPCameraShake.Instance.StopPivotOffset(sprintOffSet);
            PlayerCamera playerCamera = ArmadilloPlayerController.Instance.cameraControl;
            playerCamera.StartLerpSprintZoom(1f, 0.2f);
            sprintOffSet = null;
        }
    }
    private void ToggleRun(bool state)
    {
        if (isClimbing) return;
        fpAnimator.SetBool("isRunning", state);
    }
    #endregion

    #region Lurk
    private PivotOffsetStats lurkOffSet;
    private PivotOffsetStats lurkOffSet1;
    public void OnLurkStart()
    {
        if (ArmadilloPlayerController.Instance.fp_Layer0 == FPModeLayer0.Sprinting)
        {
            ToggleRun(false);
        }
        ArmadilloPlayerController.Instance.fp_Layer0 = FPModeLayer0.Lurking;
        ToggleLurking(true);
        if (lurkOffSet == null)
        {
            Vector3 direction = Vector3.up / 35;
            lurkOffSet = FPCameraShake.Instance.StartPivotOffsetLoop(6, Vector3.zero);
            checkLurkMovement_Ref = StartCoroutine(CheckLurkMovement_Coroutine(lurkOffSet, direction));

            PlayerCamera playerCamera = ArmadilloPlayerController.Instance.cameraControl;
            playerCamera.StartLerpSprintZoom(0.95f, 0.2f);

            Vector3 lowerCamera = Vector3.down / 30;
            lurkOffSet1 = FPCameraShake.Instance.StartPivotOffsetFixed(lowerCamera, 0.2f);
        }
    }
    private Coroutine checkLurkMovement_Ref;
    private IEnumerator CheckLurkMovement_Coroutine(PivotOffsetStats offsetStats, Vector3 offsetRealValue)
    {
        ArmadilloMovementController movementController = ArmadilloPlayerController.Instance.movementControl;
        while(true)
        {
            if(movementController.movementInputVector.magnitude>0.1f)
            {
                offsetStats.offset = offsetRealValue;
            }
            else
            {
                offsetStats.offset = Vector3.zero;

            }
            yield return new WaitForFixedUpdate();
        }
    }
    public void OnLurkStop()
    {
        if (ArmadilloPlayerController.Instance.fp_Layer0 == FPModeLayer0.Lurking) ArmadilloPlayerController.Instance.fp_Layer0 = FPModeLayer0.Walking;
        ToggleLurking(false);
        if (lurkOffSet != null)
        {
            PlayerCamera playerCamera = ArmadilloPlayerController.Instance.cameraControl;
            playerCamera.StartLerpSprintZoom(1f, 0.2f);

            lurkOffSet1.fadeOutTime = 0.2f;
            FPCameraShake.Instance.StoptPivotOffsetFixed(lurkOffSet1);
            lurkOffSet1 = null;
            StopCoroutine(checkLurkMovement_Ref);
            checkLurkMovement_Ref = null;
            FPCameraShake.Instance.StopPivotOffset(lurkOffSet);
            lurkOffSet = null;
        }
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
        fpAnimator.CrossFade("TatuSkillSonar", crossFadeTime);
        StartCoroutine(OnSonarAnimEnd_Coroutine(crossFadeTime * 2 + 0.75f));
    }
    private IEnumerator OnSonarAnimEnd_Coroutine(float duration)
    {
        yield return new WaitForSeconds(duration);
        if (ArmadilloPlayerController.Instance.fp_Layer2 == FPModeLayer2.Sonar)
        {
            ArmadilloPlayerController.Instance.ChangeCurrentActionLayer(2, (int)FPModeLayer2.Null);
            ArmadilloPlayerController.Instance.UpdateCurrentLayer();
        }
    }
    #endregion

    #region Pause
    public void OnPause()
    {
        ArmadilloPlayerController.Instance.ChangeCurrentActionLayer(4, (int)FPModeLayer4.Pause);
        fpAnimator.CrossFade("TatuPauseIdle", crossFadeTime);
    }

    public void ReturnPause()
    {
        fpAnimator.CrossFade("Idle", crossFadeTime);
        ArmadilloPlayerController.Instance.ChangeCurrentActionLayer(4, (int)FPModeLayer4.Null);
        ArmadilloPlayerController.Instance.UpdateCurrentLayer();
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
        toggleWeaponDelay_Ref = null;
    }
    public void OnGunEquiped(WeaponType weaponType)
    {
        switch (weaponType)
        {
            case WeaponType.Zapgun:
                ArmadilloPlayerController.Instance.fp_Layer1 = FPModeLayer1.ZapGun;
                break;
            case WeaponType.Watergun:
                ArmadilloPlayerController.Instance.fp_Layer1 = FPModeLayer1.WaterGun;
                break;
            case WeaponType.Pendrivegun:
                ArmadilloPlayerController.Instance.fp_Layer1 = FPModeLayer1.PendriveGun;
                break;
        }
        if (ArmadilloPlayerController.Instance.CheckIfActionIsPossible(1))
        {
            ArmadilloPlayerController.Instance.ChangeCurrentActionLayer(1, (int)ArmadilloPlayerController.Instance.fp_Layer1);
        }
    }

    #endregion

    #region EletricPistol
    public void EquipEletricPistolAnim()
    {
        fpAnimator.CrossFade("TatuZapGunEquip", 0.1f);
    }
    public void EquipEletricPistol()
    {
        fpAnimator.CrossFade("TatuZapGunIdle", crossFadeTime);
    }
    public void UnequipEletricPistol()
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

    #region WaterGun
    public void EquipWaterGunAnim()
    {
        fpAnimator.CrossFade("TatuWaterGunEquip", 0.1f);
    }
    public void EquipWaterGun()
    {
        fpAnimator.CrossFade("TatuWaterGunIdle", crossFadeTime);
    }
    public void ReloadWaterGun()
    {
        fpAnimator.SetTrigger("waterGunReload");
    }
    public void ToggleOnFireWaterGun(bool state)
    {
        fpAnimator.SetBool("waterGunIsFiring", state);
    }
    #endregion

    #region On Air
    private ShakeStats onAirShake;
    private PivotOffsetStats onAirOffset;
    public void EnterOnAir()
    {
        if (onAirShake == null) onAirShake = FPCameraShake.StartShake(0.1f, 0.2f);
    }
    public void ExitOnAir()
    {
        if (onAirShake != null)
        {
            FPCameraShake.StopShake(onAirShake);
            onAirShake = null;
        }
        FPCameraShake.StartShake(0.15f, 0.2f, 0.1f);
        FPCameraShake.Instance.StartPivotOffsetFixed(0.05f, Vector3.down / 10, 0.2f, 0.2f);
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
