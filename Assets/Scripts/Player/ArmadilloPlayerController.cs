using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem.XInput;
using UnityEngine.InputSystem;
using System.Linq;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering;

public class ArmadilloPlayerController : MonoBehaviour
{
    public static ArmadilloPlayerController Instance { get; private set; }

    [SerializeField] private bool startWithChangeFormAbility;
    [SerializeField] private bool startWithSonarAbility;
    public enum Form
    {
        Default = 0,
        Ball = 1
    }
    public Form currentForm;

    //-----Armadillo Components-----
    #region Armadillo Components Variables
    [System.NonSerialized] public ArmadilloPlayerInputController inputControl;
    [System.NonSerialized] public ArmadilloMovementController movementControl;
    [System.NonSerialized] public PlayerCamera cameraControl;
    [System.NonSerialized] public ArmadilloInteractController interactControl;
    [System.NonSerialized] public ArmadilloHPControl hpControl;
    [System.NonSerialized] public ArmadilloWeaponControl weaponControl;
    [System.NonSerialized] public ArmadilloPickUpControl pickUpControl;
    [System.NonSerialized] public ArmadilloVisualControl visualControl;
    [System.NonSerialized] public ArmadilloAudioControl audioControl;
    [System.NonSerialized] public ArmadilloLOSControl losControl;
    #endregion

    //-----Player Forms-----
    #region Player Forms Variables
    [Header("Default Form")]
    [SerializeField] private IconActivate formChangeIcon;
    [SerializeField] public Collider[] playerCollider_Default;
    [Header("Ball Form")]
    [SerializeField] public Collider[] playerCollider_Ball;
    [SerializeField] public float ballShieldAmount;
    #endregion

    //-----Layers-----
    #region Layer Mask Variables
    [Header("Layer Mask")]
    public LayerMask enemyLayer;
    public LayerMask interactiveLayer;
    public LayerMask pickableLayer;
    public LayerMask climbableLayer;
    #endregion

    //-----Sonar Ability-----
    #region Sonar Ability Variables
    [Header("Sonar Ability")]
    [SerializeField] private IconActivate sonarIcon;
    [SerializeField] private Camera sonarCamera;
    [SerializeField] private float sonarDuration;
    [SerializeField] private float sonarStartLerpDuration;
    [SerializeField] private float sonarRange;
    [SerializeField] private DecalProjector[] sonarDecal;
    [SerializeField] private CanvasGroup sonarCanvasGroup;
    [SerializeField] private Volume sonarPostProcess;
    private bool isSonarActive;
    #endregion

    [HideInInspector] public bool canSwitchWeapon;

    //----- Action Layers -----
    #region Action Layers Variables
    public enum FPModeLayer0
    {
        Idle,
        Walking,
        Sprinting,
        Lurking
    }
    [HideInInspector] public FPModeLayer0 fp_Layer0;

    public enum FPModeLayer1
    {
        Null,
        ZapGun,
        WaterGun,
        PendriveGun,
    }
    [HideInInspector] public FPModeLayer1 fp_Layer1;

    public enum FPModeLayer2
    {
        Null,
        Hold,
        Interact,
        Sonar,
        Melee,
        LedgeGrab,
        Climb
    }
    [HideInInspector] public FPModeLayer2 fp_Layer2;

    public enum FPModeLayer3
    {
        Null,
        Ball,
        Climb,
    }
    [HideInInspector] public FPModeLayer3 fp_Layer3;

    public enum FPModeLayer4
    {
        Null,
        Pause
    }
    [HideInInspector] public FPModeLayer4 fp_Layer4;

    [HideInInspector] public int currentLayer;
    [HideInInspector] public int currentAction;

    #endregion

