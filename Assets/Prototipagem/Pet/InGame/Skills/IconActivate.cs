using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IconActivate : MonoBehaviour
{
    [Header("ANIMACAO")]
    public GameObject objectToAnimate; //objeto que vai animar em pop up
    public float popUpDuration = 0.5f;           
    public Tween.LerpType lerpType = Tween.LerpType.Lerp;

    [Header("ICON")]
    public Graphic objectToChangeMaterial1; // icon background
    public Graphic objectToChangeMaterial2;   // input background

    [Header("MATERIAL")]
    public Material activeMaterial; //cor do icon
    public Material activeMaterial2; // cor do input

    [Header("SCRIPT")]
    public MonoBehaviour scriptToActivate; //ativar/desativar outros tweens

    [Header("INPUT")]
    public KeyCode inputKey = KeyCode.Q; // reutilizar o cod para outros inputs -- pensar em array e index dps

    private bool isActive = false;           
    private Material originalMaterial1; //retornar material orig
    private Material originalMaterial2;         

    void Start()
    {
        originalMaterial1 = objectToChangeMaterial1.material;
        originalMaterial2 = objectToChangeMaterial2.material;
    }

    void Update()
    {
        if (Input.GetKeyDown(inputKey))
        {
            if (!isActive)
            {
                StartCoroutine(ActivatePopUp());
            }
            else
            { 
                StartCoroutine(DeactivatePopUp());
            }
        }
    }

    IEnumerator ActivatePopUp()
    {
        //  animacao de pop up (escala de 1.0 para 1.2 e depois para 0)
        yield return StartCoroutine(PopUpAnimationRoutine(objectToAnimate.transform, Vector3.one, new Vector3(1.2f, 1.2f, 1.2f), Vector3.zero));

        // troca materiais
        objectToChangeMaterial1.material = activeMaterial;
        objectToChangeMaterial2.material = activeMaterial;

        // Ativa o script -- nao desativa dps, corrigir
        scriptToActivate.enabled = true;
        isActive = true;

        //  animacao de pop up (escala de 0 para 1.2 e depois para 1)
        yield return StartCoroutine(PopUpAnimationRoutine(objectToAnimate.transform, Vector3.zero, new Vector3(1.2f, 1.2f, 1.2f), Vector3.one));
    }

    IEnumerator DeactivatePopUp()
    {
        yield return StartCoroutine(PopUpAnimationRoutine(objectToAnimate.transform, Vector3.one, new Vector3(1.2f, 1.2f, 1.2f), Vector3.zero));

        // Restaura os materiais originais dos objetos
        objectToChangeMaterial1.material = originalMaterial1;
        objectToChangeMaterial2.material = originalMaterial2;

        // nao desativa a funcao de while(true), verificar dps
        scriptToActivate.enabled = false;
        isActive = false;

        yield return StartCoroutine(PopUpAnimationRoutine(objectToAnimate.transform, Vector3.zero, new Vector3(1.2f, 1.2f, 1.2f), Vector3.one));
    }

    IEnumerator PopUpAnimationRoutine(Transform target, Vector3 startScale, Vector3 midScale, Vector3 endScale) // nao encavalar as anim
    {
        yield return Tween.ScaleTransform(this, target, midScale, popUpDuration / 2, lerpType);
        yield return Tween.ScaleTransform(this, target, endScale, popUpDuration / 2, lerpType);
    }
}

