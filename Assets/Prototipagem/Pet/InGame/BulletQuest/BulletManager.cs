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

    [Header("ANIMACAO")]
    [SerializeField] private GameObject targetObject;
    [SerializeField] private string colorPropertyName = "_Color";

    [Header("INTERACTIVE OBJECTIVES - ANTENAS")]
    [SerializeField] private int totalAntenas;
    [SerializeField] private int requiredAntenas;
    [SerializeField] private List<Antena_Damageable> antenas;

    private int activatedAntenas = 0;
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

    #region Tutorial - fase 1
    public void Objetivo1()
    {
        ChangeColor(1.5f, 0.5f);
        StartCoroutine(WaitAndUpdateText("reconhecimento de campo", "Escale a montanha para novas instruções"));
    }

    public void Objetivo2()
    {
        ChangeColor(1.5f, 0.5f);
        StartCoroutine(WaitAndUpdateText("modo tático: cascadura", "Explore o terreno no modo cascadura"));
    }

    public void Objetivo3()
    {
        ChangeColor(1.5f, 0.5f);
        StartCoroutine(WaitAndUpdateText("furtividade", "Espreite através dos inimigos"));
    }
    public void OnAntennaActivated()
    {
        activatedAntenas++;
        // Atualiza o texto com o progresso atual
        UpdateText("energize todas as antenas", $"Antenas Ativadas: {activatedAntenas}/{requiredAntenas}");

        if (activatedAntenas >= requiredAntenas)
        {
            ChangeColor(1.5f, 0.5f);
            StartCoroutine(WaitAndUpdateText("antenas ativadas!", "Vá até o portão"));
        }
    }

        public void Objetivo4()
    {
        ChangeColor(1.5f, 0.5f);
        requiredAntenas = 2;
        StartCoroutine(WaitAndUpdateText("treine a mira", $"Ative todas as antenas: {activatedAntenas}/{requiredAntenas}"));
    }

    public void Objetivo5()
    {
        ChangeColor(1.5f, 0.5f);
        activatedAntenas = 0;
        requiredAntenas = 3;
        StartCoroutine(WaitAndUpdateText("energize todas as antenas", $"Antenas Ativadas: {activatedAntenas}/{requiredAntenas}"));
    }
    #endregion

    #region Cais - fase 2

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