    private void Awake()
    {
        canSwitchWeapon = true;
        Instance = this;
        #region Assign Components
        inputControl = GetComponent<ArmadilloPlayerInputController>();
        movementControl = GetComponent<ArmadilloMovementController>();
        cameraControl = GetComponent<PlayerCamera>();
        interactControl = GetComponent<ArmadilloInteractController>();
        hpControl = GetComponent<ArmadilloHPControl>();
        weaponControl = GetComponent<ArmadilloWeaponControl>();
        pickUpControl = GetComponent<ArmadilloPickUpControl>();
        visualControl = GetComponent<ArmadilloVisualControl>();
        audioControl = GetComponent<ArmadilloAudioControl>();
        losControl = GetComponent<ArmadilloLOSControl>();
        #endregion
        Application.targetFrameRate = 300;

    }
    private void Start()
    {
        if (startWithChangeFormAbility) UnlockChangeFormAbility();
        if (startWithSonarAbility) UnlockSonarAbility();
    }
    public void TeleportPlayer(Transform transform)
    {
        gameObject.transform.position = transform.position;
        movementControl.rb.MovePosition(transform.position);
    }
    public void TogglePlayerControls(bool state)
    {
        movementControl.enabled = state;
    }
    public void TogglePlayerOnDialogue(bool state)
    {
        if (state)
        {
            Time.timeScale = 1;
        }
        else
        {
            Time.timeScale = 0;
        }
    }
    public float GetCurrentVisibilityOfPlayer()
    {
        return 1;
    }

    //-----Change Form Functions-----
    #region Change Form Functions
    public void UnlockChangeFormAbility()
    {
        inputControl.inputAction.Armadillo.Ability1.Enable();
        inputControl.inputAction.Armadillo.Ability1.performed += ChangeToBallForm;
    }
    public void ChangeToBallForm()
    {
        if (Form.Ball == currentForm) return;
        if (changeToBallFormRef == null) changeToBallFormRef = StartCoroutine(ChangeToBallForm_Coroutine());
        formChangeIcon.ActivatePopUp();
        currentForm = Form.Ball;
    }
    public void ChangeToBallForm(InputAction.CallbackContext value)
    {
        if (Form.Ball == currentForm) return;
        if (changeToBallFormRef == null) changeToBallFormRef = StartCoroutine(ChangeToBallForm_Coroutine());
        formChangeIcon.ActivatePopUp();
        currentForm = Form.Ball;
    }

    public void ChangeToDefaultForm()
    {
        if (Form.Default == currentForm) return;
        if (changeToDefaultFormRef == null) changeToDefaultFormRef = StartCoroutine(ChangeToDefaultForm_Coroutine());
        formChangeIcon.DeactivatePopUp();
        currentForm = Form.Default;
    }
    public void ChangeToDefaultForm(InputAction.CallbackContext value)
    {
        if (Form.Default == currentForm) return;
        if (changeToDefaultFormRef == null) changeToDefaultFormRef = StartCoroutine(ChangeToDefaultForm_Coroutine());
        formChangeIcon.DeactivatePopUp();
        currentForm = Form.Default;
    }
    #endregion

    //------Sonar Ability------
    #region Sonar Ability
    public void UnlockSonarAbility()
    {
        inputControl.inputAction.Armadillo.Ability2.Enable();
        inputControl.inputAction.Armadillo.Ability2.performed += SonarAbility;
    }
    public void SonarAbility(InputAction.CallbackContext value)
    {
        float minimalValue = 0.33f;
        if (SonarAbility_Ref == null && SonarAbilityVFX_Ref == null)
        {
            if (!isSonarActive)
            {
                if (CheckIfActionIsPossible(2) && visualControl.isArmVisible)
                {
                    visualControl.OnSonar();
                    ChangeCurrentActionLayer(2, (int)FPModeLayer2.Sonar);
                }
                SonarAbilityVFX_Ref = StartCoroutine(SonarAbilityVFX_Coroutine(sonarStartLerpDuration));
                SonarAbility_Ref = StartCoroutine(SonarAbility_Coroutine(minimalValue, sonarRange, sonarStartLerpDuration, true));
                sonarIcon.ActivatePopUp();
                isSonarActive = true;
            }
            else
            {
                SonarAbility_Ref = StartCoroutine(SonarAbility_Coroutine(sonarRange, minimalValue, sonarStartLerpDuration, false));
                sonarIcon.DeactivatePopUp();
                isSonarActive = false;
            }
        }
    }

