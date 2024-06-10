using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LoadingBar : MonoBehaviour
{
    public Slider slider;
    public TMP_Text progressText;
    public float totalTime = 10f;
    public CanvasGroup canvasGroup;
    public CanvasGroup canvasGroup2;
    private float currentTime = 0f;
    private bool isFadingIn = true;
    private bool isComplete = false;

    private float targetAlpha = 1f;
    private float initialAlpha = 0f;
    private float fadeDuration = 1f;
    private float currentFadeTime = 0f;

    void Start()
    {

        canvasGroup.alpha = initialAlpha;
    
        slider.minValue = 0f;
        slider.maxValue = 1f;
        slider.value = 0f;
        UpdateProgressText();
    }

    void Update()
    {
        // Se ainda estiver na fase de fading in do CanvasGroup
        if (isFadingIn)
        {
            FadeInCanvasGroup();
        }
        // Se o fading in ja estiver completo e o carregamento nao estiver completo
        else if (!isComplete)
        {
            // Atualiza o progresso do slider
            if (currentTime < totalTime)
            {
                currentTime += Time.deltaTime;
                slider.value = currentTime / totalTime;
                UpdateProgressText();
            }
            else
            {
                // O carregamento esta completo
                isComplete = true;

                // Inicia a animacao de fading out do CanvasGroup
                isFadingIn = false;
                currentFadeTime = 0f;
            }
        }
        // Se o fading out do CanvasGroup estiver em andamento
        else
        {
            FadeOutCanvasGroup();
        }
    }

    void FadeInCanvasGroup()
    {
        if (canvasGroup != null)
        {
            // Interpolacao do alpha do CanvasGroup
            if (currentFadeTime < fadeDuration)
            {
                currentFadeTime += Time.deltaTime;
                float t = currentFadeTime / fadeDuration;
                float newAlpha = Mathf.Lerp(initialAlpha, targetAlpha, t);
                canvasGroup.alpha = newAlpha;
            }
            else
            {
                isFadingIn = false;
            }
        }
    }

    void FadeOutCanvasGroup()
    {
        if (canvasGroup != null)
        {
            // Interpolacao do alpha do CanvasGroup
            if (currentFadeTime < fadeDuration)
            {
                currentFadeTime += Time.deltaTime;
                float t = currentFadeTime / fadeDuration;
                float newAlpha = Mathf.Lerp(targetAlpha, initialAlpha, t);
                canvasGroup.alpha = newAlpha;
                canvasGroup2.alpha = newAlpha;
            }
            else
            {
                canvasGroup.alpha = initialAlpha;
                canvasGroup2.alpha = initialAlpha;

            }
        }
    }

    void UpdateProgressText()
    {
        float percentage = slider.value * 100f;
        progressText.text = percentage.ToString("F0") + "%";
    }
}