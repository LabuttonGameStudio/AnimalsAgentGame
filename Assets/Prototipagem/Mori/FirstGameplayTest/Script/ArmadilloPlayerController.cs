using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem.XInput;
using UnityEngine.InputSystem;
using System.Linq;
using UnityEngine.Rendering.HighDefinition;
using UnityEngine.UI;
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

    [SerializeField]private bool startWithChangeFormAbility;
    [SerializeField]private bool startWithInvisibilityAbility;
    [SerializeField]private bool startWithSonarAbility;
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

    //Player Forms
    [Header("Default Form")]
    [SerializeField] public GameObject playerVisual_Default;
    [SerializeField] public Collider[] playerCollider_Default;
    [Header("Ball Form")]
    [SerializeField] public GameObject playerVisual_Ball;
    [SerializeField] public Collider[] playerCollider_Ball;

    private int currentEquipedWeapon;
    [Header("Layers")]
    public LayerMask enemyLayer;
    public LayerMask interactiveLayer;
    public LayerMask pickableLayer;
    public LayerMask climbableLayer;

    [Header("Sonar Ability")]
    public CustomPassVolume sonarEffect;

    [SerializeField]private MeshRenderer ballMeshRender;

    public bool isInvisible;

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

    }
    private void Start()
    {
        if(startWithChangeFormAbility) UnlockChangeFormAbility();
        if(startWithSonarAbility) UnlockSonarAbility();
        if (startWithInvisibilityAbility) UnlockInvisibilityAbility();
    }
    private void OnDrawGizmos()
    {
        
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
        sonarEffect.enabled = !sonarEffect.enabled;
        //LayerMask visibleLayers = enemyLayer | interactiveLayer | pickableLayer | climbableLayer;
        //Collider[] colliders = Physics.OverlapSphere(transform.position, sonarRange, visibleLayers, QueryTriggerInteraction.Ignore);
        //colliders = colliders.OrderBy(x => Vector3.Distance(transform.position, x.transform.position)).ToArray();
        //foreach (Collider collider in colliders)
        //{
        //    string entityType;
        //    if (CheckIfObjectIsInLayerMask(collider.gameObject, enemyLayer))
        //    {
        //        entityType = "enemy";
        //    }
        //    else if (CheckIfObjectIsInLayerMask(collider.gameObject, interactiveLayer))
        //    {
        //        entityType = "interactive";
        //    }
        //    else if (CheckIfObjectIsInLayerMask(collider.gameObject, pickableLayer))
        //    {
        //        entityType = "pickable";
        //    }
        //    else
        //    {
        //        entityType = "climbable";
        //    }

        //    Debug.Log(collider.gameObject.name + "|Position=" + collider.transform.position +"| Type="+entityType+ "| Distance= " + Vector3.Distance(transform.position, collider.transform.position)); 
        //}
    }


    //------ Invisibility ------
    public void UnlockInvisibilityAbility()
    {
        inputControl.inputAction.Armadillo.Ability3.Enable();
        inputControl.inputAction.Armadillo.Ability3.performed += InvisibilityAbility;
    }
    public void InvisibilityAbility(InputAction.CallbackContext value)
    {
        isInvisible = !isInvisible;
        ballMeshRender.sharedMaterial.SetFloat("_ALPHA",isInvisible ? 1 : 0.25f);

    }

    //------ Change Forms ------

    private Coroutine changeToBallFormRef;
    private IEnumerator ChangeToBallForm_Coroutine()
    {
        //Move a camera para terceira pessoa em 0.5 segundos 
        currentEquipedWeapon = weaponControl.currentWeaponID;
        weaponControl.ChangeWeapon(-1);
        cameraControl.ChangeCameraState(cameraControl.thirdPersonCameraState);

        //Muda o visual do personagem, futuramente so colocar a opcao de mudar a animacao do model
        playerVisual_Default.SetActive(false);
        playerVisual_Ball.SetActive(true);
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

        //Espera 0.5 segundos pra dar tempo pro model do player em modo bola encostar o chao, tempo de travessia da camera e evitar spam de transformacao 
        yield return new WaitForSeconds(0.5f);

        inputControl.inputAction.Armadillo.Ability1.performed += ChangeToDefaultForm;

        changeToBallFormRef = null;
    }

    private Coroutine changeToDefaultFormRef;
    private IEnumerator ChangeToDefaultForm_Coroutine()
    {
        //Move a camera para terceira pessoa em 0.5 segundos 
        cameraControl.ChangeCameraState(cameraControl.firstPersonCameraState);

        //Muda o visual do personagem, futuramente so colocar a opcao de mudar a animacao do model
        playerVisual_Ball.SetActive(false);
        playerVisual_Default.SetActive(true);

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
