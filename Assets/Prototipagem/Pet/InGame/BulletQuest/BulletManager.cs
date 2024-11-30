using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class BulletManager : MonoBehaviour
{
    [Header("TEXT")]
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private TextMeshProUGUI SubdescriptionText;

    [Header("ANIMACAO")]
    [SerializeField] private GameObject targetObject;
    [SerializeField] private string colorPropertyName = "_Color";

    [Header("INTERACTIVE OBJECTIVES - INIMIGOS")]
    [SerializeField] private int requiredInimigos;
     public int InimigosDestruidos = 0;

    [Header("INTERACTIVE OBJECTIVES - GERADORES")]
    [SerializeField] private int requiredGeradores;
    public int GeradoresAtivados = 0;

    private void Awake()
    {
        Graphic targetGraphic = targetObject.GetComponent<Graphic>();
        targetGraphic.material = new Material(targetGraphic.material);
    }
    public void UpdateText(string newTitle, string newDescription)
    {
        titleText.text = newTitle;
        descriptionText.text = newDescription;
    }
    public void UpdateDescription(string newDescription)
    {
        SubdescriptionText.text = newDescription;
    }

    public void ChangeColor(float newIntensity, float duration)
    {
        Graphic targetGraphic = targetObject.GetComponent<Graphic>();
         StartCoroutine(ChangeHDRIntensityTemporarily(targetGraphic.material, newIntensity, duration));

    }

    IEnumerator WaitAndUpdateText(string newTitle, string newDescription)
    {
        yield return new WaitForSeconds(1f); // Espera pelo tempo definido
        UpdateText(newTitle, newDescription); // Atualiza o texto após o tempo de espera
    }

    IEnumerator WaitAndUpdateSubText(string newDescription)
    {

        yield return new WaitForSeconds(1f);
        UpdateDescription(newDescription); 

    }

    #region Tutorial - fase 1
    public void Objetivo1()
    {
        ChangeColor(6f, 0.5f);
        StartCoroutine(WaitAndUpdateText("reconhecimento de campo", "Escale a montanha para novas instruções"));
    }

    public void Objetivo2()
    {
        ChangeColor(6f, 0.5f);
        StartCoroutine(WaitAndUpdateText("modo tático: cascadura", "Explore o terreno no modo cascadura"));
    }

    public void Objetivo3()
    {
        ChangeColor(6f, 0.5f);
        StartCoroutine(WaitAndUpdateText("hora do combate!", "Avance pelos inimigos a frente"));
    }
    public void AllEnemiesDestroy()
    {
        InimigosDestruidos++;
        // Atualiza o texto com o progresso atual
        UpdateText("acabe com eles agente", $"inimigos destruidos: {InimigosDestruidos}/{requiredInimigos}");

        if (InimigosDestruidos >= requiredInimigos)
        {
            ChangeColor(6f, 0.5f);
            StartCoroutine(WaitAndUpdateText("ameaça eliminada!", "Vá até o portão"));
        }
    }

        public void Objetivo4()
    {
        ChangeColor(6f, 0.5f);
        requiredInimigos = 1;
        StartCoroutine(WaitAndUpdateText("treine a mira", $"Destrua o inimigo a frente: {InimigosDestruidos}/{requiredInimigos}"));
    }

    public void Objetivo5()
    {
        ChangeColor(6f, 0.5f);
        InimigosDestruidos = 0;
        requiredInimigos = 3;
        StartCoroutine(WaitAndUpdateText("acabe com eles agente", $"inimigos destruidos: {InimigosDestruidos}/{requiredInimigos}"));
    }
    #endregion

    #region Cais - fase 2

    public void Objetivo6()
    {
        ChangeColor(6f, 0.5f);
        StartCoroutine(WaitAndUpdateText("investigue a área", "Localize o sinal desconhecido"));
    }

    public void Objetivo7()
    {
        ChangeColor(6f, 0.5f);
        requiredGeradores = 3;
        StartCoroutine(WaitAndUpdateText("investigue a área", "Ache um caminho alternativo"));
        StartCoroutine(WaitAndUpdateSubText($"Ative os interruptores de energia: { GeradoresAtivados}/{requiredGeradores}"));
    }

    public void AllGeradoresActivated()
    {
        GeradoresAtivados++;
        UpdateDescription($"Ative os interruptores de energia: { GeradoresAtivados}/{requiredGeradores}");

        if (GeradoresAtivados >= requiredGeradores)
        {
            ChangeColor(6f, 0.5f);
            StartCoroutine(WaitAndUpdateSubText("Portão desativado"));
        }
    }

    public void Objetivo8()
    {
        ChangeColor(6f, 0.5f);
        InimigosDestruidos = 0;
        requiredInimigos = 11;
        StartCoroutine(WaitAndUpdateText("acabe com eles agente", $"inimigos destruidos: {InimigosDestruidos}/{requiredInimigos}"));
    }

    public void Objetivo9()
    {
        ChangeColor(6f, 0.5f);
        StartCoroutine(WaitAndUpdateText("Encontre o 1° interruptor", "investigue a área"));
    }

    public void Objetivo10()
    {
        ChangeColor(6f, 0.5f);
        InimigosDestruidos = 0;
        requiredInimigos = 4;
        StartCoroutine(WaitAndUpdateText("acabe com eles agente", $"inimigos destruidos: {InimigosDestruidos}/{requiredInimigos}"));
    }

    public void Objetivo11()
    {
        ChangeColor(6f, 0.5f);
        StartCoroutine(WaitAndUpdateText("Encontre o 2° interruptor", "investigue a área"));
    }

    #endregion

    #region Animacao
    // Corrotina para alterar a intensidade HDR por um tempo e depois retornar ao original
    private IEnumerator ChangeHDRIntensityTemporarily(Material targetMaterial, float targetIntensity, float duration)
    {

        Color originalColor = targetMaterial.GetColor(colorPropertyName);
        float originalIntensity = Mathf.Log(Mathf.Max(originalColor.maxColorComponent, 1), 2);

        Color targetColor = originalColor * Mathf.Pow(2, targetIntensity);

        float elapsedTime = 0;

        //aumentar brilho
        while (elapsedTime < duration)
        {
            float t = elapsedTime / duration;

            Color interpolatedColor = Color.Lerp(originalColor, targetColor, t);
            targetMaterial.SetColor(colorPropertyName, interpolatedColor);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        targetMaterial.SetColor(colorPropertyName, targetColor);
        yield return new WaitForSeconds(duration);

        elapsedTime = 0;

        //retornar
        while (elapsedTime < duration)
        {
            float t = elapsedTime / duration;
            Color interpolatedColor = Color.Lerp(targetColor, originalColor, t);
            targetMaterial.SetColor(colorPropertyName, interpolatedColor);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        targetMaterial.SetColor(colorPropertyName, originalColor);
    }
    #endregion
}

