using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.Localization.Tables;
using UnityEditor.Localization;


public class DialogueCoroutines
{
    public List<Coroutine> coroutines = new List<Coroutine>();
}

public class DialogueBasicControl : MonoBehaviour
{
    //Singleton
    public static DialogueBasicControl Instance { get; private set; }

    private DialogueCoroutines currentDialogueCoroutine;
    private Dialogue currentDialogue;

    [Header("COMPONENTS")]
    public CanvasGroup Radio;
    public Image IconBackground;
    public SpriteRenderer Waves;
    public Graphic Icon;
    public TMP_Text Skip;
    public TMP_Text Name;
    public TMP_Text AgentTitle;
    public TMP_Text DialogueText;

    [Header("SETTINGS")]
    public float VelocityText;
    public float TimeBetweenSentences;
    public float FadeDuration = 0.5f;

    [Header("Localization Table")]
    [SerializeField] private StringTableCollection characterTable;

    private void Awake()
    {
        Instance = this;
    }
    public void Start()
    {
        ArmadilloPlayerController.Instance.inputControl.inputAction.Dialogue.Enable();
        ArmadilloPlayerController.Instance.inputControl.inputAction.Dialogue.SkipDialogue.Enable();
        currentDialogueCoroutine = new DialogueCoroutines();
    }
    private IEnumerator SequenceDialogueEvents(DialogueEvent[] events)
    {
        for (int i = 0; i < events.Length; i++)
        {
            events[i].actionEvents.Invoke();
            if (events[i].delay > 0) yield return new WaitForSecondsRealtime(events[i].delay);
        }
    }

    public void StopCurrentDialogue()
    {
        if (currentDialogueCoroutine != null)
        {
            for (int i = 0; i < currentDialogueCoroutine.coroutines.Count; i++)
            {
                if (currentDialogueCoroutine.coroutines[i] != null) StopCoroutine(currentDialogueCoroutine.coroutines[i]);
            }
            currentDialogueCoroutine = null;
        }
    }

    public void StartDialogue(Dialogue dialogue)
    {
        StopCurrentDialogue();
        currentDialogueCoroutine = new DialogueCoroutines();
        currentDialogue = dialogue;

        IconBackground.sprite = dialogue.dialogue[0].portraitBackground;
        Icon.material = dialogue.dialogue[0].portrait;
        Name.text = dialogue.dialogue[0].name;
        AgentTitle.text = dialogue.dialogue[0].name;
        SetOpacity(1);

        currentDialogueCoroutine.coroutines.Add(StartCoroutine(Fade(Radio, 0f, 1f, FadeDuration)));

        currentDialogueCoroutine.coroutines.Add(StartCoroutine(SequenceDialogueEvents(dialogue.startDialogue)));

        if (dialogue.pauseBetweenSentences)
        {
            Skip.enabled = true;
            onDialogue_Ref = StartCoroutine(OnDialoguePauseOnEndSentence_Coroutine(dialogue));

        }
        else
        {
            onDialogue_Ref = StartCoroutine(OnDialogue_Coroutine(dialogue));

        }
        currentDialogueCoroutine.coroutines.Add(onDialogue_Ref);
    }
    public void EndDialogues(DialogueEvent[] endevent)
    {
        currentDialogueCoroutine.coroutines.Add(StartCoroutine(Fade(Radio, 1f, 0f, FadeDuration)));
        currentDialogueCoroutine.coroutines.Add(StartCoroutine(SequenceDialogueEvents(endevent)));
        SetOpacity(0);
    }


