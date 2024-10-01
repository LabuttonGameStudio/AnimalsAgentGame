using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Dialogue : MonoBehaviour
{
    [SerializeField]public bool pauseBetweenSentences;
    [Header("TEXT")]
    public DialogueText[] dialogue;

    [Header("EVENTS")]
    public DialogueEvent[] startDialogue;
    public DialogueEvent[] endDialogue;
    public void ShowDialogue()
    {
        DialogueBasicControl.Instance.StartDialogue(this);
    }

}
