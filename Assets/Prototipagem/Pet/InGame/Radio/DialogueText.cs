using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Localization;
using UnityEngine.Localization.Components;

[Serializable]
public class DialogueText 
{
    [Header("Infos")]
    public Sprite portraitBackground;
    public Material portrait;
    public string name;
    [TextArea(1, 20)] public string quote;

    [Header("Events")]
    public DialogueEvent[] eventOnDialogueBoxEnter;
}
