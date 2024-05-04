using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class DialogueBasicControl : MonoBehaviour
{

    [Header("Components")]
    public CanvasGroup Radio;
    public Image Icon;
    public TMP_Text Name;
    public TMP_Text DialogueText;

    [Header("Settings")]
    public float VelocityText;
    public float TimeBetweenSentences;
    public float FadeDuration = 0.5f;

    private string[] sentences;
    private bool typing = false;
 
    public void Dialogues(Sprite icon, string name, string[] dialogue)
    {
        StartCoroutine(FadeRadio(Radio, 0f, 1f, FadeDuration));
        Icon.sprite = icon;
        Name.text = name;
        sentences = dialogue;
        StartCoroutine(TypeSentences());

        Debug.Log("ativei");
    }

    public void CloseDialogues()
    {
        StartCoroutine(FadeRadio(Radio, 1f, 0f, FadeDuration));
        Debug.Log("skipei");
    }

    IEnumerator TypeSentences()
    {
        foreach (string sentence in sentences)
        {
            typing = true;
            DialogueText.text = "";
            foreach (char letter in sentence.ToCharArray())
            {
                DialogueText.text += letter;
                yield return new WaitForSeconds(VelocityText);
            }
            typing = false;

            // Aguarda até que o texto seja completamente exibido antes de passar para a próxima frase
            while (typing)
            {
                yield return null;
            }

            yield return new WaitForSeconds(TimeBetweenSentences); // Espera um tempo antes de iniciar a próxima sentença
        }
        StartCoroutine(FadeRadio(Radio, 1f, 0f, FadeDuration));
    }

    IEnumerator FadeRadio(CanvasGroup canvasGroup, float startAlpha, float endAlpha, float duration)
    {
        float elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            canvasGroup.alpha = Mathf.Lerp(startAlpha, endAlpha, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        canvasGroup.alpha = endAlpha;
    }


}