    private Coroutine SonarAbility_Ref;
    private IEnumerator SonarAbility_Coroutine(float startValue, float finalValue, float duration, bool toggle)
    {
        yield return new WaitForSeconds(0.55f);
        float timer = 0f;
        if (toggle)
        {
            sonarCamera.enabled = true;
            while (timer <= duration)
            {
                sonarCamera.farClipPlane = Mathf.SmoothStep(startValue, finalValue, timer / duration);
                sonarPostProcess.weight = Mathf.Min(1, timer * 1.25f / duration);
                sonarCanvasGroup.alpha = Mathf.Min(1, timer * 1.25f / duration);
                timer += 0.05f;
                yield return new WaitForSeconds(0.05f);
            }
        }
        else
        {
            while (timer <= duration)
            {
                sonarCamera.farClipPlane = Mathf.SmoothStep(startValue, finalValue, timer / duration);
                sonarPostProcess.weight = 1 - Mathf.Min(1, timer * 1.25f / duration);
                sonarCanvasGroup.alpha = 1 - Mathf.Min(1, timer * 1.25f / duration);
                timer += 0.05f;
                yield return new WaitForSeconds(0.05f);
            }
            sonarCamera.enabled = false;
        }
        sonarCamera.farClipPlane = finalValue;
        SonarAbility_Ref = null;
    }
    private Coroutine SonarAbilityVFX_Ref;
    private IEnumerator SonarAbilityVFX_Coroutine(float duration)
    {
        yield return new WaitForSeconds(0.55f);
        float timer = 0f;
        while (timer <= duration)
        {
            sonarDecal[0].size = Vector3.one * Mathf.SmoothStep(0, 40 + sonarRange * 2.5f, timer / duration);
            sonarDecal[1].size = Vector3.one * Mathf.SmoothStep(0, sonarRange * 2.5f, timer / duration);
            sonarDecal[2].size = Vector3.one * Mathf.SmoothStep(0, -40 + sonarRange * 2.5f, timer / duration);
            sonarDecal[3].size = Vector3.one * Mathf.SmoothStep(0, -90 + sonarRange * 2.5f, timer / duration);


            sonarDecal[0].fadeFactor = Mathf.SmoothStep(2.75f, 0, timer / duration);
            sonarDecal[1].fadeFactor = Mathf.SmoothStep(3, 0, timer / duration);
            sonarDecal[2].fadeFactor = Mathf.SmoothStep(2.75f, 0, timer / duration);
            sonarDecal[3].fadeFactor = Mathf.SmoothStep(1.5f, 0, timer / duration);


            timer += Time.deltaTime;
            yield return null;
        }
        sonarDecal[0].size = Vector3.one * sonarRange * 2.5f;
        sonarDecal[1].size = Vector3.one * sonarRange * 2.5f;
        sonarDecal[2].size = Vector3.one * sonarRange * 2.5f;
        sonarDecal[3].size = Vector3.one * sonarRange * 2.5f;

        sonarDecal[0].fadeFactor = 0;
        sonarDecal[1].fadeFactor = 0;
        sonarDecal[2].fadeFactor = 0;
        sonarDecal[3].fadeFactor = 0;

        SonarAbilityVFX_Ref = null;
    }
    #endregion

    //------Change Forms Coroutines------
    #region Change Form Coroutine