    private Coroutine onDialogue_Ref;
    private IEnumerator OnDialogue_Coroutine(Dialogue dialogue)
    {
        for (int i = 0; i < dialogue.dialogue.Length; i++)
        {
            IconBackground.sprite = dialogue.dialogue[i].portraitBackground;
            Icon.material = dialogue.dialogue[i].portrait;
            Name.text = dialogue.dialogue[i].name;
            AgentTitle.text = dialogue.dialogue[i].name;
            currentDialogueCoroutine.coroutines.Add(StartCoroutine(SequenceDialogueEvents(dialogue.dialogue[i].eventOnDialogueBoxEnter)));
            Coroutine typeCoroutine = StartCoroutine(TypeSentence_Coroutine(dialogue.dialogue[i].quote, false));
            currentDialogueCoroutine.coroutines.Add(typeCoroutine);
            yield return typeCoroutine;
            yield return new WaitForSecondsRealtime(TimeBetweenSentences); // espera um tempo antes de iniciar a proxima sentença
        }
        EndDialogues(dialogue.endDialogue);
        onDialogue_Ref = null;
    }
    private IEnumerator OnDialoguePauseOnEndSentence_Coroutine(Dialogue dialogue)
    {
        for (int i = 0; i < dialogue.dialogue.Length; i++)
        {
            IconBackground.sprite = dialogue.dialogue[i].portraitBackground;
            Icon.material = dialogue.dialogue[i].portrait;
            Name.text = dialogue.dialogue[i].name;
            AgentTitle.text = dialogue.dialogue[i].name;

            currentDialogueCoroutine.coroutines.Add(StartCoroutine(SequenceDialogueEvents(dialogue.dialogue[i].eventOnDialogueBoxEnter)));
            InputAction interactButton = ArmadilloPlayerController.Instance.inputControl.inputAction.Dialogue.SkipDialogue;

            Coroutine typeSequenceCoroutine = StartCoroutine(TypeSentence_Coroutine(dialogue.dialogue[i].quote, true));
            currentDialogueCoroutine.coroutines.Add(typeSequenceCoroutine);
            yield return typeSequenceCoroutine;
            while (true)
            {
                if (interactButton.WasReleasedThisFrame()) break;
                yield return null;
            }
            yield return new WaitForSecondsRealtime(0.1f);
            float timer = 0;
            bool breakBasedOnTime = false;
            while (true)
            {
                if (interactButton.WasPerformedThisFrame()) break;
                timer += Time.unscaledDeltaTime;
                if (timer > TimeBetweenSentences)
                {
                    breakBasedOnTime = true;
                    break;
                }
                yield return null;
            }
            if (!breakBasedOnTime)
            {
                while (true)
                {
                    if (interactButton.WasReleasedThisFrame()) break;
                    yield return null;
                }
            }
        }
        EndDialogues(dialogue.endDialogue);
        onDialogue_Ref = null;
        Skip.enabled = false;
    }
    IEnumerator TypeSentence_Coroutine(string sentence, bool canInputBreak)
    {
        DialogueText.text = "";

        InputAction interactButton = ArmadilloPlayerController.Instance.inputControl.inputAction.Dialogue.SkipDialogue;
        for (int i = 0; i < sentence.Length; i++)
        {
            if (sentence[i] == '<')
            {
                string commandLine = "<";
                for (int j = i + 1; j < sentence.Length; j++)
                {
                    if (sentence[j] != '>')
                    {
                        commandLine += sentence[j];
                    }
                    else
                    {
                        commandLine += ">";
                        DialogueText.text += commandLine;
                        i = j + 1;
                        break;
                    }
                }
            }
            DialogueText.text += sentence[i];
            bool breakInWhile = false;
            float timer = 0f;
            if (canInputBreak)
            {
                while (timer < 3f / VelocityText)
                {
                    if (interactButton.WasPerformedThisFrame())
                    {
                        breakInWhile = true;
                        break;
                    }
                    timer += Time.unscaledDeltaTime;
                    yield return null;
                }
                if (breakInWhile) break;
            }
            else
            {
                yield return new WaitForSecondsRealtime(3f / VelocityText);
            }
        }
        DialogueText.text = sentence;
    }

    public void SetOpacity(float opacityValue)
    {
       Color color = Waves.color;
       color.a = opacityValue;
       Waves.color = color;
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
            elapsedTime += Time.unscaledDeltaTime;
            yield return null;
        }
        canvasGroup.alpha = endAlpha;
    }


}



