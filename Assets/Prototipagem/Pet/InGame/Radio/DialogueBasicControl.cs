using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;

public class DialogueBasicControl : MonoBehaviour
{

    [Header("COMPONENTS")]
    public CanvasGroup Radio;
    public Image Icon;
    public TMP_Text Skip;
    public TMP_Text Name;
    public TMP_Text DialogueText;

    [Header("EVENTS")]
    public UnityEvent onStartDialogue;
    public UnityEvent onEndDialogue;

    [Header("SETTINGS")]
    public float VelocityText;
    public float TimeBetweenSentences;
    public float FadeDuration = 0.5f;

    private string[] sentences;
    private bool typing = false;
    public bool typingEnd = false;

    public void StartDialogues(Sprite icon, string name, string[] dialogue, UnityEvent startevent)
    {
        StartCoroutine(Fade(Radio, 0f, 1f, FadeDuration));
        Icon.sprite = icon;
        Name.text = name;
        sentences = dialogue;

        if (startevent != null)
        {
            startevent.Invoke();
        }

        StartCoroutine(TypeSentences());
        Debug.Log("ativei");
    }

    public void EndDialogues(UnityEvent endevent)
    {
        if (endevent != null)
        {
            endevent.Invoke();
        }

        Debug.Log("evento finalizado");
    }

    public void CloseDialogues()
    {
       
        // verifica se o CanvasGroup esta visivel 
        if (Radio.alpha > 0)
        {
            StartCoroutine(Fade(Radio, 1f, 0f, FadeDuration));
            Debug.Log("skipei");
        }
        else
        {
            Debug.Log("Dialogo fechado");
        }
    }

    IEnumerator TypeSentences()
    {
       
        foreach (string sentence in sentences)
        {
            typing = true;
            typingEnd = false;
            DialogueText.text = "";
            foreach (char letter in sentence.ToCharArray())
            {
                DialogueText.text += letter;
                yield return new WaitForSeconds(VelocityText);
            }
            typing = false;

            // Aguarda ate que o texto seja completamente exibido antes de passar para a proxima frase
            while (typing)
            {
                yield return null;
            }

            yield return new WaitForSeconds(TimeBetweenSentences); // espera um tempo antes de iniciar a proxima sentença

        }

        typingEnd = true;
        // fecha dialogo, se ja estiver fechado nao faz nada
        CloseDialogues();
        
    }

    IEnumerator Fade(CanvasGroup canvasGroup, float startAlpha, float endAlpha, float duration)
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



