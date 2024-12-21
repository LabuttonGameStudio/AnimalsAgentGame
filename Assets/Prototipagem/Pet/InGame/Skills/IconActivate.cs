using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class IconActivate : MonoBehaviour
{
    [Header("ANIMACAO")]
    [SerializeField] private GameObject objectToAnimate; //objeto que vai animar em pop up
    [SerializeField] private float popUpDuration = 0.5f;
    [SerializeField] private Tween.LerpType lerpType = Tween.LerpType.Lerp;

    [Header("ICON")]
    [SerializeField] private Graphic iconBackgroundGraphic; // Icon Background Graphic
    private Material iconBackgroundOriginalMat; //Icon Material Original

    [SerializeField] private Graphic inputBackgroundGraphic;   // Icon Background Graphic
    private Material inputBackgroundOriginalMat; //Input Material Original        

    [Header("MATERIAL")]
    [SerializeField]private Material iconBackgroundActiveMaterial; //cor do icon
    [SerializeField]private Material inputBackgroundActiveMaterial; // cor do input

    [Header("SCRIPT")]
    [SerializeField] private MonoBehaviour scriptToActivate; //ativar/desativar outros tweens


    private bool isActive = false;

    void Start()
    {
        iconBackgroundOriginalMat = iconBackgroundGraphic.material;
        inputBackgroundOriginalMat = inputBackgroundGraphic.material;
    }


    public void ActivatePopUp()
    {
        if (popUp_Ref != null) StopCoroutine(popUp_Ref);
        popUp_Ref = StartCoroutine(ActivatePopUp_Coroutine());
    }
    public void DeactivatePopUp()
    {
        if (popUp_Ref != null) StopCoroutine(popUp_Ref);
        popUp_Ref = StartCoroutine(DeactivatePopUp_Coroutine());
    }
    public Coroutine popUp_Ref;
    IEnumerator ActivatePopUp_Coroutine()
    {
        //  animacao de pop up (escala de 1.0 para 1.2 e depois para 0)
        yield return StartCoroutine(PopUpAnimation_Coroutine(objectToAnimate.transform, new Vector3(1.2f, 1.2f, 1.2f), Vector3.zero));

        // troca materiais
        iconBackgroundGraphic.material = iconBackgroundActiveMaterial;
        inputBackgroundGraphic.material = inputBackgroundActiveMaterial;

        // Ativa o script -- nao desativa dps, corrigir
        scriptToActivate.enabled = true;
        isActive = true;

        //  animacao de pop up (escala de 0 para 1.2 e depois para 1)
        yield return StartCoroutine(PopUpAnimation_Coroutine(objectToAnimate.transform, new Vector3(1.2f, 1.2f, 1.2f), Vector3.one));
    }

    IEnumerator DeactivatePopUp_Coroutine()
    {
        yield return StartCoroutine(PopUpAnimation_Coroutine(objectToAnimate.transform, new Vector3(1.2f, 1.2f, 1.2f), Vector3.zero));

        // Restaura os materiais originais dos objetos
        iconBackgroundGraphic.material = iconBackgroundOriginalMat;
        inputBackgroundGraphic.material = inputBackgroundOriginalMat;

        // nao desativa a funcao de while(true), verificar dps
        scriptToActivate.enabled = false;
        isActive = false;

        yield return StartCoroutine(PopUpAnimation_Coroutine(objectToAnimate.transform, new Vector3(1.2f, 1.2f, 1.2f), Vector3.one));
    }

    IEnumerator PopUpAnimation_Coroutine(Transform target, Vector3 midScale, Vector3 endScale) // nao encavalar as anim
    {
        yield return Tween.ScaleTransform(this, target, midScale, popUpDuration / 2, lerpType);
        yield return Tween.ScaleTransform(this, target, endScale, popUpDuration / 2, lerpType);
    }
}

