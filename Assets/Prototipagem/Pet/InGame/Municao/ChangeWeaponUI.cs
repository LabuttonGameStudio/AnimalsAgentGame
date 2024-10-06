using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class WeaponUIElements
{
    [Header("Icon")]
    public RectTransform iconRect;
    public Graphic iconGraphic;
    public CanvasGroup iconCanvasGroup;
    [Header("Crosshair")]
    public RectTransform crosshairRect;
    public CanvasGroup crosshairCanvasGroup;
}
public class ChangeWeaponUI : MonoBehaviour
{
    public static ChangeWeaponUI Instance;

    [Header("WEAPONS")]
    [SerializeField] private WeaponUIElements[] weaponUIElements;

    [Header("CROSSHAIR")]
    [SerializeField] private CanvasGroup SliderAmmunition; // slider da mira

    [Header("MATERIALS")]
    [SerializeField] private Material activeMaterial; // mat para arma ativa
    [SerializeField] private Material disabledMaterial; // para arma desativa

    [Header("ANIMATION")]
    [SerializeField] private float popupScale = 1.2f; // Tamanho do icone quando ativado
    [SerializeField] private float normalScale = 1f; // Tamanho do icone quando desativado
    [SerializeField] private float scaleDuration = 0.5f; // Duracaoo da transicao de escala
    [SerializeField] private Tween.LerpType lerpType = Tween.LerpType.Lerp;
    int selectedIndex;
    private void Awake()
    {
        Instance = this;
        selectedIndex = -1;
    }
    public void SelectItem(int index)
    {
        switch (index)
        {
            default:
            case 0:
            case 1:
                SliderAmmunition.alpha = 1f;
                break;
            case 2:
                SliderAmmunition.alpha = 0f;
                break;
        }
        if (selectedIndex != -1)
        {
            // reduz o icone anterior e aplica o material desativado
            StartCoroutine(ScaleDownItem(weaponUIElements[index].iconRect));
            weaponUIElements[selectedIndex].iconGraphic.material = disabledMaterial;
            weaponUIElements[selectedIndex].iconCanvasGroup.alpha = 0.25f;
            weaponUIElements[selectedIndex].crosshairCanvasGroup.alpha = 0f;


        }

        // aumenta o icone selecionado e aplica o material ativo
        StartCoroutine(ScaleUpItem(weaponUIElements[index].iconRect));
        weaponUIElements[index].iconGraphic.material = activeMaterial;
        weaponUIElements[index].iconCanvasGroup.alpha = 1f;
        weaponUIElements[index].crosshairCanvasGroup.alpha = 1f;

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