    private Coroutine changeToBallFormRef;
    private IEnumerator ChangeToBallForm_Coroutine()
    {
        //Stop CheckLOS e solta objeto
        losControl.StopLOSCheck();
        pickUpControl.Drop();
        movementControl.ExitLadder();

        //Move a camera para terceira pessoa em 0.5 segundos 
        ChangeCurrentActionLayer(3, (int)FPModeLayer3.Ball);
        cameraControl.ChangeCameraState(cameraControl.thirdPersonCameraState);
        visualControl.OnEnterBallMode();
        audioControl.onTransformSound.PlayAudio();
        sonarCamera.cullingMask = cameraControl.thirdPersonSeeThroughMask;
        //Muda o visual do personagem, futuramente so colocar a opcao de mudar a animacao do model
        //Retira a funcao do input de transformar em bola e altera a state machine pra interpretar como o modo bola
        inputControl.inputAction.Armadillo.Ability1.performed -= ChangeToBallForm;

        //Troca os colliders ativos
        foreach (Collider collider in playerCollider_Ball)
        {
            collider.enabled = true;
        }
        foreach (Collider collider in playerCollider_Default)
        {
            collider.enabled = false;
        }

        //Muda a state machine do move contoler para funcionar como bola
        movementControl.ChangeState(movementControl.ballState);
        //Tempo de animacao
        yield return new WaitForSeconds(0.500f);
        visualControl.PlayTransformationSmoke();
        yield return new WaitForSeconds(0.063f);
        visualControl.ChangeMeshToBall();
        //Espera 0.225 segundos pra dar tempo pro model do player em modo bola encostar o chao, tempo de travessia da camera e evitar spam de transformacao 
        yield return new WaitForSeconds(0.225f);

        inputControl.inputAction.Armadillo.Ability1.performed += ChangeToDefaultForm;

        changeToBallFormRef = null;
    }

    private Coroutine changeToDefaultFormRef;
    private IEnumerator ChangeToDefaultForm_Coroutine()
    {
        //Stop CheckLOS
        losControl.StartLOSCheck();

        audioControl.onTransformSound.PlayAudio();
        //Move a camera para terceira pessoa em 0.5 segundos 
        cameraControl.ChangeCameraState(cameraControl.firstPersonCameraState);

        //Muda o visual do personagem, futuramente so colocar a opcao de mudar a animacao do model
        visualControl.ChangeMeshToDefault();

        //Retira a funcao do input de transformar no modo normal e altera a state machine pra interpretar como modo normal
        inputControl.inputAction.Armadillo.Ability1.performed -= ChangeToDefaultForm;

        //Aplica uma forca para cima para dar espaco pra troca de collider para de capsula 
        movementControl.rb.AddForce(transform.up * 5f, ForceMode.Impulse);

        //Espera 0.25 segundos para a forca ter tempo de mover o objeto e o tempo de travessia da camera
        yield return new WaitForSeconds(0.25f);
        //Troca os colliders
        foreach (Collider collider in playerCollider_Default)
        {
            collider.enabled = true;
        }
        foreach (Collider collider in playerCollider_Ball)
        {
            collider.enabled = false;
        }

        //Muda a state machine do move contoler para funcionar como bola
        movementControl.ChangeState(movementControl.defaultState);

        //Espera 0.25 segundos para deixar o objeto cair ate encostar no chao e evitar spam de transformacao 
        yield return new WaitForSeconds(0.25f);
        sonarCamera.cullingMask = cameraControl.firstPeronSeeThroughMask;
        visualControl.OnEnterDefaultMode();
        ChangeCurrentActionLayer(3, (int)FPModeLayer3.Null);
        ArmadilloPlayerController.Instance.UpdateCurrentLayer();
        inputControl.inputAction.Armadillo.Ability1.performed += ChangeToBallForm;
        changeToDefaultFormRef = null;
    }

    #endregion

    //-----Die-----
    #region Die
    //----- Die -----
    public void Die()
    {
        Debug.Log("Player Die");
    }
    #endregion

