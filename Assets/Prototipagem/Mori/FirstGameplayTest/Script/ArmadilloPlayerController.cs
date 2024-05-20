using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem.XInput;
using UnityEngine.InputSystem;
using System.Linq;
using UnityEngine.Rendering.Universal;
public class ArmadilloForms
{
    public enum ArmadilloForm
    {
        Default = 0,
        Ball = 1
    }
}

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

    //Armadillo Components
    [System.NonSerialized] public ArmadilloPlayerInputController inputControl;
    [System.NonSerialized] public ArmadilloMovementController movementControl;
    [System.NonSerialized] public PlayerCamera cameraControl;
    [System.NonSerialized] public ArmadilloInteractController interactControl;
    [System.NonSerialized] public ArmadilloHPControl hpControl;
    [System.NonSerialized] public ArmadilloWeaponControl weaponControl;
    [System.NonSerialized] public ArmadilloPickUpControl pickUpControl;
    [System.NonSerialized] public ArmadilloVisualControl visualControl;

    //Player Forms
    [Header("Default Form")]
    [SerializeField] public Collider[] playerCollider_Default;
    [Header("Ball Form")]
    [SerializeField] public Collider[] playerCollider_Ball;

    private int currentEquipedWeapon;
    [Header("Layers")]
    public LayerMask enemyLayer;
    public LayerMask interactiveLayer;
    public LayerMask pickableLayer;
    public LayerMask climbableLayer;

    [Header("Sonar Ability")]

    [SerializeField] private Camera sonarCamera;

    private bool isSonarActive;
    [SerializeField] private float sonarDuration;
    [SerializeField] private float sonarStartLerpDuration;
    [SerializeField] private float sonarRange;


    [SerializeField] private DecalProjector[] sonarDecal;


    private void Awake()
    {
        Instance = this;

        inputControl = GetComponent<ArmadilloPlayerInputController>();
        movementControl = GetComponent<ArmadilloMovementController>();
        cameraControl = GetComponent<PlayerCamera>();
        interactControl = GetComponent<ArmadilloInteractController>();
        hpControl = GetComponent<ArmadilloHPControl>();
        weaponControl = GetComponent<ArmadilloWeaponControl>();
        pickUpControl = GetComponent<ArmadilloPickUpControl>();
        visualControl = GetComponent<ArmadilloVisualControl>();

    }
    private void Start()
    {
        if (startWithChangeFormAbility) UnlockChangeFormAbility();
        if (startWithSonarAbility) UnlockSonarAbility();
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, sonarRange);
    }
    private void OnValidate()
    {
        //sonarEffectDefault = sonarEffect.customPasses[0] as SeeThrough;
        //sonarEffectEnemy = sonarEffect.customPasses[1] as SeeThrough;
        //sonarEffectDefault.seeThroughMaterial.SetFloat("_MAXDISTANCE",sonarRange);
        //sonarEffectEnemy.seeThroughMaterial.SetFloat("_MAXDISTANCE",sonarRange);
    }

    public float GetCurrentVisibilityOfPlayer()
    {
        return 1;
    }

    public void UnlockChangeFormAbility()
    {
        inputControl.inputAction.Armadillo.Ability1.Enable();
        inputControl.inputAction.Armadillo.Ability1.performed += ChangeToBallForm;
    }
    public void ChangeToBallForm(InputAction.CallbackContext value)
    {
        if (Form.Ball == currentForm) return;
        if (changeToBallFormRef == null) changeToBallFormRef = StartCoroutine(ChangeToBallForm_Coroutine());
        currentForm = Form.Ball;
    }

    public void ChangeToDefaultForm(InputAction.CallbackContext value)
    {
        if (Form.Default == currentForm) return;
        if (changeToDefaultFormRef == null) changeToDefaultFormRef = StartCoroutine(ChangeToDefaultForm_Coroutine());
        currentForm = Form.Default;
    }

    //------ Sonar Ability ------

    public void UnlockSonarAbility()
    {
        inputControl.inputAction.Armadillo.Ability2.Enable();
        inputControl.inputAction.Armadillo.Ability2.performed += SonarAbility;
    }
    public void SonarAbility(InputAction.CallbackContext value)
    {
        if (SonarAbility_Ref == null)
        {
            isSonarActive = !isSonarActive;
            if (isSonarActive && SonarAbilityVFX_Ref == null)
            {
                visualControl.OnSonar();
                SonarAbilityVFX_Ref = StartCoroutine(SonarAbilityVFX_Coroutine(sonarStartLerpDuration));
            }
            SonarAbility_Ref = StartCoroutine(SonarAbility_Coroutine(isSonarActive ? 0.33f : sonarRange, isSonarActive ? sonarRange : 0.33f, sonarStartLerpDuration));
        }
    }

    private Coroutine SonarAbility_Ref;
    private IEnumerator SonarAbility_Coroutine(float startValue, float finalValue, float duration)
    {
        yield return new WaitForSeconds(0.55f);

        float timer = 0f;
        while (timer <= duration)
        {

            sonarCamera.farClipPlane = Mathf.SmoothStep(startValue, finalValue, timer / duration);
            timer += 0.05f;
            yield return new WaitForSeconds(0.05f);
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
    //------ Change Forms ------

    private Coroutine changeToBallFormRef;
    private IEnumerator ChangeToBallForm_Coroutine()
    {
        //Move a camera para terceira pessoa em 0.5 segundos 
        currentEquipedWeapon = weaponControl.currentWeaponID;
        weaponControl.ChangeWeapon(-1);
        cameraControl.ChangeCameraState(cameraControl.thirdPersonCameraState);
        visualControl.OnEnterBallMode();
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
        weaponControl.ChangeWeapon(currentEquipedWeapon);
        inputControl.inputAction.Armadillo.Ability1.performed += ChangeToBallForm;
        changeToDefaultFormRef = null;
    }

    //----- Die -----
    public void Die()
    {
        Debug.Log("Player Die");
    }

}
