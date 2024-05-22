using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;

public class DialogueBasicControl : MonoBehaviour
{
    //Singleton
    public static DialogueBasicControl Instance { get; private set; }


    [Header("COMPONENTS")]
    public CanvasGroup Radio;
    public Image Icon;
    public TMP_Text Skip;
    public TMP_Text Name;
    public TMP_Text DialogueText;

    [Header("SETTINGS")]
    public float VelocityText;
    public float TimeBetweenSentences;
    public float FadeDuration = 0.5f;

    private void Awake()
    {
        Instance = this;
    }

    private IEnumerator SequenceDialogueEvents(DialogueEvent[] events)
    {
        for (int i = 0; i < events.Length; i++)
        {
            events[i].actionEvents.Invoke();
            if(events[i].delay>0) yield return new WaitForSeconds(events[i].delay);
        }
    }

    public void StartDialogue(Dialogue dialogue)
    {
        if (dialogue != null)
        {
            StopCoroutine(startDialogue_Ref);
        }
        startDialogue_Ref = StartCoroutine(StartDialogue_Coroutine(dialogue));
    }


    private Coroutine startDialogue_Ref;
    private IEnumerator StartDialogue_Coroutine(Dialogue dialogue)
    {
        Icon.sprite = dialogue.dialogue[0].portrait;
        Name.text = dialogue.dialogue[0].name;

        StartCoroutine(Fade(Radio, 0f, 1f, FadeDuration));

        yield return StartCoroutine(SequenceDialogueEvents(dialogue.startDialogue));

        StartCoroutine(TypeSentences());
        Debug.Log("ativei");
    }
    public void EndDialogues(DialogueEvent[] endevent)
    {
        if (endevent != null)
        {
            //endevent.Invoke();
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

    private Coroutine onDialogue_Ref;
    private IEnumerator OnDialogue_Coroutine()
    {

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


    public Coroutine StartFade(CanvasGroup canvasGroup, float startAlpha, float endAlpha, float duration)
    {
        return StartCoroutine(Fade(canvasGroup, startAlpha, endAlpha, duration));
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