    //-----Action Layers-----
    #region Action Layers Functions
    public void ChangeCurrentActionLayer(int newLayer, int newActionID)
    {
        //Get current action and layer
        int currentLayer = GetCurrentFPLayer();
        int currentAction;
        switch (currentLayer)
        {
            default:
            case 0:
                currentAction = (int)fp_Layer0;
                break;
            case 1:
                currentAction = (int)fp_Layer1;
                break;
            case 2:
                currentAction = (int)fp_Layer2;
                break;
            case 3:
                currentAction = (int)fp_Layer3;
                break;
            case 4:
                currentAction = (int)fp_Layer4;
                break;
        }

        //If for some whatever reason it changes to the same return
        if (this.currentLayer == newLayer && this.currentAction == newActionID) return;

        //Leave current action and layer

        //Change current active layer and action
        visualControl.ChangeCurrentLayer(newLayer);
        this.currentLayer = newLayer;
        this.currentAction = newActionID;
        OnLeaveLayer(currentLayer, currentAction);
        switch (newLayer)
        {
            default:
            case 0:
                fp_Layer0 = (FPModeLayer0)newActionID;
                break;
            case 1:
                fp_Layer1 = (FPModeLayer1)newActionID;
                break;
            case 2:
                fp_Layer2 = (FPModeLayer2)newActionID;
                break;
            case 3:
                fp_Layer3 = (FPModeLayer3)newActionID;
                break;
            case 4:
                fp_Layer4 = (FPModeLayer4)newActionID;
                break;
        }

        //Enter new layer and action
        OnEnterLayer(newLayer);
    }
    public void UpdateCurrentLayer()
    {
        int newLayer = GetCurrentFPLayer();
        if (currentLayer != newLayer)
        {
            OnLeaveLayer(currentLayer);
            OnEnterLayer(newLayer);
            visualControl.ChangeCurrentLayer(newLayer);
            currentLayer = newLayer;
        }
    }
    public void OnEnterLayer(int layer)
    {
        switch (layer)
        {
            case 0:
                switch (fp_Layer0)
                {

                }
                break;

            case 1:
                ArmadilloPlayerController.Instance.weaponControl.ToggleWeapon(true, false);
                switch (fp_Layer1)
                {
                    case FPModeLayer1.ZapGun:
                        break;
                    case FPModeLayer1.WaterGun:
                        break;
                }
                break;

            case 2:
                switch (fp_Layer2)
                {

                }
                break;

            case 3:
                switch (fp_Layer3)
                {

                }
                break;
        }
    }
    public void OnLeaveLayer(int layer)
    {
        switch (layer)
        {
            case 0:
                switch (fp_Layer0)
                {

                }
                break;

            case 1:
                ArmadilloPlayerController.Instance.weaponControl.ToggleWeapon(false, false);
                switch (fp_Layer1)
                {
                    case FPModeLayer1.ZapGun:
                        break;
                    case FPModeLayer1.WaterGun:
                        break;
                }
                break;

            case 2:
                switch (fp_Layer2)
                {

                }
                break;

            case 3:
                switch (fp_Layer3)
                {

                }
                break;

            case 4:
                switch (fp_Layer4)
                {

                }
                break;
        }
    }
    public void OnLeaveLayer(int layer, int enumID)
    {
        switch (layer)
        {
            case 0:
                switch (enumID)
                {

                }
                break;

            case 1:
                ArmadilloPlayerController.Instance.weaponControl.ToggleWeapon(false, false);
                switch (enumID)
                {
                    case (int)FPModeLayer1.ZapGun:
                        break;
                    case (int)FPModeLayer1.WaterGun:
                        break;
                }
                break;

            case 2:
                switch (enumID)
                {

                }
                break;

            case 3:
                switch (fp_Layer3)
                {

                }
                break;

            case 4:
                switch (fp_Layer4)
                {

                }
                break;
        }
    }
    public int GetCurrentFPLayer()
    {
        if (fp_Layer4 != FPModeLayer4.Null) return 4;
        if (fp_Layer3 != FPModeLayer3.Null) return 3;
        if (fp_Layer2 != FPModeLayer2.Null) return 2;
        if (fp_Layer1 != FPModeLayer1.Null) return 1;
        return 0;
    }
    public bool CheckIfActionIsPossible(int actionLayer)
    {
        return currentLayer <= actionLayer;
    }
    #endregion
}
