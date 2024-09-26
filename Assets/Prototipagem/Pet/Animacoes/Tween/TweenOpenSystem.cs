using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TweenOpenSystem : MonoBehaviour
{
    public RectTransform panelToOpen; // O painel que será animado
    public float openDuration = 1f; // Duracao da animacao
    public float TimeForStart = 0f;
    public Tween.LerpType lerpType; // Tipo de interpolacao (Lerp ou Slerp)

    private Vector2 originalScale; // Guardara a escala original do painel

    void Start()
    {
        // Começamos com o painel fechado (escala Y = 0)
        panelToOpen.localScale = new Vector3(panelToOpen.localScale.x, 0f, panelToOpen.localScale.z);
        OpenPanel();
    }

    public void OpenPanel()
    {

        // Inicia a animacao para abrir o painel
        StartCoroutine(OpenPanelAnimation());
    }
    public void ClosePanel()
    {
        StartCoroutine(ClosePanelAnimation());
        
    }

        private IEnumerator ClosePanelAnimation()
    {
        yield return new WaitForSeconds(TimeForStart);

        
        Vector3 startScale = new Vector3(panelToOpen.localScale.x, 1f, panelToOpen.localScale.z); // Começa com Y = 0
        Vector3 finalScale = new Vector3(panelToOpen.localScale.x, 0f, panelToOpen.localScale.z); // Depois ajusta para Y = 1 (tamanho final)

        // Volta para 1.0
        yield return Tween.ScaleTransform(this, panelToOpen, finalScale, openDuration / 2, lerpType);
    }

        private IEnumerator OpenPanelAnimation()
    {
        yield return new WaitForSeconds(TimeForStart);

        // Aumenta a escala Y de 0 até 1.2 e depois volta para 1.0
        Vector3 startScale = new Vector3(panelToOpen.localScale.x, 0f, panelToOpen.localScale.z); // Começa com Y = 0
        Vector3 finalScale = new Vector3(panelToOpen.localScale.x, 1f, panelToOpen.localScale.z); // Depois ajusta para Y = 1 (tamanho final)

        // Volta para 1.0
        yield return Tween.ScaleTransform(this, panelToOpen, finalScale, openDuration / 2, lerpType);
    }
}

