using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class DialogueText 
{
    public string title;
    [Header("Infos")]
    public Sprite portrait;
    public string name;
    [TextArea(1, 20)] public string quote;

    [Header("Events")]
    public DialogueEvent[] eventOnDialogueBoxEnter;
}
