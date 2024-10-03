using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChangeWeapon : MonoBehaviour
{
    [Header("WEAPONS")]
    public RectTransform[] WeaponsIcons; // icones armas

    [Header("CROSSHAIR")]
    public GameObject[] WeaponsCross; // crosshair das armas
    public CanvasGroup SliderAmmunition; // slider da mira

    [Header("MATERIALS")]
    public Material activeMaterial; // mat para arma ativa
    public Material deactivatedMaterial; // para arma desativa

    [Header("ANIMATION")]
    public float popupScale = 1.2f; // Tamanho do icone quando ativado
    public float normalScale = 1f; // Tamanho do icone quando desativado
    public float scaleDuration = 0.5f; // Duracaoo da transicao de escala
    public Tween.LerpType lerpType = Tween.LerpType.Lerp; 

    private int selectedIndex = 0; // Comeca com a arma da posiaco 1 selecionada

    private void Awake()
    {
        SelectItem(selectedIndex);
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            SelectItem(0); // Seleciona o primeiro item
            SliderAmmunition.GetComponent<CanvasGroup>().alpha = 1f;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            SelectItem(1); // Seleciona o segundo item
            SliderAmmunition.GetComponent<CanvasGroup>().alpha = 1f;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            SelectItem(2); // Seleciona o terceiro item
            SliderAmmunition.GetComponent<CanvasGroup>().alpha = 0f;
        }
    }

    void SelectItem(int index)
    {
        if (selectedIndex != -1)
        {
            // reduz o icone anterior e aplica o material desativado
            StartCoroutine(ScaleDownItem(WeaponsIcons[selectedIndex]));
            WeaponsIcons[selectedIndex].GetComponent<Graphic>().material = deactivatedMaterial;
            WeaponsIcons[selectedIndex].GetComponent<CanvasGroup>().alpha = 0.25f; 
            WeaponsCross[selectedIndex].GetComponent<CanvasGroup>().alpha = 0f;
            

        }

        // aumenta o icone selecionado e aplica o material ativo
        StartCoroutine(ScaleUpItem(WeaponsIcons[index]));
        WeaponsIcons[index].GetComponent<Graphic>().material = activeMaterial;
        WeaponsIcons[index].GetComponent<CanvasGroup>().alpha = 1f; 
        WeaponsCross[index].GetComponent<CanvasGroup>().alpha = 1f;

        selectedIndex = index; 
    }

    IEnumerator ScaleUpItem(RectTransform item)
    {
        // Anim do aumento de escala para o item selecionado
        Vector3 midScale = new Vector3(popupScale, popupScale, popupScale);
        yield return Tween.ScaleTransform(this, item, midScale, scaleDuration, lerpType);
    }

    IEnumerator ScaleDownItem(RectTransform item)
    {
        Vector3 normalScaleVector = new Vector3(normalScale, normalScale, normalScale);
        yield return Tween.ScaleTransform(this, item, normalScaleVector, scaleDuration, lerpType);
    }
}

